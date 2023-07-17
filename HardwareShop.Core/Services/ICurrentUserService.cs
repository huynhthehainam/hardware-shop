namespace HardwareShop.Core.Services
{
    public interface ICurrentUserService
    {
        bool IsSystemAdmin();
        int GetUserId();
        Guid GetUserGuid();
    }
}
