using MediatR;
using Microsoft.EntityFrameworkCore;
using LaunchListApi.Model;
using LaunchListApi.Model.DataAccess;
using LaunchListApi.Services.Mediator.DomainEvents;
using LaunchListApi.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ridewise.Services.DomainServices
{
    public abstract class DomainServiceBase<T>: INotificationHandler<CreatedEvent<T>>,
                                                INotificationHandler<UpdatedEvent<T>>
        where T: class, IAggregateRoot, new()
    {
        protected LaunchListApiContext _dc;

        public DomainServiceBase() : this(new LaunchListApiContext())
        {

        }

        public DomainServiceBase(LaunchListApiContext dc)
        {
            this._dc = dc;
        }

        /// <summary>
        /// Handle the CreatedEvent for objects of type T
        /// </summary>
        /// <param name="notification"></param>
        /// <param name="cancellationToken"></param>
        public async Task Handle(CreatedEvent<T> notification, CancellationToken cancellationToken)
        {
            await HandleNotifications(notification);
        }

        /// <summary>
        /// Handle the UpdatedEvent for objects of type T
        /// </summary>
        /// <param name="notification">The event to handle</param>
        /// <param name="cancellationToken">A cancellation token</param>
        public async Task Handle(UpdatedEvent<T> notification, CancellationToken cancellationToken)
        {
            await HandleNotifications(notification);
        }

        private async Task HandleNotifications(DomainEventNotificationBase notification)
        {
            Guid eventStreamId = notification.EventStreamId;

            // If the event stream id is empty then we throw an error. 
            if (eventStreamId == Guid.Empty)
                throw new ArgumentException($"An attempt was made to process an event stream for an aggregate of type {typeof(T).Name} using an empty Event Stream Id.");

            // Retrieve the current "snapshot" of the client aggregate, or create a new one if we're doing a Create, or for some other reason one doesn't exist already
            T aggregateRecord = await this.GetAggregate(eventStreamId) ?? new T();

            // Process the event stream against this instance of the aggregate 
            var result = await this.ProcessEventStream(aggregateRecord, notification.DomainEventType, eventStreamId);
            
            if (result.IsSuccessResult() == false)
            {
                throw new ArgumentException($"An error occurred whilst attempting to process the events relating to a aggregate of type {typeof(T).Name}; Event stream id {eventStreamId}");
            }
        }
               
        /// <summary>
        /// Process the stream of events identified by <paramref name="eventStreamId"/> against the aggregate provided in <paramref name="aggregateRecord"/>
        /// </summary>
        /// <param name="aggregateRecord"></param>
        /// <param name="eventType"></param>
        /// <param name="eventStreamId"></param>
        /// <returns></returns>
        private async Task<UpdateServiceResult<Guid>> ProcessEventStream(T aggregateRecord, DomainEventType eventType, Guid eventStreamId)
        {
            try
            {
                // Get the set of events in the event stream that are NEWER than the current snapshot
                List<DomainEvent> eventStream = _dc.DomainEvents.Where(e => e.EventStreamId == eventStreamId && e.Timestamp.CompareTo(aggregateRecord.EventVersionTimestamp) > 0).ToList();

                if (eventStream.Any() == false)
                {
                    return new UpdateServiceResult<Guid>(ResultType.NothingFound, new List<string>() { "Error - an attempt was made to process an event stream using an EventStreamId that was not present in the event data store, or where every event is older than the current aggregate snapshot view." });
                }

                // Loop through the event stream applying the changes in chronological order.
                foreach (DomainEvent e in eventStream)
                {
                    try
                    {
                        aggregateRecord = await this.ProcessEvent(e, aggregateRecord);
                    }
                    catch (Exception ex)
                    {
                        // TODO: Decide whether to explictly roll back the transaction - might be best
                        List<string> errors = new List<string>();
                        errors.Add(ex.Message);
                        return new UpdateServiceResult<Guid>(ResultType.InternalServerError, errors);
                    }
                }

                // Ensure the aggregate is updated with the timestamp of the latest event
                aggregateRecord.EventVersionTimestamp = eventStream.OrderBy(e => e.Timestamp).Last().Timestamp;

                // If we've created a NEW domain object, then add it to the DC, otherwise mark it as modified
                if (_dc.Entry<T>(aggregateRecord).State == EntityState.Detached)
                    _dc.Add<T>(aggregateRecord);

                // Save to the data store
                await _dc.SaveChangesAsync();

                // Return a Good result containing the Id of the new record
                return eventType.IsCreateEvent() ? new UpdateServiceResult<Guid>(aggregateRecord.EventStreamId) : new UpdateServiceResult<Guid>();
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>();
                errors.Add(ex.Message);

                return new UpdateServiceResult<Guid>(ResultType.InternalServerError, errors);
            }
        }

        /// <summary>
        /// <para>When overridden in a derived class, this method returns an aggregate of the type specified by <see cref="T"/>.</para>
        /// <para>Given the provided <paramref name="eventStreamId"/>, return the most recent snapshot of the aggregate type that has a matching <see cref="IAggregateRoot.EventStreamId"/> property.</para>
        /// </summary>
        /// <param name="eventStreamId">The identifier of the event stream being processed</param>
        /// <returns>A <see cref="Task"/> that wraps an async operation returning an aggregate of type <see cref="T"/></returns>
        /// <remarks>The aggregate returned should include all navigation properties.</remarks>
        protected abstract Task<T> GetAggregate(Guid eventStreamId);

        /// <summary>
        /// Process a single <see cref="DomainEvent"/> against the implementation of <see cref="IAggregateRoot"/> provided in <paramref name="aggregateRecord"/>
        /// </summary>
        /// <param name="evt">The <see cref="DomainEvent"/> to process.</param>
        /// <param name="aggregateRecord">An instance of a <see cref="Type"/> that implements <see cref="IAggregateRoot"/>.</param>
        /// <returns>An instance of type <see cref="T"/>, which is the result of the application of the <see cref="DomainEvent"/> in <paramref name="evt"/> on the provided <paramref name="aggregateRecord"/>.</returns>
        protected abstract Task<T> ProcessEvent(DomainEvent evt, T aggregateRecord);
    }
}
