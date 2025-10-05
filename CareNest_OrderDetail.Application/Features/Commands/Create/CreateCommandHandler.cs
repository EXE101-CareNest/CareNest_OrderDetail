using CareNest_OrderDetail.Application.Exceptions.Validators;
using CareNest_OrderDetail.Application.Interfaces.CQRS.Commands;
using CareNest_OrderDetail.Application.Interfaces.UOW;
using CareNest_OrderDetail.Application.Interfaces.Services;
using CareNest_OrderDetail.Domain.Entitites;
using Shared.Helper;

namespace CareNest_OrderDetail.Application.Features.Commands.Create
{
    public class CreateCommandHandler : ICommandHandler<CreateCommand, OrderDetail>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductDetailApi _productDetailApi;

        public CreateCommandHandler(IUnitOfWork unitOfWork, IProductDetailApi productDetailApi)
        {
            _unitOfWork = unitOfWork;
            _productDetailApi = productDetailApi;
        }

        public async Task<OrderDetail> HandleAsync(CreateCommand command)
        {
            Validate.ValidateCreate(command);

            // Lấy thông tin ProductDetail để tính giá và tồn kho
            var product = await _productDetailApi.GetByIdAsync(command.ProductDetailId!);
            if (product == null)
            {
                throw new ArgumentException($"Không tìm thấy chi tiết sản phẩm với ID: {command.ProductDetailId}");
            }

            int newQuantityInStock = product.QuantityInStock - command.Quantity;
            if (newQuantityInStock < 0)
            {
                throw new ArgumentException("Hàng trong kho đã hết");
            }

            // Trừ tồn kho ngay trên ProductDetail (PUT full body)
            var updateRequest = new ProductDetailUpdateDto
            {
                Name = product.Name,
                Price = product.Price,
                Status = product.Status,
                Discount = product.Discount,
                IsDefault = product.IsDefault,
                ImgUrls = product.ImgUrls,
                QuantityInStock = newQuantityInStock
            };

            var updated = await _productDetailApi.UpdateAsync(command.ProductDetailId!, updateRequest);
            if (!updated)
            {
                throw new ArgumentException("Không thể cập nhật tồn kho cho sản phẩm");
            }

            double lineTotal = product.Price * command.Quantity;

            OrderDetail orderDetail = new()
            {
                Quantity = command.Quantity,
                ProductDetailId = command.ProductDetailId,
                OrderId = command.OrderId,
                TotalAmount = lineTotal,
                CreatedAt = TimeHelper.GetUtcNow()
            };
            await _unitOfWork.GetRepository<OrderDetail>().AddAsync(orderDetail);
            await _unitOfWork.SaveAsync();

            return orderDetail;
        }
    }
}
