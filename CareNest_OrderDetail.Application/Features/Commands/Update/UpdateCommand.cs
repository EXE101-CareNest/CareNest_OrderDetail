using CareNest_OrderDetail.Application.Interfaces.CQRS.Commands;
using CareNest_OrderDetail.Domain.Commons.Enum;
using CareNest_OrderDetail.Domain.Entitites;

namespace CareNest_OrderDetail.Application.Features.Commands.Update
{
    public class UpdateCommand : ICommand<Order>
    {
        public string Id { get; set; } = string.Empty;
        /// <summary>
        /// Id khách hàng order
        /// </summary>
        public string? CustomerId { get; set; }
        /// <summary>
        /// Id cửa hàng bán
        /// </summary>
        public string? ShopId { get; set; }
        /// <summary>
        /// Id địa chỉ giao hàng cuỷa khách hàng
        /// </summary>
        public string? ShipAddressId { get; set; }
        /// <summary>
        /// tổng tiền đơn hàng
        /// </summary>
        public double TotalAmount { get; set; }
        /// <summary>
        /// phương thức thanh toán
        /// </summary>
        public string? PaymentMethod { get; set; }
        /// <summary>
        /// ghi chú
        /// </summary>
        public string? Note { get; set; }
        /// <summary>
        /// trạng thái: Pending / Confirmed / Checkin / Processing / Finished / Cancel
        /// </summary>
        public OrderStatus? Status { get; set; }
    }
}
