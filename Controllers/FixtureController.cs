using Microsoft.AspNetCore.Mvc;
using Prode2022Server.Services;
using System.Linq;
using System.Collections.Immutable;
using Prode2022Server.Models;

namespace Prode2022Server.Controllers
{
    [Route("Fixture")]
    [ApiController]
    public class FixtureController: Controller
    {
        private readonly FixtureService _FixtureService;

        public FixtureController(FixtureService _fixtureservice)
        {
            _FixtureService = _fixtureservice;
        }

        [HttpGet]
        public async Task<ActionResult<ImmutableArray<Matchs>>> GetMatches()
        {
            return await _FixtureService.GetAllMatchs();
        }
    }
}