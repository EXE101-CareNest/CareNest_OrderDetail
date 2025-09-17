using CareNest_OrderDetail.Application.Features.Commands.Create;
using CareNest_OrderDetail.Application.Features.Commands.Update;
using CareNest_OrderDetail.Domain.Commons.Constant;

namespace CareNest_OrderDetail.Application.Exceptions.Validators
{
    public class Validate
    {
        /// <summary>
        /// kiểm tra toàn bộ tạo đơn hàng
        /// </summary>
        /// <param name="command"></param>
        public static void ValidateCreate(CreateCommand command)
        {
            ValidateQuantity(command.Quantity);
        }
        /// <summary>
        /// kiểm tra cập nhật đơn hàng
        /// </summary>
        /// <param name="command"></param>
        public static void ValidateUpdate(UpdateCommand command)
        {
            ValidateQuantity(command.Quantity);
        }
        /// <summary>
        /// Valiđ tên đơn hàng
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="BadRequestException"></exception>
        public static void ValidateQuantity(int? quantity)
        {
            if (quantity < 0)
            {
                throw new BadRequestException(MessageConstant.InvalidNumber);
            }
        }

    }
}
