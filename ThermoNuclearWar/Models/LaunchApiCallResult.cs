using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThermoNuclearWar.Models
{
    public class LaunchApiCallResult
    {
        public Code Result { get; set; }

        public string Message { get; set; }

        public enum Code
        {
            Success,
            Failure
        }
    }
}