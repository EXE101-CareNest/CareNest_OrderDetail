
namespace CareNest_OrderDetail.Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Role { get; }
    }
}
