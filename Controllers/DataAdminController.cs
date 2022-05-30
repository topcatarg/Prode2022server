using Microsoft.AspNetCore.Mvc;
using Prode2022Server.Services;
using System.Linq;
using System.Collections.Immutable;
using Prode2022Server.Models;
using Prode2022Server.Helpers;

namespace Prode2022Server.Controllers
{
    [Route("DataAdmin")]
    [ApiController]
    public class DataAdminController : Controller
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
                || country.Team.IsNullOrEmpty()
                || country.Code.IsNullOrEmpty()
            )
            {
                return new BadRequestResult();
            }
            //validate country
            bool result = await dataadminservice.UpSert(country);

            return result ? new OkResult() : new BadRequestResult();
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
            return result ? new OkResult() : new BadRequestResult();
        }

        [Route("GetAllFixtureGroups")]
        [HttpGet]
        public async Task<ActionResult<List<FixtureGroups>>> GetAllFixtureGroups()
        {
            return await dataadminservice.GetAllFixtureGroups();
        }

        [Route("UpSertFixtureGroups")]
        [HttpPost]
        public async Task<ActionResult> UpSertFixtureGroups(FixtureGroups fixtureGroups)
        {
            if (fixtureGroups == null
                || fixtureGroups.GroupName.IsNullOrEmpty())
            {
                return new BadRequestResult();
            }
            //validate country
            bool result = await dataadminservice.UpSert(fixtureGroups);

            return result ? new OkResult() : new BadRequestResult();
        }

        [Route("DeleteFixtureGroups")]
        [HttpPost]
        public async Task<ActionResult> Delete(FixtureGroups fixtureGroups)
        {
            if (fixtureGroups == null)
            {
                return new BadRequestResult();
            }
            bool result = await dataadminservice.Delete(fixtureGroups);
            return result ? new OkResult() : new BadRequestResult();
        }

        [Route("GetAllMatchs")]
        [HttpGet]
        public async Task<ActionResult<List<FixtureMatch>>> GetAllMatchs()
        {
            return await dataadminservice.GetAllMatchs();
        }

        [Route("GetAllFixtureMatchs")]
        [HttpGet]
        public async Task<ActionResult<List<FixtureMatch>>> GetAllFixtureMatchs()
        {
            return await dataadminservice.GetAllFixtureMatchs();
        }

        [Route("DeleteFixtureMatch")]
        [HttpPost]
        public async Task<ActionResult> Delete(FixtureMatch fixtureMatch)
        {
            if (fixtureMatch == null)
            {
                return new BadRequestResult();
            }
            bool result = await dataadminservice.Delete(fixtureMatch);
            return result ? new OkResult() : new BadRequestResult();
        }
    
        [Route("UpSertFixtureMatch")]
        [HttpPost]
        public async Task<ActionResult> Upsert(FixtureMatch fixtureMatch)
        {
            if (fixtureMatch == null
                || fixtureMatch.Date.IsNullOrEmpty()
                || fixtureMatch.Time.IsNullOrEmpty()
                || fixtureMatch.Team1.IsNullOrZero()
                || fixtureMatch.Team2.IsNullOrZero()
                || fixtureMatch.Stage.IsNullOrZero()
                )
            {
                return new BadRequestResult();
            }
            bool result = await dataadminservice.UpSert(fixtureMatch);

            return result ? new OkResult() : new BadRequestResult();
        }
    
        [Route("GetMatchResults")]
        [HttpGet]
        public async Task<ActionResult<List<MatchResult>>> GetMatchResults()
        {
            return await dataadminservice.GetMatchResultsAsync();
        }

        [Route("StoreMatchResult")]
        [HttpPost]
        public async Task<ActionResult<bool>> StoreMatchResult(MatchResult matchResult)
        {
            return await dataadminservice.StoreMatchResult(matchResult);
        }
    }


}