using System;
using System.Collections.Generic;
using System.Linq;
using LightControl.Api.Models;

namespace LightControl.Api.Infrastructure
{
  public interface ILedContext {
    IEnumerable<Led> All { get; }
    Led Get(int ledId);
    Led Flick(int id);
  }
}
