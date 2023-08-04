namespace HardwareShop.Core.Services
{
    public interface ICurrentUserService
    {
        bool IsSystemAdmin();
        Guid GetUserGuid();
    }
}
