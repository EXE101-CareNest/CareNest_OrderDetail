using CareNest_OrderDetail.API.Extensions;
using CareNest_OrderDetail.Application.Common;
using CareNest_OrderDetail.Application.Features.Commands.Create;
using CareNest_OrderDetail.Application.Features.Commands.Delete;
using CareNest_OrderDetail.Application.Features.Commands.Update;
using CareNest_OrderDetail.Application.Features.Queries.GetAllPaging;
using CareNest_OrderDetail.Application.Features.Queries.GetById;
using CareNest_OrderDetail.Application.Interfaces.CQRS;
using CareNest_OrderDetail.Domain.Commons.Constant;
using CareNest_OrderDetail.Domain.Entitites;
using Microsoft.AspNetCore.Mvc;

namespace CareNest_OrderDetail.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IUseCaseDispatcher _dispatcher;
        public OrderDetailController(IUseCaseDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }
        /// <summary>
        /// Hiển thị toàn bộ danh sách chi tiết đơn hàng hiện có trong hệ thống với phân trang và sắp xếp
        /// </summary>
        /// <param name="pageIndex">trang hiện tại</param>
        /// <param name="pageSize">Số lượng phần tử trong trang</param>
        /// <param name="sortColumn">cột muốn sort: name, updateat,ownerid</param>
        /// <param name="sortDirection">cách sort asc or desc</param>
        /// <returns>Danh sách chi tiết đơn hàng</returns>
        [HttpGet]
        public async Task<IActionResult> GetPaging(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortColumn = null,
            [FromQuery] string? sortDirection = "asc")
        {
            var query = new GetAllPagingQuery()
            {
                Index = pageIndex,
                PageSize = pageSize,
                SortColumn = sortColumn,
                SortDirection = sortDirection
            };
            var result = await _dispatcher.DispatchQueryAsync<GetAllPagingQuery, PageResult<OrderDetailResponse>>(query);
            return this.OkResponse(result, MessageConstant.SuccessGet);
        }

        /// <summary>
        /// Hiển thị chi tiết chi tiết chi tiết đơn hàng theo id
        /// </summary>
        /// <param name="id">Id chi tiết đơn hàng</param>
        /// <returns>chi tiết chi tiết đơn hàng</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var query = new GetByIdQuery() { Id = id };
            OrderDetail result = await _dispatcher.DispatchQueryAsync<GetByIdQuery, OrderDetail>(query);
            return this.OkResponse(result, MessageConstant.SuccessGet);
        }

        /// <summary>
        /// tạo mới chi tiết chi tiết đơn hàng
        /// </summary>
        /// <param name="command">thông tin chi tiết đơn hàng</param>
        /// <returns>thông tin chi tiết đơn hàng mới tạo xog</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateCommand command)
        {
            OrderDetail result = await _dispatcher.DispatchAsync<CreateCommand, OrderDetail>(command);

            return this.OkResponse(result, MessageConstant.SuccessCreate);
        }

        /// <summary>
        /// Cập nhật thông tin chi tiết đơn hàng
        /// </summary>
        /// <param name="id">Id chi tiết đơn hàng</param>
        /// <param name="request">các thông tin cần sửa</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateRequest request)
        {

            var command = new UpdateCommand()
            {
                Id = id,
                OrderId = request.OrderId,
                TotalAmount = request.TotalAmount,
                ProductDetailId = request.ProductDetailId,
                Quantity = request.Quantity
            };
            OrderDetail result = await _dispatcher.DispatchAsync<UpdateCommand, OrderDetail>(command);

            return this.OkResponse(result, MessageConstant.SuccessUpdate);
        }

        /// <summary>
        /// xoá chi tiết đơn hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await _dispatcher.DispatchAsync(new DeleteCommand { Id = id });
            return this.OkResponse(MessageConstant.SuccessDelete);
        }
    }
}
