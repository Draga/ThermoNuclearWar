using System;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ThermoNuclearWar.Models;

namespace ThermoNuclearWar.Controllers
{
    public class LaunchesController : ApiController
    {
        private ThermoNuclearWarContext db = new ThermoNuclearWarContext();

        private readonly HttpClient HttpClient = new HttpClient();

        private string ThermoNuclearWarAccessPoint => "http://gitland.azurewebsites.net:80";

        private string LaunchCallPath => "/api/warheads/launch";

        /// <summary>
        ///  Returns a copy of the original launch code cooldown timespan to avoid accidental cahnges to it. This is thermo-nuclear war afterall!
        /// </summary>
        private TimeSpan LaunchCodeCooldown => new TimeSpan(0, 5, 0);

        // GET: api/Launches
        public IQueryable<Launch> GetLaunches()
        {
            return db.Launches;
        }

        // GET: api/Launches/5
        [ResponseType(typeof(Launch))]
        public async Task<IHttpActionResult> GetLaunch(int id)
        {
            Launch launch = await db.Launches.FindAsync(id);
            if (launch == null)
            {
                return NotFound();
            }

            return Ok(launch);
        }

        // POST: api/Launches
        [ResponseType(typeof(Launch))]
        public async Task<IHttpActionResult> PostLaunch(LaunchCode launchCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CheckLaunchCooldown(launchCode);

            var launchApiCall = new LaunchApiCallRequest { LaunchCode = launchCode.Code };
            var response = await HttpClient.PostAsJsonAsync(this.ThermoNuclearWarAccessPoint + this.LaunchCallPath, launchApiCall);
            if (response.IsSuccessStatusCode)
            {
                var launch = new Launch
                {
                    LaunchCode = launchCode,
                    DateTime = DateTime.Now
                };
                db.Launches.Add(launch);
                await db.SaveChangesAsync();
            }
            else
            {
                
            }



            return CreatedAtRoute("DefaultApi", new { id = launch.Id }, launch);
        }

        /// <summary>
        /// Make sure the launch code has not been used recently. 
        /// </summary>
        /// <param name="launchCode">The Launch Code to be used for this Launch.</param>
        private void CheckLaunchCooldown(LaunchCode launchCode)
        {
            var lastLaunchCodeLaunch = db.Launches
                .Where(l => l.LaunchCode.Id == launchCode.Id)
                .OrderByDescending(l => l.DateTime)
                .FirstOrDefault();
            if (lastLaunchCodeLaunch != null)
            {
                var now = DateTime.Now;
                var timeBetweenLaunches = now.Subtract(lastLaunchCodeLaunch.DateTime);
                if (timeBetweenLaunches < this.LaunchCodeCooldown)
                {
                    var message = string.Format(
                        "Launch code requires a cooldown of {0}.{1}The last use of this launch code was at {2} ({3} ago).{1}Please wait {4} ({5})",
                        this.LaunchCodeCooldown,
                        Environment.NewLine,
                        lastLaunchCodeLaunch.DateTime,
                        timeBetweenLaunches,
                        this.LaunchCodeCooldown.Subtract(timeBetweenLaunches),
                        lastLaunchCodeLaunch.DateTime.Add(this.LaunchCodeCooldown));
                    var httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.Forbidden, message);
                    throw new HttpResponseException(httpResponseMessage);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}