using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;

namespace WarehouseManagementSystem.Controllers
{
    public class JobsController : Controller
    {
        // GET: Jobs
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetJobs()
        {
            List<Models.Job> jobs = await Models.Job.GetJobs();
            return Json(jobs, JsonRequestBehavior.AllowGet);
        }
    }
}