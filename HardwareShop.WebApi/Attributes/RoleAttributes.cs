
using Microsoft.AspNetCore.Authorization;

namespace HardwareShop.WebApi.Attributes;

public class AdminRoleAuthorizeAttribute : AuthorizeAttribute
{
    public AdminRoleAuthorizeAttribute() : base()
    {
        Roles = "admin";
    }
}