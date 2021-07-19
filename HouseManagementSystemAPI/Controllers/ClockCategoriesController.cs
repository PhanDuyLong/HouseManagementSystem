using HMS.Data.Responses;
using HMS.Data.Services;
using HMS.Data.Utilities;
using HMS.Data.ViewModels.Clock;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace HMSAPI.Controllers
{
    /// <summary>
    /// ClockCategoriesController
    /// </summary>
    [Route("api/clockCategories")]
    [ApiController]
    public class ClockCategoriesController : ControllerBase
    {
        private IClockCategoryService _clockCategoryService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="clockCategoryService"></param>
        public ClockCategoriesController(IClockCategoryService clockCategoryService)
        {
            _clockCategoryService = clockCategoryService;
        }

        /// <summary>
        /// Get All Categories
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ClockDetailViewModel>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ResultResponse), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetClockCategoriesAsync()
        {
            var categories = await _clockCategoryService.GetClockCategoriesAsync();
            if (categories == null || categories.Count == 0)
                return NotFound(new MessageResult("NF01", new string[] { "ClockCategories" }).Value);
            return Ok(categories);
        }
    }
}
