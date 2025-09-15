using CareNest_OrderDetail.Application.Interfaces.CQRS.Queries;
using CareNest_OrderDetail.Domain.Entitites;

namespace CareNest_OrderDetail.Application.Features.Queries.GetById
{
    public class GetByIdQuery : IQuery<Order>
    {
        public required string Id { get; set; }
    }
}
