using System.Collections.Generic;
using System.Data.Entity;

namespace ThermoNuclearWar.Models
{
    public class LaunchCodesInitializer : DropCreateDatabaseAlways<ThermoNuclearWarContext>
    {
        protected override void Seed(ThermoNuclearWarContext context)
        {
            IList<LaunchCode> launchCodes = new List<LaunchCode>();

            launchCodes.Add(new LaunchCode() { Code = "NICEGAMEOFCHESS" });

            context.LaunchCodes.AddRange(launchCodes);

            base.Seed(context);
        }
    }
}