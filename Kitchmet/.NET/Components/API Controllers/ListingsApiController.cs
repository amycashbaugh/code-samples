using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sabio.Models;
using Sabio.Models.Domain;
using Sabio.Models.Requests.Listings;
using Sabio.Services;
using Sabio.Services.Interfaces;
using Sabio.Web.Controllers;
using Sabio.Web.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/listings")]
    [ApiController]
    public class ListingsApiController : BaseApiController
    {
        private IListingsService _service = null;
       
        private IAuthenticationService<int> _authService = null;

        public ListingsApiController(IListingsService service
            , ILogger<ListingsApiController> logger
            , IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }

        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(ListingDetailAddRequest model)
        {
            ObjectResult result = null;

            try
            {
                int userId = _authService.GetCurrentUserId();
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

        [HttpPut("{id:int}")]
        public ActionResult<SuccessResponse> Update(ListingDetailUpdateRequest model)
        {
            int intCode = 200;
            BaseResponse response = null;

            try
            {
                _service.Update(model);
                response = new SuccessResponse();
            }
            catch (Exception exp)
            {
                intCode = 500;
                base.Logger.LogError(exp.ToString());
                response = new ErrorResponse(exp.Message);
            }

            return StatusCode(intCode, response);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<ListingDetails>> SelectById(int id)
        {
            int intCode = 200;
            BaseResponse response = null;

            try
            {
                ListingDetails listingWizard = _service.SelectById(id);

                if (listingWizard == null)
                {
                    intCode = 404;
                    response = new ErrorResponse("Listing Page Not Found");
                }
                else
                {
                    response = new ItemResponse<ListingDetails> { Item = listingWizard };
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

        [HttpGet("current")]
        public ActionResult<ItemResponse<Paged<ListingDetails>>> SelectByCreatedBy(int pageIndex, int pageSize)
        {
            int intCode = 200;
            BaseResponse response = null;
            try
            {
                int createdBy = _authService.GetCurrentUserId();

                Paged<ListingDetails> paged = _service.SelectByCreatedBy(createdBy, pageIndex, pageSize);

                if (paged == null)
                {

                    intCode = 404;
                    response = new ErrorResponse("Listing Page Not Found");
                }
                else
                {
                    response = new ItemResponse<Paged<ListingDetails>> { Item = paged };

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
        [HttpGet("paginate")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<Paged<ListingDetails>>> SelectAll(int pageIndex, int pageSize)
        {
            int intCode = 200;
            BaseResponse response = null;
            try
            {
                Paged<ListingDetails> paged = _service.SelectAll(pageIndex, pageSize);

                if (paged == null)
                {

                    intCode = 404;
                    response = new ErrorResponse("File Page Not Found.");
                }
                else
                {
                    response = new ItemResponse<Paged<ListingDetails>> { Item = paged };

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

        [HttpGet("searchlocation")]
        [AllowAnonymous]
        public ActionResult<ItemResponse<Paged<ListingDetails>>> SearchByLocation(int pageIndex, int pageSize, double lat, double lng, int distance)
        {
            int intCode = 200;
            BaseResponse response = null;
            try
            {
                Paged<ListingDetails> paged = _service.SearchByLocation(pageIndex, pageSize, lat, lng, distance);

                if (paged == null)
                {
                    intCode = 404;
                    response = new ErrorResponse("File Page Not Found.");
                }
                else
                {
                    response = new ItemResponse<Paged<ListingDetails>> { Item = paged };

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

        [HttpDelete("statusupdate/{id:int}")]
        public ActionResult<SuccessResponse> Delete(int id)
        {
            int intCode = 200;
            BaseResponse response = null;

            try
            {
                _service.UpdateStatus(id);
                response = new SuccessResponse();
            }
            catch (Exception ex)
            {
                intCode = 500;
                response = new ErrorResponse(ex.Message);
            }

            return StatusCode(intCode, response);

        }

    }
}
