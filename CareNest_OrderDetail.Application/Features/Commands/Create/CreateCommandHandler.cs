using CareNest_OrderDetail.Application.Exceptions.Validators;
using CareNest_OrderDetail.Application.Interfaces.CQRS.Commands;
using CareNest_OrderDetail.Application.Interfaces.UOW;
using CareNest_OrderDetail.Domain.Entitites;
using Shared.Helper;

namespace CareNest_OrderDetail.Application.Features.Commands.Create
{
    public class CreateCommandHandler : ICommandHandler<CreateCommand, OrderDetail>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderDetail> HandleAsync(CreateCommand command)
        {
            Validate.ValidateCreate(command);

            // tính tổng giá của 1 sản phẩm * số lượng  


            OrderDetail orderDetail = new()
            {
                Quantity = command.Quantity,
                ProductDetailId = command.ProductDetailId,
                OrderId = command.OrderId,
                TotalAmount = command.TotalAmount,
                CreatedAt = TimeHelper.GetUtcNow()
            };
            await _unitOfWork.GetRepository<OrderDetail>().AddAsync(orderDetail);
            await _unitOfWork.SaveAsync();

            return orderDetail;
        }
    }
}
