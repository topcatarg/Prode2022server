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

        [Route("UpSert")]
        [HttpPost]
        public async Task<ActionResult> UpSert(Country country)
        {
            if (country == null
                || country.Team == ""
                || country.Code == "")
            {
                return new BadRequestResult();
            }
            //validate country
            bool result = await dataadminservice.UpSert(country);
            
            return result?new OkResult() : new BadRequestResult();
        }

        [Route("Delete")]
        [HttpPost]
        public async Task<ActionResult> Delete(Country country)
        {
            if (country == null)
            {
                return new BadRequestResult();
            }
            bool result = await dataadminservice.Delete(country);
            return result?new OkResult() : new BadRequestResult();
        }
    }

}