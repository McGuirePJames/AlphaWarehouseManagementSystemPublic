using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace WarehouseManagementSystem.Models
{
    public class User
    {
        public string ID { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public List<Models.UserRoles> UserRoles { get; set; }
        public Models.Employee Employee { get; set; }
        public string ProfilePicturePath { get; set; }

        public User()
        {
            this.UserRoles = new List<Models.UserRoles>();
        }

        internal static async Task<WarehouseManagementSystem.Models.Response> Create(WarehouseManagementSystem.Models.User user)
        {
            Microsoft.Owin.IOwinContext owinContext = System.Web.HttpContext.Current.GetOwinContext();

            UserManager<IdentityUser> userManager = owinContext.GetUserManager<UserManager<IdentityUser>>();

            IdentityUser identityUser = new IdentityUser(user.EmailAddress) { Email = user.EmailAddress };

            IdentityResult identityResult = await userManager.CreateAsync(identityUser, user.Password);

            if (identityResult.Succeeded)
            {
                System.Web.Routing.RequestContext requestContext = HttpContext.Current.Request.RequestContext;
                string token = await userManager.GenerateEmailConfirmationTokenAsync(identityUser.Id);

                string confirmURL = new UrlHelper(requestContext).Action("ConfirmEmail", "BillingModuleAccount", new { userid = identityUser.Id, token = token }, HttpContext.Current.Request.Url.Scheme);
                await userManager.SendEmailAsync(identityUser.Id, "Email Confirmation", $"This email is an invitiation to use the Wareshouse Management System.  By clicking the following link, you will be able to login using your email address and your assigned password: {confirmURL}");
                return new Models.Response() { Success = true, Message = "Success" };
            }
            return new Models.Response() { Success = false, Message = identityResult.Errors.FirstOrDefault() };
        }
        internal static async Task<List<Models.User>> GetUsers()
        {
            List<Models.User> users = new List<Models.User>();

            string sqlQuery =
                            @"SELECT AspNetUsers.Id, AspNetUsers.Email, Employees.Id, Employees.FirstName, Employees.LastName, Employees.HireDate, AspNetUserProfilePictures.Source, " +
                            @"Shifts.ShiftsId, Shifts.ShiftDescription ShiftDescription, Jobs.JobsId, Jobs.JobDescription " +
                            @"FROM AspNetUsers " +
                            @"LEFT OUTER JOIN Employees " +
                            @"ON AspNetUsers.Id = Employees.Id " +
                            @"LEFT OUTER JOIN Jobs " +
                            @"ON Employees.JobsId = Jobs.JobsId " +
                            @"LEFT OUTER JOIN Shifts " +
                            @"ON Employees.ShiftsId = Shifts.ShiftsId " +
                            @"LEFT OUTER JOIN AspNetUserProfilePictures " +
                            @"ON AspNetUsers.Id = AspNetUserProfilePictures.UserId " +
                            @"ORDER BY Employees.FirstName ASC, Employees.LastName ";

            using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    await conn.OpenAsync();

                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                    while (rdr.Read())
                    {
                        Models.User user = new Models.User();
                        user.ID = rdr["Id"].ToString();
                        user.EmailAddress = rdr["Email"].ToString();
                        user.Employee = new Employee();
                        user.Employee.FirstName = rdr["FirstName"].ToString();
                        user.Employee.LastName = rdr["LastName"].ToString();
                        user.Employee.HireDate = Convert.ToDateTime(rdr["HireDate"]);
                        user.Employee.Job.Id = Convert.ToInt32(rdr["JobsId"]);
                        user.Employee.Job.JobDescription = rdr["JobDescription"].ToString();
                        user.Employee.Shift.Id = Convert.ToInt32(rdr["ShiftsId"]);
                        user.Employee.Shift.ShiftDescription = rdr["ShiftDescription"].ToString();
                        user.ProfilePicturePath = rdr["Source"].ToString();
                        users.Add(user);
                    }
                }
            }
            return users;
        }
        internal static async Task<WarehouseManagementSystem.Models.Response> Login(WarehouseManagementSystem.Models.User user)
        {

            Microsoft.Owin.IOwinContext owinContext = System.Web.HttpContext.Current.GetOwinContext();

            UserManager<IdentityUser> userManager = owinContext.GetUserManager<UserManager<IdentityUser>>();
            SignInManager<IdentityUser, string> signInManager = owinContext.Get<SignInManager<IdentityUser, string>>();

            IdentityUser identityUser = await userManager.FindByNameAsync(user.EmailAddress);
            if (user != null)
            {
                SignInStatus signInStatus = await signInManager.PasswordSignInAsync(user.EmailAddress, user.Password, true, true);

                bool isEmailConfirmed = await userManager.IsEmailConfirmedAsync(identityUser.Id);

                //if (!isEmailConfirmed)
                //{
                //    return new WarehouseManagementSystem.Models.Response() { Success = false, Message = "Email has not been confirmed" };
                //}
                switch (signInStatus)
                {
                    case SignInStatus.Success:
                        return new WarehouseManagementSystem.Models.Response() { Success = true, Message = "Success" };
                    case SignInStatus.LockedOut:
                        return new WarehouseManagementSystem.Models.Response() { Success = false, Message = "This account has been temporarily locked. Please try again later." };
                    case SignInStatus.RequiresVerification:
                        return new WarehouseManagementSystem.Models.Response() { Success = false, Message = "Sign in requires additional verification" };
                    case SignInStatus.Failure:
                        return new WarehouseManagementSystem.Models.Response() { Success = false, Message = "Username or password is incorrect" };
                }
            }
            return new WarehouseManagementSystem.Models.Response() { Success = false, Message = "Password or username is incorrect" };

        }
        internal static async Task<Models.Response> UploadProfilePicture(string id, string userId, string source)
        {
            Models.Response response = new Models.Response();

            string sqlQuery =
                        @"INSERT INTO AspNetUserProfilePictures(AspNetUserProfilePicturesId, UserId, Source) " +
                        @"VALUES(@AspNetUserProfilePicturesId, @UserId, @Source) ";

            using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.Add("@AspNetUserProfilePicturesId", SqlDbType.NVarChar);
                    cmd.Parameters["@AspNetUserProfilePicturesId"].Value = id;

                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar);
                    cmd.Parameters["@UserId"].Value = userId;

                    cmd.Parameters.Add("@Source", SqlDbType.NVarChar);
                    cmd.Parameters["@Source"].Value = source;

                    await conn.OpenAsync();

                    SqlTransaction tran = conn.BeginTransaction();

                    cmd.Transaction = tran;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        tran.Commit();
                        response.Success = true;
                        response.Message = "Success";
                    }
                    catch (SqlException sqlEx)
                    {
                        response.Success = false;
                        response.Message = sqlEx.Message.ToString();
                        tran.Rollback();
                    }
                    catch (Exception ex)
                    {
                        response.Success = false;
                        response.Message = ex.Message.ToString();
                        tran.Rollback();
                    }
                }
            }
            return response;
        }
        internal static async Task<Models.Response> DeleteProfilePicture(string userId)
        {
            Models.Response response = new Models.Response();

            string sqlQuery =
                            @"DELETE " +
                            @"FROM AspNetUserProfilePictures " +
                            @"WHERE AspNetUserProfilePictures.UserId = @UserId ";

            using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar);
                    cmd.Parameters["@UserId"].Value = userId;

                    await conn.OpenAsync();

                    SqlTransaction tran = conn.BeginTransaction();

                    cmd.Transaction = tran;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        tran.Commit();
                        response.Success = true;
                        response.Message = "Success";
                    }
                    catch (SqlException sqlEx)
                    {
                        response.Success = false;
                        response.Message = sqlEx.Message.ToString();
                        tran.Rollback();
                    }
                    catch (Exception ex)
                    {
                        response.Success = false;
                        response.Message = ex.Message.ToString();
                        tran.Rollback();
                    }
                }
            }
            return response;
        }
        internal static async Task<Models.Response> GetUserProfilePictureSource(string userId)
        {
            Models.Response response = new Models.Response();

            string sqlQuery =
                            @"SELECT AspNetUserProfilePictures.Source " +
                            @"FROM AspNetUserProfilePictures " +
                            @"WHERE AspNetUserProfilePictures.UserId = @UserId ";

            using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar);
                    cmd.Parameters["@UserId"].Value = userId;

                    await conn.OpenAsync();

                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    try
                    {
                        response.Message = null;
                        while (rdr.Read())
                        {
                            response.Message = rdr["Source"].ToString();
                        }
                        response.Success = true;
                    }
                    catch (SqlException sqlEx)
                    {
                        response.Success = false;
                        response.Message = sqlEx.Message.ToString();
                    }
                    catch (Exception ex)
                    {
                        response.Success = false;
                        response.Message = ex.Message.ToString();
                    }
                }
            }
            return response;
        }
        internal static string GetUserIdByName(string name)
        {
            Microsoft.Owin.IOwinContext owinContext = System.Web.HttpContext.Current.GetOwinContext();
            UserManager<IdentityUser> userManager = owinContext.GetUserManager<UserManager<IdentityUser>>();
            return userManager.FindByName(name).Id;
        }
        internal static async Task<IdentityUser> GetCurrentUser()
        {
            Microsoft.Owin.IOwinContext owinContext = System.Web.HttpContext.Current.GetOwinContext();
            UserManager<IdentityUser> userManager = owinContext.GetUserManager<UserManager<IdentityUser>>();

            string userName = HttpContext.Current.User.Identity.Name;
            IdentityUser identityUser = await userManager.FindByNameAsync(userName);


            return identityUser;
        }
        internal static async Task<Models.Response> UpdateUserName(string userId, string username)
        {
            Models.Response response = new Models.Response();
            string sqlQuery =
                        @"UPDATE AspNetUsers " +
                        @"SET AspNetUsers.UserName = @UserName " +
                        @"WHERE AspNetUsers.Id = @UserId ";

            using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.Add("@UserName", SqlDbType.NVarChar);
                    cmd.Parameters["@UserName"].Value = username;

                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar);
                    cmd.Parameters["@UserId"].Value = userId;

                    await conn.OpenAsync();

                    SqlTransaction tran = conn.BeginTransaction();

                    cmd.Transaction = tran;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        tran.Commit();
                        response.Success = true;
                        response.Message = "Success";
                    }
                    catch (SqlException sqlEx)
                    {
                        response.Success = false;
                        response.Message = sqlEx.Message.ToString();
                        tran.Rollback();
                    }
                    catch (Exception ex)
                    {
                        response.Success = false;
                        response.Message = ex.Message.ToString();
                        tran.Rollback();
                    }
                }
            }
            return response;
        }
        internal static async Task<Models.Response> UpdateEmail(string userId, string email)
        {
            Models.Response response = new Models.Response();
            string sqlQuery =
                            @"UPDATE AspNetUsers " +
                            @"SET AspNetUsers.Email = @Email " +
                            @"WHERE AspNetUsers.Id = @UserId ";

            using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.Add("@Email", SqlDbType.NVarChar);
                    cmd.Parameters["@Email"].Value = email;

                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar);
                    cmd.Parameters["@UserId"].Value = userId;

                    await conn.OpenAsync();

                    SqlTransaction tran = conn.BeginTransaction();

                    cmd.Transaction = tran;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        tran.Commit();
                        response.Success = true;
                        response.Message = "Success";
                    }
                    catch (SqlException sqlEx)
                    {
                        response.Success = false;
                        response.Message = sqlEx.Message.ToString();
                        tran.Rollback();
                    }
                    catch (Exception ex)
                    {
                        response.Success = false;
                        response.Message = ex.Message.ToString();
                        tran.Rollback();
                    }
                }
            }
            return response;
        }
        internal static async Task<Models.Response> UpdateHireDate(string userId, DateTime hireDate)
        {
            Models.Response response = new Models.Response();
            string sqlQuery =
                            @"UPDATE Employees " +
                            @"SET Employees.HireDate = @HireDate " +
                            @"WHERE Employees.Id = @UserId ";

            using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.Add("@HireDate", SqlDbType.NVarChar);
                    cmd.Parameters["@HireDate"].Value = hireDate;

                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar);
                    cmd.Parameters["@UserId"].Value = userId;

                    await conn.OpenAsync();

                    SqlTransaction tran = conn.BeginTransaction();

                    cmd.Transaction = tran;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        tran.Commit();
                        response.Success = true;
                        response.Message = "Success";
                    }
                    catch (SqlException sqlEx)
                    {
                        response.Success = false;
                        response.Message = sqlEx.Message.ToString();
                        tran.Rollback();
                    }
                    catch (Exception ex)
                    {
                        response.Success = false;
                        response.Message = ex.Message.ToString();
                        tran.Rollback();
                    }
                }
            }
            return response;
        }
        internal static async Task<Models.Response> UpdateJob(string userId, int jobId)
        {
            Models.Response response = new Models.Response();
            string sqlQuery =
                            @"UPDATE Employees " +
                            @"SET Employees.JobsId = @JobId " +
                            @"WHERE Employees.Id = @UserId ";

            using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.Add("@JobId", SqlDbType.Int);
                    cmd.Parameters["@JobId"].Value = jobId;

                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar);
                    cmd.Parameters["@UserId"].Value = userId;

                    await conn.OpenAsync();

                    SqlTransaction tran = conn.BeginTransaction();

                    cmd.Transaction = tran;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        tran.Commit();
                        response.Success = true;
                        response.Message = "Success";
                    }
                    catch (SqlException sqlEx)
                    {
                        response.Success = false;
                        response.Message = sqlEx.Message.ToString();
                        tran.Rollback();
                    }
                    catch (Exception ex)
                    {
                        response.Success = false;
                        response.Message = ex.Message.ToString();
                        tran.Rollback();
                    }
                }
            }
            return response;
        }
        internal static async Task<Models.Response> UpdateShift(string userId, int shiftId)
        {
            Models.Response response = new Models.Response();
            string sqlQuery =
                            @"UPDATE Employees " +
                            @"SET Employees.ShiftsId = @ShiftId " +
                            @"WHERE Employees.Id = @UserId ";

            using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.Add("@ShiftId", SqlDbType.NVarChar);
                    cmd.Parameters["@ShiftId"].Value = shiftId;

                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar);
                    cmd.Parameters["@UserId"].Value = userId;

                    await conn.OpenAsync();

                    SqlTransaction tran = conn.BeginTransaction();

                    cmd.Transaction = tran;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        tran.Commit();
                        response.Success = true;
                        response.Message = "Success";
                    }
                    catch (SqlException sqlEx)
                    {
                        response.Success = false;
                        response.Message = sqlEx.Message.ToString();
                        tran.Rollback();
                    }
                    catch (Exception ex)
                    {
                        response.Success = false;
                        response.Message = ex.Message.ToString();
                        tran.Rollback();
                    }
                }
            }
            return response;
        }
    }
}
