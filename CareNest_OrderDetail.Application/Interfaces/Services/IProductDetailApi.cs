using System.Threading.Tasks;

namespace CareNest_OrderDetail.Application.Interfaces.Services
{
    // Giữ các DTO để tái sử dụng với client generic
    public class ProductDetailDto
    {
        public string? Id { get; set; }
        public string? CategoryId { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public bool Status { get; set; }
        public double? Discount { get; set; }
        public bool IsDefault { get; set; }
        public string? ImgUrls { get; set; }
        public int QuantityInStock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class ProductDetailUpdateDto
    {
        public string? Name { get; set; }
        public double Price { get; set; }
        public bool Status { get; set; }
        public double? Discount { get; set; }
        public bool IsDefault { get; set; }
        public string? ImgUrls { get; set; }
        public int QuantityInStock { get; set; }
    }
}


