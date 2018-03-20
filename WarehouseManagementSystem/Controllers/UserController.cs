using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace WarehouseManagementSystem.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public async Task<ActionResult> Login()
        {
            return View();
        }
        public async Task<ActionResult> Manage()
        {
            List<Models.User> users = await Models.User.GetUsers();
            List<Models.UserRoles> userRoles = await Models.UserRoles.GetRoles();
            List<Models.Job> jobs = await Models.Job.GetJobs();
            List<Models.Shift> shifts = await Models.Shift.GetShifts();

            //assigns userroles to the user model
            for(int i = 0; i < users.Count; i++)
            {
                users[i].UserRoles = userRoles.Where(id => id.UserID == users[i].ID).ToList();
            }
            ViewModels.User.Manage viewModelManageUsers = new ViewModels.User.Manage()
            {
                Users = users,
                Jobs = jobs,
                Shifts = shifts
            };

            return View(viewModelManageUsers);
        }
        [HttpPost]
        public async Task<ActionResult> Login(string username, string password)
        {
            WarehouseManagementSystem.Models.User user = new WarehouseManagementSystem.Models.User()
            {
                EmailAddress = username,
                Password = password
            };

            WarehouseManagementSystem.Models.Response response = await WarehouseManagementSystem.Models.User.Login(user);

            return Json(new
            {
                success = response.Success,
                responseText = response.Message
            });

        }
        [HttpPost]
        public async Task<ActionResult> Create(Models.User user, Models.Employee employee)
        {
            UserManager<IdentityUser> userManager = HttpContext.GetOwinContext().GetUserManager<UserManager<IdentityUser>>();
            Models.Response userCreateResponse = await Models.User.Create(user);

            if (userCreateResponse.Success)
            {
                IdentityUser identityUser = await userManager.FindByNameAsync(user.EmailAddress);
                employee.UserId = identityUser.Id;
                Models.Response employeeCreateResponse = await Models.Employee.Create(employee);

                if (employeeCreateResponse.Success)
                {
                    foreach(Models.UserRoles role in user.UserRoles)
                    {
                        Models.Response addToRoleResponse = await Models.UserRoles.AddUserToRole(identityUser.Id, role.RoleID);

                        if (!addToRoleResponse.Success)
                        {
                            return Json(new { Success = false, ResponseText = addToRoleResponse.Message });
                        }
                    }
                }
                else
                {
                    return Json(new { Success = false, ResponseText = employeeCreateResponse.Message });
                }
            }
            else
            {
                return Json(new { Success = false, ResponseText = userCreateResponse.Message });
            }
            return Json(new { Success = true, ResponseText = "Success"});
        }
        public ActionResult ForgotPassword()
        {
            return View();
        }
        public ActionResult ResetPassword()
        {
            return View();
        }
        public async Task<ActionResult> Add()
        {
            List<Models.Role> roles = await Models.Role.GetRoles();
            List<Models.Job> jobs = await Models.Job.GetJobs();
            List<Models.Shift> shifts = await Models.Shift.GetShifts();

            ViewModels.User.Add viewModelUserAdd = new ViewModels.User.Add()
            {
                Roles = roles,
                Jobs = jobs,
                Shifts = shifts
            };

            return View(viewModelUserAdd);
        }
        public ActionResult Home()
        {
            return View(@"~/Views/User/Home.cshtml");
        }
        [HttpPost]
        public async Task<ActionResult> UploadProfilePicture(HttpPostedFileBase image, string userId)
        {
            //resizes image
            WebImage webImage = new WebImage(image.InputStream);
            if (webImage.Width > 500)
            {
                webImage.Resize(500, 500);
            }
            webImage.FileName = image.FileName;

            //checks if image already exists.  If so it replaces it
            Models.Response getImageResult = await Models.User.GetUserProfilePictureSource(userId);

            if(getImageResult.Success && getImageResult.Message != null)
            {
                Models.Response deleteImageResponse = await Models.User.DeleteProfilePicture(userId);

                if (!deleteImageResponse.Success)
                {
                    return Json(new { Success = false, ResponseText = deleteImageResponse.Message });
                }

                Models.Azure.Blob.RemoveBlobFromStorage(getImageResult.Message.Split('/').Last(), "profilepictures");
            }

            Models.Response azureImageUploadResponse = Models.Azure.Blob.UploadImageToBlobStorage(webImage, "profilepictures");
            Models.Azure.Blob blobSource = Models.Azure.Blob.GetBlobByName(image.FileName, "profilepictures");

            if (azureImageUploadResponse.Success)
            {
                Models.Response userUploadProfilePictureResponse = await Models.User.UploadProfilePicture(Guid.NewGuid().ToString(), userId, blobSource.Uri);

                if (!userUploadProfilePictureResponse.Success)
                {
                    return Json(new { Success = false, ResponseText = userUploadProfilePictureResponse.Message });
                }
            }
            else
            {
                return Json(new { Success = false, ResponseText = azureImageUploadResponse.Message });
            }
            return Json(new { Success = true, ResponseText = "Success" });
        }
        [HttpPost]
        public async Task<ActionResult> UpdateUserName(string userId, string username)
        {
            Models.Response updateUserNameResult = await Models.User.UpdateUserName(userId, username);

            if (!updateUserNameResult.Success)
            {
                return Json(new { success = false, responseText = updateUserNameResult.Message });
            }
            return Json(new { success = true, responseText = "Success" });
        }
        [HttpPost]
        public async Task<ActionResult> UpdateEmail(string userId, string email)
        {
            Models.Response updateEmailResult = await Models.User.UpdateEmail(userId, email);

            if (!updateEmailResult.Success)
            {
                return Json(new { success = false, responseText = updateEmailResult.Message });
            }
            return Json(new { success = true, responseText = "Success" });
        }
        [HttpPost]
        public async Task<ActionResult> UpdateHireDate(string userId, string hireDate)
        {
            DateTime hireDateDateTime = Convert.ToDateTime(hireDate);

            Models.Response updateHireDateResult = await Models.User.UpdateHireDate(userId, hireDateDateTime);

            if (!updateHireDateResult.Success)
            {
                return Json(new { success = false, responseText = updateHireDateResult.Message });
            }
            return Json(new { success = true, responseText = "Success" });
        }
        [HttpPost]
        public async Task<ActionResult> UpdateJob(string userId, int jobId)
        {
            Models.Response updateJobTypeResult = await Models.User.UpdateJob(userId, jobId);

            if (!updateJobTypeResult.Success)
            {
                return Json(new { success = false, responseText = updateJobTypeResult.Message });
            }
            return Json(new { success = true, responseText = "Success" });
        }
        [HttpPost]
        public async Task<ActionResult> UpdateShift(string userId, int shiftId)
        {
            Models.Response updateShiftResult = await Models.User.UpdateShift(userId, shiftId);

            if (!updateShiftResult.Success)
            {
                return Json(new { success = false, responseText = updateShiftResult.Message });
            }
            return Json(new { success = true, responseText = "Success" });
        }
    }
}