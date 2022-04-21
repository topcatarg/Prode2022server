using Microsoft.AspNetCore.Mvc;
using Prode2022Server.Services;
using System.Linq;
using System.Collections.Immutable;
using Prode2022Server.Models;

namespace Prode2022Server.Controllers
{
    [Route("DataAdmin")]
    [ApiController]
    public class DataAdminController: Controller
    {

        DataAdminServices dataadminservice;

        public DataAdminController(DataAdminServices das)
        {
            dataadminservice = das;
        }

        [Route("AllCountries")]
        [HttpGet]
        public async Task<ActionResult<List<Country>>> GetAllCountries()
        {
            return await dataadminservice.GetAllCountries();
        }

        [Route("AddCountry")]
        [HttpPost]
        public async Task<ActionResult> AddCountry(Country country)
        {
            //validate country
            bool result = await dataadminservice.AddCountry(country);
            
            return result?new OkResult() : new BadRequestResult();
        }
    }

}