using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WarehouseManagementSystem.Controllers
{
    public class ShiftsController : Controller
    {
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetShifts()
        {
            List<Models.Shift> shifts = await Models.Shift.GetShifts();
            return Json(shifts, JsonRequestBehavior.AllowGet);
        }
    }
}