using Finance_Manager_BAL;
using Finance_Manager_MAL.DTO;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Finance_Manager_API.Filters
{

    /// <summary>
    /// Attribute to define required permission
    /// </summary>
    public class PermissionAttribute : Attribute, IAsyncActionFilter
    {
        private readonly string _permission;

        public PermissionAttribute(string permission)
        {
            _permission = permission;
        }

        public async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var permissionService = context.HttpContext
                .RequestServices
                .GetService<PermissionService>();

            var result = await permissionService.ValidateAsync(
                context.HttpContext,
                _permission
            );

            if (!result)
            {
                context.Result = new Microsoft.AspNetCore.Mvc.JsonResult(
                    new ApiResponse<string>
                    {
                        Success = false,
                        Message = "Access Denied"
                    })
                { StatusCode = 403 };

                return;
            }

            await next();
        }
    }
}
