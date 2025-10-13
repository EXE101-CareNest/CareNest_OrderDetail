using CareNest_OrderDetail.Application.Common;
using CareNest_OrderDetail.Application.Interfaces.CQRS.Queries;
using CareNest_OrderDetail.Application.Interfaces.UOW;
using CareNest_OrderDetail.Domain.Entitites;

namespace CareNest_OrderDetail.Application.Features.Queries.GetAllPaging
{
    public class GetAllPagingQueryHandler : IQueryHandler<GetAllPagingQuery, PageResult<OrderDetailResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllPagingQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PageResult<OrderDetailResponse>> HandleAsync(GetAllPagingQuery query)
        {
            var selector = ObjectMapperExtensions.CreateMapExpression<OrderDetail, OrderDetailResponse>();

            var orderByFunc = GetOrderByFunc(query.SortColumn, query.SortDirection);

            // Build predicate for filtering by OrderId if provided
            System.Linq.Expressions.Expression<Func<OrderDetail, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(query.OrderId))
            {
                predicate = od => od.OrderId == query.OrderId;
            }

            // Get total count with the same predicate (without paging)
            var repo = _unitOfWork.GetRepository<OrderDetail>();
            var baseQuery = repo.Entities.AsQueryable();
            if (predicate != null)
            {
                baseQuery = baseQuery.Where(predicate);
            }
            int totalItems = baseQuery.Count();

            IEnumerable<OrderDetailResponse> items = await repo.FindAsync(
                predicate: predicate,
                orderBy: orderByFunc,
                selector: selector,
                pageSize: query.PageSize,
                pageIndex: query.Index);

            return new PageResult<OrderDetailResponse>(items, totalItems, query.Index, query.PageSize);
        }


        private Func<IQueryable<OrderDetail>, IOrderedQueryable<OrderDetail>> GetOrderByFunc(string? sortColumn, string? sortDirection)
        {
            var ascending = string.IsNullOrWhiteSpace(sortDirection) || sortDirection.ToLower() != "desc";

            return sortColumn?.ToLower() switch
            {
                "updateat" => q => ascending ? q.OrderBy(a => a.UpdatedAt) : q.OrderByDescending(a => a.UpdatedAt),
                _ => q => q.OrderBy(a => a.CreatedAt) // fallback nếu không có sortColumn
            };
        }
    }
}
