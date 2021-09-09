using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sabio.Models;
using Sabio.Models.AppSettings;
using Sabio.Models.Domain;
using Sabio.Models.Requests;
using Sabio.Models.Requests.Addresses;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Core;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UsersApiController : BaseApiController
    {
        private IUsersService _service = null;
        private IAuthenticationService<int> _authService = null;
        IOptions<SecurityConfig> _options;
        private AppKeys _appKeys;
        EmailService _emailService = new EmailService();

        public UsersApiController(IOptions<AppKeys> appKeys
            , IUsersService service
            , ILogger<UsersApiController> logger
            , IAuthenticationService<int> authService
            , IOptions<SecurityConfig> options) : base(logger)
        {
            _authService = authService;
            _service = service;
            _options = options;
            _appKeys = appKeys.Value;
        }

        [HttpGet]
        public ActionResult<ItemsResponse<User>> GetAll()
        {
            int intCode = 200;
            BaseResponse response = null; 

            try
            {
                List<User> list = _service.GetAllUsers();

                if (list == null)
                {
                    intCode = 404;
                    response = new ErrorResponse("App Resource Not Found");
                }
                else
                {
                    response = new ItemsResponse<User> { Items = list };
                }
            }

            catch (Exception exp)
            {
                intCode = 500;
                response = new ErrorResponse(exp.Message);
                base.Logger.LogError(exp.ToString());
            }

            return StatusCode(intCode, response);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<User>> GetById(int id)
        {
            int intCode = 200;
            BaseResponse response = null;

            try
            {
                User user = _service.GetById(id);

                if (user == null)
                {
                    intCode = 404;
                    response = new ErrorResponse("Application Resource Not Found.");
                }
                else
                {
                    response = new ItemResponse<User> { Item = user };
                }
            }
            catch (SqlException sqlExp)
            {
                intCode = 500;
                response = new ErrorResponse($"SqlException Error:{ sqlExp.Message }");
                base.Logger.LogError(sqlExp.ToString());
            }
            catch (ArgumentException argExp)
            {
                intCode = 500;
                response = new ErrorResponse($"ArgumentException Error:{ argExp.Message }");
            }
            catch (Exception exp)
            {
                intCode = 500;
                base.Logger.LogError(exp.ToString());
                response = new ErrorResponse($"Generic Error:{ exp.Message }");
            }

            return StatusCode(intCode, response);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int intCode = 200;
            BaseResponse response = null;

            try
            {
                _service.Delete(id);

                response = new SuccessResponse();
            }
            catch (Exception exp)
            {
                intCode = 500;
                response = new ErrorResponse(exp.Message);
            }

            return StatusCode(intCode, response);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<int>> Create(UserAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                int id = _service.Create(model);
                string token = Guid.NewGuid().ToString();

                _service.InsertToken(token, id);

                _emailService.VerifyEmail(_appKeys.SendGridAppKey, model.Email, model.FirstName, token).Wait();

                ItemResponse<int> response = new ItemResponse<int>() { Item = id };
                result = Created201(response);
            }
            catch (Exception exp)
            {
                Logger.LogError(exp.ToString());
                ErrorResponse response = new ErrorResponse(exp.Message);
                result = StatusCode(500, response);
            }

            return result;
        }

        [HttpPost("confirmation/{token}")]
        public ActionResult<SuccessResponse> ConfirmEmail([FromRoute] string token)
        {
            ObjectResult result = null;
            BaseResponse response = null;
            try
            {
                _service.VerifyEmail(token);
                response = new SuccessResponse();
                result = Ok200(response);
            }
            catch (Exception ex)
            {
                response = new ErrorResponse("Bad Request");
                result = StatusCode(400, response);
            }

            return result;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ItemResponse<int>>> LoginAsync(UserLoginRequest model)
        {
            ObjectResult result = null;
            BaseResponse response = null;

            if (model != null)
            {
                try
                {
                    bool isSuccessful = await _service.LogInAsync(model.Email, model.Password);
                    if (isSuccessful == false)
                    {
                        response = new ErrorResponse("Unauthorized");
                        result = StatusCode(401, response);
                    }
                    else
                    {
                        response = new SuccessResponse();
                        result = Ok200(response);
                    }

                }
                catch (Exception exp)
                {
                    Logger.LogError(exp.ToString());
                    response = new ErrorResponse(exp.Message);
                    result = StatusCode(500, response);
                }
            }
            else
            {
                response = new ErrorResponse("Bad Request");
                result = StatusCode(400, response);
            }

            return result;
        }

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(UserUpdateRequest model)
        {
            int code = 200;
            BaseResponse response = null;

            try
            {
                _service.Update(model);

                response = new SuccessResponse();
            }
            catch (Exception exp)
            {
                code = 500;
                response = new ErrorResponse(exp.Message);
            }

            return StatusCode(code, response);
        }

        [HttpGet("current")]
        public ActionResult<ItemResponse<IUserAuthData>> GetCurrrent()
        {
            IUserAuthData user = _authService.GetCurrentUser();
            ItemResponse<IUserAuthData> response = new ItemResponse<IUserAuthData>();
            response.Item = user;

            return Ok200(response);
        }
    }
}
