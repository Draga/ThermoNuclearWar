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

        private static string ThermoNuclearWarAccessPoint => "http://gitland.azurewebsites.net:80";

        private static string LaunchCallPath => "/api/warheads/launch";
        private static string StatusCallPath => "/api/warheads/status";

        /// <summary>
        ///  Returns a copy of the original launch code cooldown timespan to avoid accidental cahnges to it. This is thermo-nuclear war afterall!
        /// </summary>
        private TimeSpan LaunchCodeCooldown => new TimeSpan(0, 5, 0);

        // GET: api/Launches
        public async Task<bool> GetLaunchApiStatus()
        {
            var response = await HttpClient.GetAsync(ThermoNuclearWarAccessPoint + StatusCallPath);

            // API call unsuccesful.
            if (!response.IsSuccessStatusCode)
            {
                var message = "Unable to request status";
                var httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, message);
                throw new HttpResponseException(httpResponseMessage);
            }


            var launchApiStatusResult = await response.Content.ReadAsAsync<LaunchApiStatusResult>();

            return launchApiStatusResult.Status == LaunchApiStatusResult.Code.Online;

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
        public async Task<IHttpActionResult> PostLaunch(int launchCodeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: avoid multiple istances to create launches while they are being checked.
            // This should not be a problem at the moment since the application runs only on the president computer
            // and there is a client side validation.

            var launchCode = db.LaunchCodes.Find(launchCodeId);

            CheckLaunchCooldown(launchCode);

            var launch = await Launch(launchCode);
            return Ok(launch);
        }

        /// <summary>
        /// Asks the thermo-nuclear REST API to make a launch. Can throw various HTTP errors if unsuccesful.
        /// </summary>
        /// <param name="launchCode">The launch code to be used.</param>
        /// <returns>The Launch if successful.</returns>
        private async Task<Launch> Launch(LaunchCode launchCode)
        {
            // The API requires the code in the format YYMMDD-AAAAAAAAAA (where YYMMDD is 6 digits
            // representing the current date and AAAAAAAAAA is the pass phrase given to the President)
            var apiLaunchCode = DateTime.Now.ToString("yyMMdd") + "-" + launchCode.Code;
            
            var response =
                await HttpClient.PostAsJsonAsync(ThermoNuclearWarAccessPoint + LaunchCallPath + "?LaunchCode=" + apiLaunchCode, apiLaunchCode );

            // API call unsuccesful.
            if (!response.IsSuccessStatusCode)
            {
                var message = "Unable to make a launch request";
                var httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, message);
                throw new HttpResponseException(httpResponseMessage);
            }

            var launchApiCallResult = await response.Content.ReadAsAsync<LaunchApiCallResult>();

            // API call return a non succesful code.
            if (launchApiCallResult.Result != LaunchApiCallResult.Code.Success)
            {
                var message = string.Format(
                    "Launch request returned an error:{0}{1}", Environment.NewLine,
                    launchApiCallResult.Message);
                var httpResponseMessage = Request.CreateErrorResponse(HttpStatusCode.ServiceUnavailable, message);
                throw new HttpResponseException(httpResponseMessage);
            }

            // Launch was successful.
            var launch = new Launch
            {
                LaunchCode = launchCode,
                DateTime = DateTime.Now
            };
            db.Launches.Add(launch);
            await db.SaveChangesAsync();

            return launch;
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
                        "Launch code requires a cooldown of {0}.{1}" +
                        "The last use of this launch code was at {2} ({3} ago).{1}" +
                        "Please wait {4} ({5})",
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