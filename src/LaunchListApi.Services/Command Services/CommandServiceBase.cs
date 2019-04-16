using MediatR;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using LaunchListApi.Model;
using LaunchListApi.Model.DataAccess;
using LaunchListApi.Services.Mediator.Commands;
using LaunchListApi.Services.Mediator.DomainEvents;
using LaunchListApi.Services.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ridewise.Services.CommandServices
{
    public abstract class CommandServiceBase<T> : IRequestHandler<CreateCommand<T>, CommandResult<Guid>>,
                                                  IRequestHandler<CreateChildCommand<T>, CommandResult<Guid>>,
                                                  IRequestHandler<ModifyCommand<T>, CommandResult<Guid>>,
                                                  IRequestHandler<ModifyChildCommand<T>, CommandResult<Guid>>,
                                                  IRequestHandler<DeleteCommand<T>, CommandResult<Guid>>
        where T : class, IAggregateRoot, new()
    {

        protected readonly CurrentUser _currentUser;
        protected readonly IAuthorizationService _authzService;
        protected readonly LaunchListApiContext _dc;
        protected readonly IMediator _mediator;

        public CommandServiceBase()
        {
        }

        public CommandServiceBase(IAuthorizationService authzService, CurrentUser currentUser, IMediator mediator) : this(new LaunchListApiContext(), authzService, currentUser, mediator)
        {
        }

        public CommandServiceBase(LaunchListApiContext dc, IAuthorizationService authzService, CurrentUser currentUser, IMediator mediator)
        {
            this._dc = dc;
            this._authzService = authzService;
            this._currentUser = currentUser;
            this._mediator = mediator;
        }


        /// <summary>
        /// Handle CREATE commands
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CommandResult<Guid>> Handle(CreateCommand<T> request, CancellationToken cancellationToken)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.EventData))
                return new CommandResult<Guid>(ResultType.BadRequest);

            // Call extension point to get the action authorised for the user in question, and the specific resource
            var authResult = await this.AuthoriseCreate(request.EventData);

            if (authResult.ResultType == ResultType.AccessDenied)
            {
                // TODO: Move the Access Denied audit log creation to here, from the child classes?
                return new CommandResult<Guid>(authResult.ResultType);
            }
            else
            {
                // This is a Create event (and thus starts a new event stream) so we need to create the Event Stream Id
                Guid eventStreamId = Guid.NewGuid();

                // Save the domain event record to the database
                DomainEventType eventType = (DomainEventType)Enum.Parse(typeof(DomainEventType), string.Format("Create{0}Event", typeof(T).Name));
                DomainEvent domainEvent = new DomainEvent(request.EventData, _currentUser.UserName, eventType, eventStreamId);

                _dc.DomainEvents.Add(domainEvent);
                await _dc.SaveChangesAsync();

                // Now publish events to signal the domain model to be updated, and to update the audit log
                CreatedEvent<T> eventNotification = new CreatedEvent<T>(request.EventData, _currentUser.UserName, domainEvent.EventStreamId, domainEvent.Id);
                await _mediator.Publish(eventNotification);

                // Drop an AuditLogEvent onto the mediator, to dispatch a request to update the system audit log. Again, we're not going to wait for the outcome of this event. Just fire and forget.
                AuditLogEvent auditLogNotification = new AuditLogEvent(eventNotification.DomainEventType, _currentUser.UserName, domainEvent.EventStreamId, domainEvent.Id);
                await _mediator.Publish(auditLogNotification);

                // If the response was not 201 CREATED (or it is, and we have errors) then we need to ensure that we pass the correct response code back, along with any "error" messages

                //return new CommandResult<Guid>(eventStreamId);
                return new CommandResult<Guid>(authResult.ResultType, eventStreamId, authResult.Errors);
            }
        }

        /// <summary>
        /// Handle a CREATE command which specifies that a new entity should be added to a child collection on an aggregate
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CommandResult<Guid>> Handle(CreateChildCommand<T> request, CancellationToken cancellationToken)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.EventData) || request.ParentEventStreamId == Guid.Empty)
                return new CommandResult<Guid>(ResultType.BadRequest);

            if (await GetSpecificAggregateRoot(request.ParentEventStreamId) == null)
            {
                return new CommandResult<Guid>(ResultType.NothingFound);
            }

            // Call extension point to get the action authorised for the user in question, and the specific resource
            var authResult = await this.AuthoriseModify(request.EventData, request.ParentEventStreamId, request.ChildEntityType);

            if (authResult.ResultType == ResultType.AccessDenied)
            {
                // TODO: Move the Access Denied audit log creation to here, from the child classes?
                return new CommandResult<Guid>(authResult.ResultType);
            }
            else
            {
                // Create a domain event record for this aggregate, and this new child entity (This is an UPDATE for the aggregate, recorded as a Create of the new child)
                DomainEventType eventType = (DomainEventType)Enum.Parse(typeof(DomainEventType), string.Format("Create{0}Event", request.ChildEntityType.Name));
                DomainEvent domainEvent = new DomainEvent(request.EventData, _currentUser.UserName, eventType, request.ParentEventStreamId);

                // Save the domain event
                _dc.DomainEvents.Add(domainEvent);
                await _dc.SaveChangesAsync();

                // Publish domain event notification - this is published as a MODIFY on the aggregate as this will direct it to the correct Aggregate's domain serivce. 
                UpdatedEvent<T> eventNotification = new UpdatedEvent<T>(request.EventData, _currentUser.UserName, domainEvent.EventStreamId, domainEvent.Id);
                await _mediator.Publish(eventNotification);

                // Drop an AuditLogEvent onto the mediator, to dispatch a request to update the system audit log. Again, we're not going to wait for the outcome of this event. Just fire and forget.
                AuditLogEvent auditLogNotification = new AuditLogEvent(domainEvent.DomainEventType, _currentUser.UserName, domainEvent.EventStreamId, domainEvent.Id);
                await _mediator.Publish(auditLogNotification);

                // Return new command result, with the PARENT's event stream id.
                return new CommandResult<Guid>(request.ParentEventStreamId);
            }
        }

        /// <summary>
        /// Handle a MODIFY command which specifies that an aggregate should be modified in some way
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CommandResult<Guid>> Handle(ModifyCommand<T> request, CancellationToken cancellationToken)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(request.EventData) || request.ParentEventStreamId == Guid.Empty)
                return new CommandResult<Guid>(ResultType.BadRequest);

            if (await GetSpecificAggregateRoot(request.ParentEventStreamId) == null)
            {
                return new CommandResult<Guid>(ResultType.NothingFound);
            }


            // Call extension point to get the action authorised for the user in question, and the specific resource
            var authResult = await this.AuthoriseModify(request.EventData, request.ParentEventStreamId);

            if (authResult.ResultType == ResultType.AccessDenied)
            {
                // TODO: Move the Access Denied audit log creation to here, from the child classes?
                return new CommandResult<Guid>(authResult.ResultType);
            }

            // Create a domain event record for this aggregate, and this entity (This is an UPDATE for the aggregate)
            DomainEventType eventType = (DomainEventType)Enum.Parse(typeof(DomainEventType), string.Format("Modify{0}Event", typeof(T).Name));
            DomainEvent domainEvent = new DomainEvent(request.EventData, _currentUser.UserName, eventType, request.ParentEventStreamId);

            // Save the domain event
            _dc.DomainEvents.Add(domainEvent);
            await _dc.SaveChangesAsync();

            // Publish domain event notification - this is published as a MODIFY on the aggregate as this will direct it to the correct Aggregate's domain serivce. 
            UpdatedEvent<T> eventNotification = new UpdatedEvent<T>(request.EventData, _currentUser.UserName, domainEvent.EventStreamId, domainEvent.Id);
            await _mediator.Publish(eventNotification);

            // Drop an AuditLogEvent onto the mediator, to dispatch a request to update the system audit log. Again, we're not going to wait for the outcome of this event. Just fire and forget.
            AuditLogEvent auditLogNotification = new AuditLogEvent(domainEvent.DomainEventType, _currentUser.UserName, domainEvent.EventStreamId, domainEvent.Id);
            await _mediator.Publish(auditLogNotification);

            // Return new command result, with the PARENT's event stream id.
            return (authResult.Errors == null || authResult.Errors.Count == 0) ? new CommandResult<Guid>() : new CommandResult<Guid>(ResultType.OkForCommand, authResult.Errors);
        }

        /// <summary>
        /// Handle a DELETE command which specifies that an aggregate should be marked as deleted
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CommandResult<Guid>> Handle(DeleteCommand<T> request, CancellationToken cancellationToken)
        {
            // Validate input
            if (request.EventStreamId == Guid.Empty)
                return new CommandResult<Guid>(ResultType.BadRequest);

            if (await GetSpecificAggregateRoot(request.EventStreamId) == null)
            {
                return new CommandResult<Guid>(ResultType.NothingFound);
            }

            // Call extension point to get the action authorised for the user in question, and the specific resource
            var authResult = await this.AuthoriseDelete(request.EventStreamId);

            if (authResult.ResultType == ResultType.AccessDenied)
            {
                // TODO: Move the Access Denied audit log creation to here, from the child classes?
                return new CommandResult<Guid>(authResult.ResultType);
            }

            // Create a domain event record for this aggregate, and this entity (This is an UPDATE for the aggregate as we're not DELETING the record physically)
            string eventData = new JObject() { new JProperty("Status", EntityStatus.Cancelled.ToString()) }.ToString();
            DomainEventType eventType = (DomainEventType)Enum.Parse(typeof(DomainEventType), ($"Modify{typeof(T).Name}Event"));
            DomainEvent domainEvent = new DomainEvent(eventData, _currentUser.UserName, eventType, request.EventStreamId);

            // Save the domain event
            _dc.DomainEvents.Add(domainEvent);
            await _dc.SaveChangesAsync();
             
            // Publish domain event notification - this is published as a MODIFY on the aggregate as we are not physically deleting the record, rather we're changing its status. 
            UpdatedEvent<T> eventNotification = new UpdatedEvent<T>(eventData, _currentUser.UserName, domainEvent.EventStreamId, domainEvent.Id);
            await _mediator.Publish(eventNotification);

            // Drop an AuditLogEvent onto the mediator, to dispatch a request to update the system audit log. Again, we're not going to wait for the outcome of this event. Just fire and forget.
            // We are going to audit this as a DELETE even though it's enacted as a modify, for clarity
            AuditLogEvent auditLogNotification = new AuditLogEvent((DomainEventType)Enum.Parse(typeof(DomainEventType), ($"Delete{typeof(T).Name}Event")), _currentUser.UserName, domainEvent.EventStreamId, domainEvent.Id);
            await _mediator.Publish(auditLogNotification);

            // Return new command result, with the PARENT's event stream id.
            return new CommandResult<Guid>();
        }

        /// <summary>
        /// Handle a PATCh command which specifies updates on a child entity of the aggregate root
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CommandResult<Guid>> Handle(ModifyChildCommand<T> request, CancellationToken cancellationToken)
        {
            // Validate input
            if (request.ParentEventStreamId == Guid.Empty || request.ChildEntityId < 1 || request.ChildEntityType == null || string.IsNullOrWhiteSpace(request.EventData))
                return new CommandResult<Guid>(ResultType.BadRequest);

            T aggregateRoot = await GetSpecificAggregateRoot(request.ParentEventStreamId);

            // find the child collection
            var childCollectionProperty = aggregateRoot?.GetType().GetProperties().Where(p => p.PropertyType.GenericTypeArguments.Contains(request.ChildEntityType)).First();
            IEnumerable childCollection = (IEnumerable)childCollectionProperty?.GetMethod.Invoke(aggregateRoot, null);
            int childCollectionCount = childCollectionProperty == null || childCollection == null ? 0 : (int)childCollectionProperty.PropertyType.GetProperty("Count").GetValue(childCollection);

            bool childIdFound = false;

            if (childCollection != null)
            {
                foreach (dynamic child in childCollection)
                {
                    if (child.Id == request.ChildEntityId)
                    {
                        childIdFound = true;
                        break;
                    }
                }
            }

            // TODO: Return nothing found if the childentityId is not in the collection
            if (aggregateRoot == null || childCollection == null || childCollectionCount == 0 || childIdFound == false)
            {
                return new CommandResult<Guid>(ResultType.NothingFound);
            }

            // Call extension point to get the action authorised for the user in question, and the specific resource. 
            // This is a modify on a child entity, we're going to rely on the permission for modification of the parent entity for this.
            var authResult = await this.AuthoriseModify(request.EventData, request.ParentEventStreamId, request.ChildEntityType);

            if (authResult.ResultType == ResultType.AccessDenied)
            {
                // TODO: Move the Access Denied audit log creation to here, from the child classes?
                return new CommandResult<Guid>(authResult.ResultType);
            }

            // Create a domain event record for this aggregate, and this entity (This is an UPDATE for the aggregate, since it's updating child data owned by this aggregate root)
            DomainEventType eventType = (DomainEventType)Enum.Parse(typeof(DomainEventType), ($"Modify{request.ChildEntityType.Name}Event"));
            DomainEvent domainEvent = new DomainEvent(request.EventData, _currentUser.UserName, eventType, request.ParentEventStreamId, request.ChildEntityId);

            // Save the domain event
            _dc.DomainEvents.Add(domainEvent);
            await _dc.SaveChangesAsync();

            // Publish domain event notification - this is published as a MODIFY on the aggregate as we are not physically deleting the record, rather we're changing its status. 
            UpdatedEvent<T> eventNotification = new UpdatedEvent<T>(request.EventData, _currentUser.UserName, domainEvent.EventStreamId, domainEvent.Id);
            await _mediator.Publish(eventNotification);

            // Drop an AuditLogEvent onto the mediator, to dispatch a request to update the system audit log. Again, we're not going to wait for the outcome of this event. Just fire and forget.
            // We are going to audit this as a DELETE even though it's enacted as a modify, for clarity
            AuditLogEvent auditLogNotification = new AuditLogEvent((DomainEventType)Enum.Parse(typeof(DomainEventType), ($"Modify{request.ChildEntityType.Name}Event")), _currentUser.UserName, domainEvent.EventStreamId, domainEvent.Id);
            await _mediator.Publish(auditLogNotification);

            // Return new command result, with the PARENT's event stream id.
            return new CommandResult<Guid>();
        }

        /// <summary>
        /// Override this method in derived classes. This is called to perform type-specific and instance specific input validation and 
        /// user access authorisation.
        /// </summary>
        /// <param name="eventData">A <see cref="string"/> containing the JSON representation of the event data being operated on</param>
        /// <returns></returns>
        protected abstract Task<UpdateServiceResult<Guid>> AuthoriseCreate(string eventData);

        /// <summary>
        /// Override this method in derived classes. This is called to perform type-specific and instance specific input validation and 
        /// user access authorisation of MODIFY operations. This includes commands which add, remove or modify child entities.
        /// </summary>
        /// <param name="eventData">A <see cref="string"/> containing the JSON representation of the event data being operated on</param>
        /// <param name="eventStreamId">A <see cref="Guid"/> indicating the EventStreamId of the aggregate being modified.</param>
        /// <param name="childEntityType"?>The <see cref="Type"/>of the Child Entity being modified, if any. Null if the modification is to the aggregate root.</param>
        /// <returns>An <see cref="UpdateServiceResult{Guid}"/> containing the result of the authorisation test</returns>
        protected abstract Task<UpdateServiceResult<Guid>> AuthoriseModify(string eventData, Guid eventStreamId, Type childEntityType = null);

        /// <summary>
        /// Override this method in derived classes. This is called to perform type-specific and instance specific input validation and 
        /// user access authorisation of DELETE operations on aggregate root entities.
        /// </summary>
        /// <param name="eventStreamId">A <see cref="Guid"/> indicating the EventStreamid of the aggregate being deleted</param>
        /// <returns>An <see cref="UpdateServiceResult{Guid}"/> containing the result of the authorisation test</returns>
        protected abstract Task<UpdateServiceResult<Guid>> AuthoriseDelete(Guid eventStreamId);

        /// <summary>
        /// Override this method in derived classes. Retrieve an instance of the aggregate root class which matches the <paramref name="eventStreamId"/> provided.
        /// </summary>
        /// <param name="eventStreamId"></param>
        /// <returns></returns>
        protected abstract Task<T> GetSpecificAggregateRoot(Guid eventStreamId);
    }
}
