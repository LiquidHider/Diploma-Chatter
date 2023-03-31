using AutoMapper;
using Chatter.Domain.BusinessLogic.Enums;
using Chatter.Domain.BusinessLogic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chatter.Domain.API.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public abstract class BaseAPIController : ControllerBase
    {
        protected readonly IMapper _mapper;

        public BaseAPIController(IMapper mapper)
        {
            _mapper = mapper;
        }

        protected IActionResult MapErrorResponse(ServiceResult result)
        {
            return result.Error.Type switch
            {
                ErrorType.BusinessError => BadRequest(result.Error),
                ErrorType.NoDataError => NotFound(result.Error),
                ErrorType.DataError => UnprocessableEntity(result.Error),
                ErrorType.DataConflict => Conflict(result.Error),
                _ => StatusCode(StatusCodes.Status500InternalServerError, result.Error)
            };
        }
    }
}
