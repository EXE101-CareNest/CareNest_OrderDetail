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

        public async Task<Order> HandleAsync(UpdateCommand command)
        {
            // Gọi validator để kiểm tra dữ liệu
            //Validate.ValidateUpdate(command);

            // Tìm để cập nhật
            Order? service = await _unitOfWork.GetRepository<Order>().GetByIdAsync(command.Id)
               ?? throw new BadRequestException("Id: " + MessageConstant.NotFound);

            service.Note = command.Note;
            service.Status = command.Status;
            service.CustomerId = command.CustomerId;
            service.PaymentMethod = command.PaymentMethod;
            service.ShipAddressId = command.ShipAddressId;
            service.TotalAmount = command.TotalAmount;
            service.Status = command.Status;
            service.UpdatedAt = TimeHelper.GetUtcNow();

            _unitOfWork.GetRepository<Order>().Update(service);
            await _unitOfWork.SaveAsync();
            return service;

        }
    }
}
