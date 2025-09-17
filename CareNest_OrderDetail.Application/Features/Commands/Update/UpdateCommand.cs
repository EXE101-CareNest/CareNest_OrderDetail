using CareNest_OrderDetail.Application.Interfaces.CQRS.Commands;
using CareNest_OrderDetail.Domain.Entitites;

namespace CareNest_OrderDetail.Application.Features.Commands.Update
{
    public class UpdateCommand : ICommand<OrderDetail>
    {
        public string Id { get; set; } = string.Empty;
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
