﻿using System.Threading.Tasks;
using FlakyApi.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FlakyApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CircuitBreakerController : ControllerBase
    {
        private readonly ILogger<CircuitBreakerController> _logger;
        private readonly IService _service;

        public CircuitBreakerController(ILogger<CircuitBreakerController> logger, IService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> Status()
        {
            try
            {
                var result = await _service.DoSomething();
                return Ok(result);
            }
            catch (ServiceCurrentlyUnavailableException e)
            {
                _logger.LogError(e, "service is not available");
                return BadRequest(new
                {
                    TimeStep = e.RelativeTimeStep,
                    Message = nameof(ServiceCurrentlyUnavailableException)
                });
            }
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [Produces("application/json")]
        public IActionResult Settings([FromServices] IOptions<FlakyStrategyOptions> options)
        {
            return Ok(options.Value);
        }

        [HttpGet]
        [Route("[action]")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status202Accepted)]
        [Produces("application/json")]
        public async Task<IActionResult> Reset([FromServices] IFlakyStrategy flakyStrategy)
        {
            await flakyStrategy.Reset();
            return StatusCode(StatusCodes.Status202Accepted);
        }
    }
}
