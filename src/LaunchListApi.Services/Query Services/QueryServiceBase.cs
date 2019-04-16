using MediatR;
using Microsoft.AspNetCore.Authorization;
using LaunchListApi.Model;
using LaunchListApi.Services.Mediator.DomainEvents;
using LaunchListApi.Services.Mediator.Queries;
using LaunchListApi.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LaunchListApi.Model.DataAccess;

namespace LaunchListApi.Services.QueryServices
{
    public abstract class QueryServiceBase<T> : IRequestHandler<GetCountQuery<T>, QueryCountResult>,
                                                IRequestHandler<GetListQuery<T>, QueryResultList<T>>,
                                                IRequestHandler<GetSingleAggregateQuery<T>, QueryResult<T>>
        where T : class, IAggregateRoot, new()
    {
        protected readonly CurrentUser _currentUser;
        protected readonly IAuthorizationService _authzService;
        protected readonly LaunchListApiContext _dc;
        protected readonly IMediator _mediator;

        public QueryServiceBase()
        {

        }

        public QueryServiceBase(IAuthorizationService authzService, CurrentUser currentUser, IMediator mediator) : this(new LaunchListApiContext(), authzService, currentUser, mediator)
        {
        }

        public QueryServiceBase(LaunchListApiContext dc, IAuthorizationService authzService, CurrentUser currentUser, IMediator mediator)
        {
            this._dc = dc;
            this._authzService = authzService;
            this._currentUser = currentUser;
            this._mediator = mediator;
        }

        // Generic query message handlers
        public async Task<QueryCountResult> Handle(GetCountQuery<T> request, CancellationToken cancellationToken)
        {
            // Drop an AuditLogEvent onto the mediator, to dispatch a request to update the system audit log. Again, we're not going to wait for the outcome of this event. Just fire and forget.
            string dataAccessTypeString = string.Format("Get{0}Count", typeof(T).Name);
            var dataAccessType = (DataAccessType)Enum.Parse(typeof(DataAccessType), dataAccessTypeString);
            AuditLogEvent auditLogNotification = new AuditLogEvent(_currentUser.UserName, Guid.Empty, dataAccessType);
            await _mediator.Publish(auditLogNotification);

            // Call the GetCount method to retrieve the client count.
            var result = await GetCount();
            if (result.ResultType == ResultType.OkForQuery)
            {
                // Return the result list identifier
                return new QueryCountResult(result.Count);
            }
            else
            {
                if (result.ResultType == ResultType.AccessDenied)
                {
                    // Drop an AuditLogEvent onto the mediator to indicate that this action was denied
                    AuditLogEvent accessDeniedAuditEntry = new AuditLogEvent(_currentUser.UserName, Guid.Empty, dataAccessType, true);
                    await _mediator.Publish(accessDeniedAuditEntry);
                }

                // Query returned a non-success result, so return the result, and the errors
                return new QueryCountResult(result.ResultType);
            }
        }

        public async Task<QueryResultList<T>> Handle(GetListQuery<T> request, CancellationToken cancellationToken)
        {
            // Drop an AuditLogEvent onto the mediator, to dispatch a request to update the system audit log. Again, we're not going to wait for the outcome of this event. Just fire and forget.
            string dataAccessTypeString = string.Format("Get{0}List", typeof(T).Name);
            var dataAccessType = (DataAccessType)Enum.Parse(typeof(DataAccessType), dataAccessTypeString);
            AuditLogEvent auditLogNotification = new AuditLogEvent(_currentUser.UserName, Guid.Empty, dataAccessType);
            await _mediator.Publish(auditLogNotification);

            // Call the GetList method to get a list of the requested aggregate type, using the provided paging parameters and filter
            var result = await GetList(request.Paging);

            if (result.ResultType == ResultType.OkForQuery)
            {
                // Return the result list identifier
                return new QueryResultList<T>(result.Content);
            }
            else
            {
                if (result.ResultType == ResultType.AccessDenied)
                {
                    // Drop an AuditLogEvent onto the mediator to indicate that this action was denied
                    AuditLogEvent accessDeniedAuditEntry = new AuditLogEvent(_currentUser.UserName, Guid.Empty, dataAccessType, true);
                    await _mediator.Publish(accessDeniedAuditEntry);
                }

                // Query returned a non-success result, so return the result, and the errors
                return new QueryResultList<T>(result.ResultType);
            }
        }

        public async Task<QueryResult<T>> Handle(GetSingleAggregateQuery<T> request, CancellationToken cancellationToken)
        {
            // Drop an AuditLogEvent onto the mediator, to dispatch a request to update the system audit log. Again, we're not going to wait for the outcome of this event. Just fire and forget.
            string dataAccessTypeString = string.Format("Get{0}", typeof(T).Name);
            var dataAccessType = (DataAccessType)Enum.Parse(typeof(DataAccessType), dataAccessTypeString);
            AuditLogEvent auditLogNotification = new AuditLogEvent(_currentUser.UserName, request.RequestedEventStreamId, dataAccessType);
            await _mediator.Publish(auditLogNotification);

            if (request.RequestedEventStreamId == Guid.Empty)
                return new QueryResult<T>(ResultType.BadRequest);

            // Call the GetSingleAggregate method to get a list of the requested aggregate type, using the provided paging parameters and filter
            var result = await GetSingleAggregate(request.RequestedEventStreamId);

            if (result.ResultType == ResultType.OkForQuery)
            {
                // Return the result list identifier
                return new QueryResult<T>(result.Content);
            }
            else
            {
                if (result.ResultType == ResultType.AccessDenied)
                {
                    // Drop an AuditLogEvent onto the mediator to indicate that this action was denied
                    AuditLogEvent accessDeniedAuditEntry = new AuditLogEvent(_currentUser.UserName, request.RequestedEventStreamId, dataAccessType, true);
                    await _mediator.Publish(accessDeniedAuditEntry);
                }

                // Query returned a non-success result, so return the result, and the errors
                return new QueryResult<T>(result.ResultType);
            }
        }

        // Extension points - these need to be implemented by sub classes serving as Aggregate-specific services
        protected abstract Task<QueryServiceCountResult<T>> GetCount();

        protected abstract Task<QueryServiceResultList<T>> GetList(PagingParameters paging);

        protected abstract Task<QueryServiceResult<T>> GetSingleAggregate(Guid eventStreamId);
    }
}
