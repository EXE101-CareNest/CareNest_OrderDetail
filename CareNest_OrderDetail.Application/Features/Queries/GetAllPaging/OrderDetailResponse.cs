namespace CareNest_OrderDetail.Application.Features.Queries.GetAllPaging
{
    public class OrderDetailResponse
    {
        /// <summary>
        /// Id đơn hàng
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// Id chi tiết sản phẩm 
        /// </summary>
        public string? ProductDetailId { get; set; }
        /// <summary>
        /// Id đơn hàng
        /// </summary>
        public string? OrderId { get; set; }
        /// <summary>
        /// số lượng thú cưng
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// tổng tiền đơn hàng
        /// </summary>
        public double TotalAmount { get; set; }
    }
}
