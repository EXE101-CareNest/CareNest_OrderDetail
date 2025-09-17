using CareNest_OrderDetail.Application.Exceptions;
using CareNest_OrderDetail.Application.Interfaces.CQRS.Commands;
using CareNest_OrderDetail.Application.Interfaces.UOW;
using CareNest_OrderDetail.Domain.Commons.Constant;
using CareNest_OrderDetail.Domain.Entitites;
using Shared.Helper;

namespace CareNest_OrderDetail.Application.Features.Commands.Update
{
    public class UpdateCommandHandler : ICommandHandler<UpdateCommand, OrderDetail>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderDetail> HandleAsync(UpdateCommand command)
        {
            // Gọi validator để kiểm tra dữ liệu
            //Validate.ValidateUpdate(command);

            // Tìm để cập nhật
            OrderDetail? orderDetail = await _unitOfWork.GetRepository<OrderDetail>().GetByIdAsync(command.Id)
               ?? throw new BadRequestException("Id: " + MessageConstant.NotFound);


            orderDetail.TotalAmount = command.TotalAmount;
            orderDetail.Quantity = command.Quantity;
            orderDetail.OrderId = command.OrderId;
            orderDetail.ProductDetailId = command.ProductDetailId;
            orderDetail.UpdatedAt = TimeHelper.GetUtcNow();

            _unitOfWork.GetRepository<OrderDetail>().Update(orderDetail);
            await _unitOfWork.SaveAsync();
            return orderDetail;

        }
    }
}
