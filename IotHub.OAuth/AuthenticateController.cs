using IotHub.Core.Api;
using Microsoft.AspNetCore.Mvc;

namespace IotHub.OAuth
{
    [Route("api/Authenticate")]
    [ApiController]
    public class AuthenticateController : JsonControllerBase
    {
        [HttpGet]
        public bool Valid(string tokenSession)
        {
            return AuthenticateServices.IsValidToken(tokenSession);
        }

        [HttpPost]
        public LoginResponse Post(LoginRequest request)
        {
            string tokenSession;

            var u = AuthenticateServices.Login(request.Email, request.Password, out tokenSession);
          
            if (u == null)
            {
                return new LoginResponse()
                {
                    Message = "Invalid email or password",
                    StatusCode = System.Net.HttpStatusCode.Unauthorized
                };
            }

            return new LoginResponse()
            {
                ExpiredInMinutes = AuthenticateServices.EpireAfterMinutes,
                Message = "Success",
                StatusCode = System.Net.HttpStatusCode.OK,
                Success = true,
                TokenSession = tokenSession
            };
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse : BaseResponse
    {
        public string TokenSession { get; set; }
        public int ExpiredInMinutes { get; set; }

    }
    

}
