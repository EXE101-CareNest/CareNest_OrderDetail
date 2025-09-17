using CareNest_OrderDetail.Application.Common;
using CareNest_OrderDetail.Application.Interfaces.CQRS.Queries;

namespace CareNest_OrderDetail.Application.Features.Queries.GetAllPaging
{
    public class GetAllPagingQuery : IQuery<PageResult<OrderDetailResponse>>
    {
        public int Index { get; set; }
        public int PageSize { get; set; }
        public string? SortColumn { get; set; } // "Name", "Note", "CreatedAt"
        public string? SortDirection { get; set; } // "asc" or "desc"
    }
}
