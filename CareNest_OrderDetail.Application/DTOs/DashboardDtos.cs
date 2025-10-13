namespace CareNest_OrderDetail.Application.DTOs
{
    public class ShopSummaryDto
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int? Status { get; set; }
        public string? ImgUrl { get; set; }
    }

    public class ShopItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string OwnerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Status { get; set; }
        public string ImgUrl { get; set; } = string.Empty;
        public string WorkingDays { get; set; } = string.Empty;
    }

    public class ProductCategoryItemDto
    {
        public string Id { get; set; } = string.Empty;
        public string ShopId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? UpdatedBy { get; set; }
    }

    public class PagedListDto<T>
    {
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)(TotalCount) / (PageSize <= 0 ? 1 : PageSize));
    }

    public class DashboardResponseDto
    {
        public string? ShopId { get; set; }
        public ShopSummaryDto? Shop { get; set; }
        public object Data { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)(TotalCount) / (PageSize <= 0 ? 1 : PageSize));
    }
}


