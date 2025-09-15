using CareNest_Order.Application.Interfaces.CQRS.Commands;
using CareNest_Order.Application.Interfaces.UOW;
using CareNest_OrderDetail.Application.Features.Commands.Create;
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

        public async Task<Order> HandleAsync(CreateCommand command)
        {
            //Validate.ValidateCreate(command);

            Order service = new()
            {
                Status = command.Status,
                CustomerId = command.CustomerId,
                Note = command.Note,
                PaymentMethod = command.PaymentMethod,
                ShipAddressId = command.ShipAddressId,
                TotalAmount = command.TotalAmount,
                ShopId = command.ShopId,
                CreatedAt = TimeHelper.GetUtcNow()
            };
            await _unitOfWork.GetRepository<Order>().AddAsync(service);
            await _unitOfWork.SaveAsync();

            return service;
        }
    }
}
