using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests;
using Sabio.Services;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using File = Sabio.Models.Domain.File;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesApiController : BaseApiController
    {
        private IFilesService _service = null;
        private IAuthenticationService<int> _authService = null;

        public FilesApiController(IFilesService service, ILogger<FilesApiController> logger, IAuthenticationService<int> authService) : base(logger)
        {
            _authService = authService;
            _service = service;

        }

        [HttpPost("upload")]
        public ActionResult<ItemsResponse<File>> UploadFiles(List<IFormFile> files)
        {
            ObjectResult result = null;

            if(files != null)
            {
                try
                {
                    int userId = _authService.GetCurrentUserId();
                    List<File> urls = _service.UploadMutliple(files, userId);
                    ItemsResponse<File> response = new ItemsResponse<File>() { Items = urls };
                    result = StatusCode(201, response);
                }
                catch (Exception exp)
                {
                    Logger.LogError(exp.ToString());
                    ErrorResponse response = new ErrorResponse(exp.Message);
                    result = StatusCode(500, response);
                }
            }

            return result;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(FileAddRequest model)
        {
            ObjectResult result = null;

            try
            {   int userId = _authService.GetCurrentUserId();
                int id = _service.Create(model, userId);

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

        [HttpGet("paginate")]
        public ActionResult<ItemResponse<Paged<File>>> SelectAll(int pageIndex, int pageSize)
        {
            int intCode = 200;
            BaseResponse response = null;
            try
            {
                Paged<File> paged = _service.SelectAll(pageIndex, pageSize);

                if (paged == null)
                {

                    intCode = 404;
                    response = new ErrorResponse("File Page Not Found.");
                }
                else
                {
                    response = new ItemResponse<Paged<File>> { Item = paged };
                 
                }
            }

            catch (Exception exp)
            {
                intCode = 500;
                base.Logger.LogError(exp.ToString());
                response = new ErrorResponse($"Generic Error:{ exp.Message }");
            }

            return StatusCode(intCode, response);
        }

        [HttpGet]
        public ActionResult<ItemResponse<Paged<File>>> SelectByCreatedBy(int pageIndex, int pageSize)
        {
            int intCode = 200;
            BaseResponse response = null;
            try
            {
                int createdBy = _authService.GetCurrentUserId();

                Paged<File> paged = _service.SelectByCreatedBy(createdBy, pageIndex, pageSize);

                if (paged == null)
                {

                    intCode = 404;
                    response = new ErrorResponse("File Page Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<File>> { Item = paged };
                  
                }
            }

            catch (Exception exp)
            {
                intCode = 500;
                base.Logger.LogError(exp.ToString());
                response = new ErrorResponse($"Generic Error:{ exp.Message }");
            }

            return StatusCode(intCode, response);
        }

        [HttpGet("{id:int}")]
        public ActionResult<ItemResponse<File>> SelectById(int id)
        {
            int intCode = 200;
            BaseResponse response = null;

            try
            {
                File singleFile = _service.SelectById(id);

                if (singleFile == null)
                {
                    intCode = 404;
                    response = new ErrorResponse("File Page Not Found");
                }
                else
                {
                    response = new ItemResponse<File> { Item = singleFile };
                }
            }
            catch (Exception exp)
            {
                intCode = 500;
                base.Logger.LogError(exp.ToString());
                response = new ErrorResponse($"Generic Error:{ exp.Message }");
            }

            return StatusCode(intCode, response);
        }
    }
}
