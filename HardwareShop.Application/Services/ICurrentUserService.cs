namespace HardwareShop.Application.Services
{
    public interface ICurrentUserService
    {
        bool IsSystemAdmin();
        Guid GetUserGuid();
    }
}
