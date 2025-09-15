using CareNest_Order.Domain.Commons;

namespace CareNest_OrderDetail.Domain.Entitites
{
    public class OrderDetail : BaseEntity
    {
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
