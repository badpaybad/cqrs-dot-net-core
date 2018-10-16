using IotHub.Core.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace IotHub.Core.Authorize
{
    public class IotHubAuthorizeAttribute : Attribute, IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            //todo: no working?
            throw new NotImplementedException();
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            //todo: no working?
            throw new NotImplementedException();
        }
    }
    //public class IotHubAuthorizeAttribute : TypeFilterAttribute//Attribute, IAuthorizationFilter
    //{
    //    //public void OnAuthorization(AuthorizationFilterContext context)
    //    //{
    //    //    context.Result = new JsonResult(new BaseResponse() {
    //    //    Message="Unauthorize",
    //    //    StatusCode= System.Net.HttpStatusCode.Unauthorized,
    //    //    Success=false});
    //    //}
    //    public IotHubAuthorizeAttribute() : base(typeof(IotHubAuthorizeAttributeImpl))
    //    {
    //    }

    //    private class IotHubAuthorizeAttributeImpl : Attribute, IAuthorizationFilter
    //    {
    //        public void OnAuthorization(AuthorizationFilterContext context)
    //        {
    //            context.Result = new JsonResult(new BaseResponse()
    //            {
    //                Message = "Unauthorize",
    //                StatusCode = System.Net.HttpStatusCode.Unauthorized,
    //                Success = false
    //            });
    //        }
    //    }
    //}
}
