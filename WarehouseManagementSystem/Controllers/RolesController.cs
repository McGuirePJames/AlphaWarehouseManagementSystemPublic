using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;


namespace WarehouseManagementSystem.Controllers
{
    public class RolesController : Controller
    {
        public async Task<ActionResult> Manage()
        {
            List<Models.Role> roles = await Models.Role.CompilePermissions();

            return View(roles);
        }        
        [HttpPost]
        public async Task<ActionResult> Create(Models.Role role)
        {
            Models.Response response = await Models.Role.Create(role);

            if (response.Success)
            {
                return Json(new { Success = true, Response = "Success" });
            }
            return Json(new { Success = false, Response = response.Message });
        }
        [HttpPost]
        public async Task<ActionResult> Delete(Models.Role role)
        {
            Models.Response response = await Models.Role.DeleteCascade(role);

            if (response.Success)
            {
                return Json(new { Success = true, Response = "Success" });
            }
            return Json(new { Success = false, Response = response.Message });
        }
        [HttpPost]
        public async Task<ActionResult> Update(List<Models.Role.PermissionGroup.PermissionGroupDetails> permissionGroupDetails)
        {
            foreach(Models.Role.PermissionGroup.PermissionGroupDetails permissionGroupDetail in permissionGroupDetails)
            {
                Models.Response resultPermissionGroupDetail = await Models.Role.PermissionGroup.PermissionGroupDetails.Update(permissionGroupDetail);
                if (!resultPermissionGroupDetail.Success)
                {
                    return Json(new { Success = false, Response = resultPermissionGroupDetail.Message });
                }
            }
            return Json(new { Success = true, Response = "Success"});
        }
    }
}