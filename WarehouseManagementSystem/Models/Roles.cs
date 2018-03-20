using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;


namespace WarehouseManagementSystem.Models
{
    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public List<PermissionGroup> PermissionGroups { get; set; }

        public Role()
        {
            this.PermissionGroups = new List<Models.Role.PermissionGroup>();
        }

        internal static async Task<Models.Response> Create(Models.Role role)
        {

            Models.IdentityModel.ApplicationDbContext context = new Models.IdentityModel.ApplicationDbContext();
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            IdentityRole identityRole = new IdentityRole();
            identityRole.Name = role.Name;
            identityRole.Id = role.Id;

            IdentityResult responseCreateRole = await roleManager.CreateAsync(identityRole);

            if (responseCreateRole.Succeeded)
            {
                foreach (Models.Role.PermissionGroup permissionGroup in role.PermissionGroups)
                {
                    Models.Response responseCreatePermissionGroup = await Models.Role.PermissionGroup.Create(permissionGroup);
                    if (!responseCreatePermissionGroup.Success)
                    {
                        return new Models.Response() { Success = false, Message = responseCreatePermissionGroup.Message };
                    }
                }
                //if this line is reached, nothing has errored out
                foreach (Models.Role.PermissionGroup permissionGroup in role.PermissionGroups)
                {
                    foreach (Models.Role.PermissionGroup.PermissionGroupDetails permissionGroupDetails in permissionGroup.PermissionGroupsDetails)
                    {
                        Models.Response responseCreatePermissionGroupDetails = await Models.Role.PermissionGroup.PermissionGroupDetails.Create(permissionGroupDetails);

                        if (!responseCreatePermissionGroupDetails.Success)
                        {
                            return new Models.Response() { Success = false, Message = responseCreatePermissionGroupDetails.Message };
                        }
                    }
                }
            }
            else
            {
                return new Models.Response() { Success = false, Message = responseCreateRole.Errors.FirstOrDefault() };
            }
            return new Models.Response() { Success = true, Message = "Success" };

        }
        internal static async Task<Models.Response> InactivateRole(string roleId)
        {
            Models.Response response = new Models.Response();

            string sqlQuery =
                    @"UPDATE AspNetRolesStatus " +
                    @"SET AspNetRolesStatus.StatusId = 2 " +
                    @"WHERE AspNetRolesStatus.RoleId = @RoleId ";

            using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.Add("@RoleId", SqlDbType.NVarChar);
                    cmd.Parameters["@RoleId"].Value = roleId;

                    await conn.OpenAsync();

                    SqlTransaction tran = conn.BeginTransaction();
                    cmd.Transaction = tran;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        tran.Commit();
                    }
                    catch (SqlException sqlEx)
                    {
                        tran.Rollback();
                        response.Message = sqlEx.ToString();
                    }
                    catch (Exception ex)
                    {
                        response.Message = ex.ToString();
                    }
                }
            }

            return response;
        }
        internal static async Task<List<Models.Role>> GetRoles()
        {
            List<Models.Role> roles = new List<Models.Role>();

            string sqlQuery =
                @"SELECT AspNetRoles.Id, AspNetRoles.Name " +
                @"FROM AspNetRoles ";

            using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    await conn.OpenAsync();

                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                    while (rdr.Read())
                    {
                        Models.Role role = new Models.Role();
                        role.Id = rdr["Id"].ToString();
                        role.Name = rdr["Name"].ToString();
                        roles.Add(role);
                    }
                }
            }
            return roles;
        }
        internal static async Task<List<Models.Role>> CompilePermissions()
        {
            List<Models.Role> roles = await Models.Role.GetRoles();
            List<PermissionGroup> permissionGroups = await Models.Role.PermissionGroup.GetPermissionGroups();
            List<Models.Role.PermissionGroup.PermissionGroupDetails> permissionGroupDetails = await Models.Role.PermissionGroup.PermissionGroupDetails.GetPermissionGroupDetails();


            for (int i = 0; i < permissionGroups.Count; i++)
            {
                permissionGroups[i].PermissionGroupsDetails.AddRange(permissionGroupDetails.Where(x => x.PermissionGroupsId == permissionGroups[i].Id).ToList());
            }
            for (int i = 0; i < roles.Count; i++)
            {
                roles[i].PermissionGroups.AddRange(permissionGroups.Where(x => x.RoleId == roles[i].Id).ToList());
            }
            return roles;
        }
        internal static async Task<Models.Response> Delete(string roleId)
        {
            Models.Response response = new Models.Response();
            string sqlQuery =
                        @"DELETE " +
                        @"FROM AspNetRoles " +
                        @"WHERE AspNetRoles.Id = @RoleId ";

            using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {

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
        internal static async Task<Models.Response> DeleteCascade(Models.Role role)
        {
            foreach (Models.Role.PermissionGroup permissionGroup in role.PermissionGroups)
            {
                foreach (Models.Role.PermissionGroup.PermissionGroupDetails permissionGroupDetails in permissionGroup.PermissionGroupsDetails)
                {
                    Models.Response resultDeletePermissionGroupDetails = await Models.Role.PermissionGroup.PermissionGroupDetails.Delete(permissionGroupDetails.Id);

                    if (!resultDeletePermissionGroupDetails.Success)
                    {
                        return new Models.Response() { Success = false, Message = resultDeletePermissionGroupDetails.Message };
                    }
                }
            }

            foreach (Models.Role.PermissionGroup permissionGroup in role.PermissionGroups)
            {
                Models.Response resultDeletePermissionGroup = await Models.Role.PermissionGroup.Delete(permissionGroup.Id);
                if (!resultDeletePermissionGroup.Success)
                {
                    return new Models.Response() { Success = false, Message = resultDeletePermissionGroup.Message };
                }
            }

            Models.Response resultDeleteRole = await Models.Role.Delete(role.Id);
            if (!resultDeleteRole.Success)
            {
                return new Models.Response() { Success = false, Message = resultDeleteRole.Message };
            }
            return new Models.Response() { Success = true, Message = "Success" };
        }


        public class PermissionGroup
        {
            public string Id { get; set; }
            public string RoleId { get; set; }
            public string Description { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime ModifiedDate { get; set; }
            public List<PermissionGroupDetails> PermissionGroupsDetails { get; set; }

            public PermissionGroup()
            {
                this.PermissionGroupsDetails = new List<Models.Role.PermissionGroup.PermissionGroupDetails>();
            }

            internal static async Task<List<PermissionGroup>> GetPermissionGroups()
            {
                List<PermissionGroup> permissionGroups = new List<PermissionGroup>();

                string sqlQuery =
                            @"SELECT PermissionGroups.PermissionGroupsId ,PermissionGroups.RoleId ,PermissionGroups.Description ,PermissionGroups.CreatedDate ,PermissionGroups.ModifiedDate " +
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
                            Models.Role.PermissionGroup permissionGroup = new Models.Role.PermissionGroup();
                            permissionGroup.Id = rdr["PermissionGroupsId"].ToString();
                            permissionGroup.RoleId = rdr["RoleId"].ToString();
                            permissionGroup.Description = rdr["Description"].ToString();
                            permissionGroup.CreatedDate = Convert.ToDateTime(rdr["CreatedDate"]);
                            permissionGroup.ModifiedDate = Convert.ToDateTime(rdr["ModifiedDate"]);
                            permissionGroups.Add(permissionGroup);
                        }
                    }
                }
                return permissionGroups;
            }
            internal static async Task<Models.Response> Create(PermissionGroup permissionGroup)
            {
                Models.Response response = new Models.Response();
                string sqlQuery =
                                @"INSERT INTO PermissionGroups(PermissionGroupsId, RoleId, Description, CreatedDate, ModifiedDate) " +
                                @"VALUES(@PermissionGroupsId, @RoleId, @Description, @CreatedDate, @ModifiedDate) ";

                using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {

                        cmd.Parameters.Add("@PermissionGroupsId", SqlDbType.NVarChar);
                        cmd.Parameters["@PermissionGroupsId"].Value = permissionGroup.Id;

                        cmd.Parameters.Add("@RoleId", SqlDbType.NVarChar);
                        cmd.Parameters["@RoleId"].Value = permissionGroup.RoleId;

                        cmd.Parameters.Add("@Description", SqlDbType.NVarChar);
                        cmd.Parameters["@Description"].Value = permissionGroup.Description;

                        cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime);
                        cmd.Parameters["@CreatedDate"].Value = System.DateTime.Now;

                        cmd.Parameters.Add("@ModifiedDate", SqlDbType.DateTime);
                        cmd.Parameters["@ModifiedDate"].Value = System.DateTime.Now;

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
            internal static async Task<Models.Response> Delete(string permissionGroupsId)
            {
                Models.Response response = new Models.Response();
                string sqlQuery =
                            @"DELETE " +
                            @"FROM PermissionGroups " +
                            @"WHERE PermissionGroups.PermissionGroupsId = @PermissionGroupsId ";

                using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {

                        cmd.Parameters.Add("@PermissionGroupsId", SqlDbType.NVarChar);
                        cmd.Parameters["@PermissionGroupsId"].Value = permissionGroupsId;

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


            public class PermissionGroupDetails
            {
                public string Id { get; set; }
                public string PermissionGroupsId { get; set; }
                public string Description { get; set; }
                public int Allowed { get; set; }
                public DateTime CreatedDate { get; set; }
                public DateTime ModifiedDate { get; set; }

                internal static async Task<List<PermissionGroupDetails>> GetPermissionGroupDetails()
                {
                    List<PermissionGroupDetails> permissionGroupDetails = new List<PermissionGroupDetails>();

                    string sqlQuery =
                                    @"SELECT PermissionGroupDetails.PermissionGroupDetailsId, PermissionGroupDetails.PermissionGroupsId, PermissionGroupDetails.Description, PermissionGroupDetails.Allowed, " +
                                    @"PermissionGroupDetails.CreatedDate, PermissionGroupDetails.ModifiedDate " +
                                    @"FROM PermissionGroupDetails " +
                                    @"ORDER BY CASE " +
                                        @"WHEN PermissionGroupDetails.Description = 'CREATE' THEN '1' " +
                                        @"WHEN PermissionGroupDetails.Description = 'READ' THEN '2' " +
                                        @"WHEN PermissionGroupDetails.Description = 'UPDATE' THEN '3' " +
                                        @"WHEN PermissionGroupDetails.Description = 'DELETE' THEN '4' " +
                                    @"END ASC ";

                    using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                        {
                            await conn.OpenAsync();

                            SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                            while (rdr.Read())
                            {
                                PermissionGroupDetails permissionGroupDetail = new PermissionGroupDetails();
                                permissionGroupDetail.Id = rdr["PermissionGroupDetailsId"].ToString();
                                permissionGroupDetail.PermissionGroupsId = rdr["PermissionGroupsId"].ToString();
                                permissionGroupDetail.Description = rdr["Description"].ToString();
                                permissionGroupDetail.Allowed = Convert.ToInt32(rdr["Allowed"]);
                                permissionGroupDetail.CreatedDate = Convert.ToDateTime(rdr["CreatedDate"]);
                                permissionGroupDetail.ModifiedDate = Convert.ToDateTime(rdr["ModifiedDate"]);
                                permissionGroupDetails.Add(permissionGroupDetail);
                            }
                        }
                    }
                    return permissionGroupDetails;
                }
                internal static async Task<Models.Response> Create(PermissionGroupDetails permissionGroupDetails)
                {
                    Models.Response response = new Models.Response();
                    string sqlQuery =
                                @"INSERT INTO PermissionGroupDetails(PermissionGroupDetailsId, PermissionGroupsId, Description, Allowed, CreatedDate, ModifiedDate) " +
                                @"VALUES(@PermissionGroupDetailsId, @PermissionGroupsId, @Description, @Allowed, @CreatedDate, @ModifiedDate) ";

                    using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                        {

                            cmd.Parameters.Add("@PermissionGroupDetailsId", SqlDbType.NVarChar);
                            cmd.Parameters["@PermissionGroupDetailsId"].Value = permissionGroupDetails.Id;

                            cmd.Parameters.Add("@PermissionGroupsId", SqlDbType.NVarChar);
                            cmd.Parameters["@PermissionGroupsId"].Value = permissionGroupDetails.PermissionGroupsId;

                            cmd.Parameters.Add("@Description", SqlDbType.NVarChar);
                            cmd.Parameters["@Description"].Value = permissionGroupDetails.Description;

                            cmd.Parameters.Add("@Allowed", SqlDbType.Bit);
                            cmd.Parameters["@Allowed"].Value = permissionGroupDetails.Allowed;

                            cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime);
                            cmd.Parameters["@CreatedDate"].Value = System.DateTime.Now;

                            cmd.Parameters.Add("@ModifiedDate", SqlDbType.DateTime);
                            cmd.Parameters["@ModifiedDate"].Value = System.DateTime.Now;

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
                internal static async Task<Models.Response> Delete(string permissionGroupDetailsId)
                {
                    Models.Response response = new Models.Response();
                    string sqlQuery =
                                    @"DELETE " +
                                    @"FROM PermissionGroupDetails " +
                                    @"WHERE PermissionGroupDetails.PermissionGroupDetailsId = @PermissionGroupDetailsId ";

                    using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                        {

                            cmd.Parameters.Add("@PermissionGroupDetailsId", SqlDbType.NVarChar);
                            cmd.Parameters["@PermissionGroupDetailsId"].Value = permissionGroupDetailsId;

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
                internal static async Task<Models.Response> Update(PermissionGroupDetails permissionGroupDetails)
                {
                    Models.Response response = new Models.Response();
                    string sqlQuery =
                                    @"UPDATE PermissionGroupDetails " +
                                    @"SET Allowed = @Allowed " +
                                    @"WHERE PermissionGroupDetails.PermissionGroupDetailsId = @PermissionGroupDetailsId ";

                    using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                        {
                            cmd.Parameters.Add("@Allowed", SqlDbType.Int);
                            cmd.Parameters["@Allowed"].Value = permissionGroupDetails.Allowed;

                            cmd.Parameters.Add("@PermissionGroupDetailsId", SqlDbType.NVarChar);
                            cmd.Parameters["@PermissionGroupDetailsId"].Value = permissionGroupDetails.Id;

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
    }
}