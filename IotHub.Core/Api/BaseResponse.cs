using System.Net;

namespace IotHub.Core.Api
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; } = System.Net.HttpStatusCode.Unauthorized;

        //public Microsoft.AspNetCore.Mvc.JsonResult ToJsonResult()
        //{
        //    return new Microsoft.AspNetCore.Mvc.JsonResult(this);
        //}
    }
}
