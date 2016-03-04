using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThermoNuclearWar.Models
{
    public class LaunchApiStatusResult
    {
        public Code Status { get; set; }

        public enum Code
        {
            Online,
            Offline
        }
    }
}