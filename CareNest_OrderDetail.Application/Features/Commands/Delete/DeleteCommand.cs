
using CareNest_OrderDetail.Application.Interfaces.CQRS.Commands;

namespace CareNest_OrderDetail.Application.Features.Commands.Delete
{
    public class DeleteCommand : ICommand
    {
        public required string Id { get; set; }
    }
}
