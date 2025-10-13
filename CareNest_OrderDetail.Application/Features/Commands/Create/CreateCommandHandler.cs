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
        private readonly IAPIService _apiService;

        public CreateCommandHandler(IUnitOfWork unitOfWork, IAPIService apiService)
        {
            _unitOfWork = unitOfWork;
            _apiService = apiService;
        }

        public async Task<OrderDetail> HandleAsync(CreateCommand command)
        {
            Validate.ValidateCreate(command);

            // Lấy thông tin ProductDetail để tính giá và tồn kho
            var productRes = await _apiService.GetAsync<ProductDetailDto>("product", $"/api/ProductDetails/{command.ProductDetailId}");
            if (!productRes.IsSuccess || productRes.Data == null)
            {
                throw new ArgumentException(productRes.Message ?? $"Không tìm thấy chi tiết sản phẩm với ID: {command.ProductDetailId}");
            }
            var product = productRes.Data;

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

            var updateRes = await _apiService.PutAsync<object>("product", $"/api/ProductDetails/{command.ProductDetailId}", updateRequest);
            if (!updateRes.IsSuccess)
            {
                throw new ArgumentException(updateRes.Message ?? "Không thể cập nhật tồn kho cho sản phẩm");
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
