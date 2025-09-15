using CareNest_OrderDetail.Application.Exceptions;
using CareNest_OrderDetail.Application.Interfaces.CQRS.Commands;
using CareNest_OrderDetail.Application.Interfaces.UOW;
using CareNest_OrderDetail.Domain.Commons.Constant;
using CareNest_OrderDetail.Domain.Entitites;

namespace CareNest_OrderDetail.Application.Features.Commands.Delete
{
    public class DeleteCommandHandler : ICommandHandler<DeleteCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task HandleAsync(DeleteCommand command)
        {
            // Lấy shop theo ID
            Order? shop = await _unitOfWork.GetRepository<Order>().GetByIdAsync(command.Id)
                                              ?? throw new BadRequestException("Id: " + MessageConstant.NotFound);

            _unitOfWork.GetRepository<Order>().Delete(shop);

            await _unitOfWork.SaveAsync();

        }
    }
}
