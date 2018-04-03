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
        public List<RolePermissionGroups> PermissionGroups { get; set; }

        public UserRoles()
        {
            this.PermissionGroups = new List<RolePermissionGroups>();
        }

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
        internal static async Task<Models.Response> RemoveUserFromRole(string userId, string roleId)
        {
            Models.Response response = new Models.Response();
            string sqlQuery =
                        @"DELETE " +
                        @"FROM AspNetUserRoles " +
                        @"WHERE UserId = @UserId " +
                        @"AND RoleId = @RoleId ";

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
        internal static async Task<List<Models.UserRoles>> GetUserRolesByUserId(string userId)
        {
            List<Models.UserRoles> userRoles = new List<Models.UserRoles>();

            string sqlQuery =
                        @"SELECT AspNetUserRoles.UserId, AspNetUserRoles.RoleId, AspNetRoles.Name " +
                        @"FROM AspNetUserRoles " +
                        @"INNER JOIN AspNetRoles " +
                        @"ON AspNetUserRoles.RoleId = AspNetRoles.Id " +
                        @"WHERE AspNetUserRoles.UserId = @UserId " +
                        @"ORDER BY AspNetRoles.Name ";


            using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    await conn.OpenAsync();

                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar);
                    cmd.Parameters["@UserId"].Value = userId;

                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                    while (rdr.Read())
                    {
                        Models.UserRoles userRole = new Models.UserRoles();
                        userRole.UserID = rdr["UserId"].ToString();
                        userRole.RoleID = rdr["RoleId"].ToString();
                        userRole.RoleName = rdr["Name"].ToString();
                        userRoles.Add(userRole);
                    }
                }
            }
            return userRoles;
        }
        internal static async Task<List<Models.UserRoles>> GetFullUserPermissionsByUserId(string userId)
        {

            List<Models.UserRoles> userRoles = await GetUserRolesByUserId(userId);
            List<Models.UserRoles.RolePermissionGroups> rolePermissionGroups = await Models.UserRoles.RolePermissionGroups.GetPermissionGroups();
            List<Models.UserRoles.RolePermissionGroups.PermissionGroupDetails> permissionGroupDetails = await Models.UserRoles.RolePermissionGroups.PermissionGroupDetails.GetPermissionGroupDetails();

            for(int i = 0; i <= rolePermissionGroups.Count; i++)
            {
                rolePermissionGroups[i]._PermissionGroupDetails = permissionGroupDetails.Where(details => details.PermissionGroupsId == rolePermissionGroups[i].PermissionGroupsId).ToList();
            }
            for(int i = 0; i < userRoles.Count; i++)
            {
                userRoles[i].PermissionGroups = rolePermissionGroups.Where(group => group.RoleId == userRoles[i].RoleID).ToList();
            }
            return userRoles;
        }
        internal static Boolean DoesUserHavePermission(string webPage, string permissionType, string userId)
        {

            string sqlQuery =
                        @"SELECT PermissionGroupDetails.Allowed " +
                        @"FROM AspNetUserRoles " +
                        @"INNER JOIN PermissionGroups " +
                        @"ON AspNetUserRoles.RoleId = PermissionGroups.RoleId " +
                        @"INNER JOIN AspNetRoles " +
                        @"ON AspNetUserRoles.RoleId = AspNetRoles.Id " +
                        @"INNER JOIN AspNetUsers " +
                        @"ON AspNetUserRoles.UserId = AspNetUsers.Id " +
                        @"INNER JOIN PermissionGroupDetails " +
                        @"ON PermissionGroups.PermissionGroupsId = PermissionGroupDetails.PermissionGroupsId " +
                        @"WHERE PermissionGroups.Description = @PermissionGroupsDescription " +
                        @"AND PermissionGroupDetails.Description = @PermissionGroupDetailsDescription " +
                        @"AND AspNetUsers.Id = @UserId " +
                        @"ORDER BY AspNetUsers.Email, AspNetRoles.Name, PermissionGroups.Description, PermissionGroupDetails.Description ";

            using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.Add("@PermissionGroupsDescription", SqlDbType.NVarChar);
                    cmd.Parameters["@PermissionGroupsDescription"].Value = webPage;

                    cmd.Parameters.Add("@PermissionGroupDetailsDescription", SqlDbType.NVarChar);
                    cmd.Parameters["@PermissionGroupDetailsDescription"].Value = permissionType;

                    cmd.Parameters.Add("@UserId", SqlDbType.NVarChar);
                    cmd.Parameters["@UserId"].Value = userId;

                    conn.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        if(Convert.ToInt32(rdr["Allowed"]) == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        public class RolePermissionGroups
        {
            public string PermissionGroupsId { get; set; }
            public string PermissionGroupsName { get; set; }
            public string RoleId { get; set; }
            public List<PermissionGroupDetails> _PermissionGroupDetails { get; set; }

            public RolePermissionGroups()
            {
                this._PermissionGroupDetails = new List<PermissionGroupDetails>();
            }

            internal static async Task<List<Models.UserRoles.RolePermissionGroups>> GetPermissionGroupsByRoleId(string roleId)
            {
                List<Models.UserRoles.RolePermissionGroups> permissionGroups = new List<Models.UserRoles.RolePermissionGroups>();

                string sqlQuery =
                            @"SELECT PermissionGroups.PermissionGroupsId, PermissionGroups.RoleId, PermissionGroups.Description " +
                            @"FROM PermissionGroups " +
                            @"WHERE PermissionGroups.RoleId = @RoleId " +
                            @"ORDER BY PermissionGroups.Description ";

                using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        await conn.OpenAsync();

                        cmd.Parameters.Add("@RoleId", SqlDbType.NVarChar);
                        cmd.Parameters["@RoleId"].Value = roleId;

                        SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                        while (rdr.Read())
                        {
                            Models.UserRoles.RolePermissionGroups permissionGroup = new Models.UserRoles.RolePermissionGroups();

                            permissionGroup.PermissionGroupsId = rdr["PermissionGroupsId"].ToString();
                            permissionGroup.RoleId = rdr["RoleId"].ToString();
                            permissionGroup.PermissionGroupsName = rdr["Description"].ToString();
                            permissionGroups.Add(permissionGroup);
                        }
                    }
                }
                return permissionGroups;
            }
            internal static async Task<List<Models.UserRoles.RolePermissionGroups>> GetPermissionGroups()
            {
                List<Models.UserRoles.RolePermissionGroups> permissionGroups = new List<Models.UserRoles.RolePermissionGroups>();

                string sqlQuery =
                            @"SELECT PermissionGroups.PermissionGroupsId, PermissionGroups.RoleId, PermissionGroups.Description " +
                            @"FROM PermissionGroups " +
                            @"ORDER BY PermissionGroups.Description ";

                using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        await conn.OpenAsync();

                        SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                        while (rdr.Read())
                        {
                            Models.UserRoles.RolePermissionGroups permissionGroup = new Models.UserRoles.RolePermissionGroups();

                            permissionGroup.PermissionGroupsId = rdr["PermissionGroupsId"].ToString();
                            permissionGroup.RoleId = rdr["RoleId"].ToString();
                            permissionGroup.PermissionGroupsName = rdr["Description"].ToString();
                            permissionGroups.Add(permissionGroup);
                        }
                    }
                }
                return permissionGroups;
            }

            public class PermissionGroupDetails
            {
                public string PermissionGroupDetailsId { get; set; }
                public string PermissionGroupsId { get; set; }
                public string PermissionGroupDetailsDescription { get; set; }
                public string Allowed { get; set; }

                internal static async Task<List<Models.UserRoles.RolePermissionGroups.PermissionGroupDetails>> GetPermissionGroupDetailsByPermissionGroupId(string permissionGroupId)
                {
                    List<Models.UserRoles.RolePermissionGroups.PermissionGroupDetails> permissionGroupDetails = new List<Models.UserRoles.RolePermissionGroups.PermissionGroupDetails>();

                    string sqlQuery =
                            @"SELECT PermissionGroupDetails.PermissionGroupDetailsId, PermissionGroupDetails.PermissionGroupsId, PermissionGroupDetails.Description, PermissionGroupDetails.Allowed " +
                            @"FROM PermissionGroupDetails " +
                            @"WHERE PermissionGroupDetails.PermissionGroupDetailsId = @PermissionGroupDetailsId " +
                            @"ORDER BY PermissionGroupDetails.PermissionGroupsId DESC, PermissionGroupDetails.Description ";


                    using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                        {
                            await conn.OpenAsync();

                            SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                            while (rdr.Read())
                            {
                                Models.UserRoles.RolePermissionGroups.PermissionGroupDetails permissionGroupDetail = new Models.UserRoles.RolePermissionGroups.PermissionGroupDetails();
                                permissionGroupDetail.PermissionGroupDetailsId = rdr["PermissionGroupDetailsId"].ToString();
                                permissionGroupDetail.PermissionGroupsId = rdr["PermissionGroupsId"].ToString();
                                permissionGroupDetail.PermissionGroupDetailsDescription = rdr["Description"].ToString();
                                permissionGroupDetail.Allowed = rdr["Allowed"].ToString();
                                permissionGroupDetails.Add(permissionGroupDetail);
                            }
                        }
                    }
                    return permissionGroupDetails;
                }
                internal static async Task<List<Models.UserRoles.RolePermissionGroups.PermissionGroupDetails>> GetPermissionGroupDetails()
                {
                    List<Models.UserRoles.RolePermissionGroups.PermissionGroupDetails> permissionGroupDetails = new List<Models.UserRoles.RolePermissionGroups.PermissionGroupDetails>();

                    string sqlQuery =
                            @"SELECT PermissionGroupDetails.PermissionGroupDetailsId, PermissionGroupDetails.PermissionGroupsId, PermissionGroupDetails.Description, PermissionGroupDetails.Allowed " +
                            @"FROM PermissionGroupDetails " +
                            @"ORDER BY PermissionGroupDetails.PermissionGroupsId DESC, PermissionGroupDetails.Description ";

                    using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                        {
                            await conn.OpenAsync();

                            SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                            while (rdr.Read())
                            {
                                Models.UserRoles.RolePermissionGroups.PermissionGroupDetails permissionGroupDetail = new Models.UserRoles.RolePermissionGroups.PermissionGroupDetails();
                                permissionGroupDetail.PermissionGroupDetailsId = rdr["PermissionGroupDetailsId"].ToString();
                                permissionGroupDetail.PermissionGroupsId = rdr["PermissionGroupsId"].ToString();
                                permissionGroupDetail.PermissionGroupDetailsDescription = rdr["Description"].ToString();
                                permissionGroupDetail.Allowed = rdr["Allowed"].ToString();
                                permissionGroupDetails.Add(permissionGroupDetail);
                            }
                        }
                    }
                    return permissionGroupDetails;
                }
            }


        }

    }
}