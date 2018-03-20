using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace WarehouseManagementSystem.Models
{
    public class UserRoles
    {
        public string UserID { get; set; }
        public string RoleID { get; set; }
        public string RoleName { get; set; }

        internal static async Task<List<Models.UserRoles>> GetRoles()
        {
            List<Models.UserRoles> userRoles = new List<Models.UserRoles>();

            string sqlQuery =
                        @"SELECT AspNetUserRoles.UserId, AspNetUserRoles.RoleId, AspNetRoles.Name [RoleName]  " +
                        @"FROM AspNetUserRoles " +
                        @"INNER JOIN AspNetRoles " +
                        @"ON AspNetUserRoles.RoleId = AspNetRoles.Id ";

            using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    await conn.OpenAsync();

                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                    while (rdr.Read())
                    {
                        Models.UserRoles userRole = new Models.UserRoles();
                        userRole.UserID = rdr["UserId"].ToString();
                        userRole.RoleID = rdr["RoleId"].ToString();
                        userRole.RoleName = rdr["RoleName"].ToString();
                        userRoles.Add(userRole);
                    }
                }
            }
            return userRoles;
        }
        internal static async Task<Models.Response> AddUserToRole(string userId, string roleId)
        {
            Models.Response response = new Models.Response();
            string sqlQuery =
                            @"INSERT INTO AspNetUserRoles(UserId, RoleId) " +
                            @"VALUES(@UserId, @RoleId) ";

            using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar);
                    cmd.Parameters["@UserId"].Value = userId;

                    cmd.Parameters.Add("@RoleId", SqlDbType.NVarChar);
                    cmd.Parameters["@RoleId"].Value = roleId;

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