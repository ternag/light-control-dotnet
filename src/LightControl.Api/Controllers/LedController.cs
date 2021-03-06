﻿using System;
using System.Collections.Generic;
using System.Linq;
using LightControl.Api.AppModel;
using LightControl.Api.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LightControl.Api.Controllers
{
  [ApiController]
  [Route("/api/[controller]")]
  public class LedController : ControllerBase
  {
    private readonly ILogger _logger;
    private readonly ILedContext _ledContext;
    private readonly IHardwareContext _hardwareContext;

    public LedController(ILedContext ledContext, ILogger<LedController> logger, IHardwareContext hardwareContext)
    {
      _logger = logger;
      _ledContext = ledContext;
      _hardwareContext = hardwareContext;
    }

    [HttpGet]
    public ActionResult<IEnumerable<LedDto>> Get()
    {
      return CatchExceptions(() => _ledContext.All.Select(l => l.ToDto()));
    }


    [HttpGet]
    [Route("{id}")]
    public ActionResult<LedDto> Get(ushort id)
    {
      _logger.LogInformation($"Getting LED {id}");
      return CatchExceptions(() => _ledContext.Get(id).ToDto());
    }

    [HttpPut]
    [Route("{id}")]
    public ActionResult<LedDto> Put(ushort id, [FromBody] LedUpdateDisplay newDisplayValue)
    {
      _logger.LogInformation($"Updating LED {id}, display={newDisplayValue.Display}");

      return CatchExceptions(() =>
      {
        Led knownLed = _ledContext.Get(id);
        knownLed.Display = newDisplayValue.Display;
        return knownLed.ToDto();
      });
    }

    [HttpGet]
    [Route("{id}/_flick")]
    public ActionResult<LedDto> Flick(ushort id)
    {
      _logger.LogInformation($"Flicking LED {id}");
      return CatchExceptions(() => FlickAndUpdate(id).ToDto());
    }

    private Led FlickAndUpdate(LedId id)
    {
      var led = _ledContext.Flick(id);
      _hardwareContext.Hal.Update(led);
      return led;
    }

    private ActionResult<T> CatchExceptions<T>(Func<T> method)
    {
      try
      {
        return method();
      }
      catch (ArgumentException ae)
      {
        _logger.LogError(ae, ae.Message);
        return NotFound(ae.Message);
      }
      catch (Exception e)
      {
        _logger.LogError(e, e.Message);
        return BadRequest(e.Message);
      }
    }
  }
}