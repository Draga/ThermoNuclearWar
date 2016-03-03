using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ThermoNuclearWar.Models
{
    public class Launch
    {
        [Key]
        public int Id { get; set; }

        public LaunchCode LaunchCode { get; set; }

        public DateTime DateTime { get; set; }
    }
}