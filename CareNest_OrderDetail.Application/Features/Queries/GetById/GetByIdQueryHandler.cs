using CareNest_OrderDetail.Application.Interfaces.CQRS.Queries;
using CareNest_OrderDetail.Application.Interfaces.UOW;
using CareNest_OrderDetail.Domain.Commons.Constant;
using CareNest_OrderDetail.Domain.Entitites;

namespace CareNest_OrderDetail.Application.Features.Queries.GetById
{
    public class GetByIdQueryHandler : IQueryHandler<GetByIdQuery, OrderDetail>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OrderDetail> HandleAsync(GetByIdQuery query)
        {
            OrderDetail? service = await _unitOfWork.GetRepository<OrderDetail>().GetByIdAsync(query.Id);

            if (service == null)
            {
                throw new Exception(MessageConstant.NotFound);
            }
            return service;
        }
    }
}
