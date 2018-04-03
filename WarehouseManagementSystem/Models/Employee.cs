using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace WarehouseManagementSystem.Models
{
    public class Employee
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }      
        public DateTime HireDate { get; set; }
        public Models.Shift Shift { get; set; }
        public int ShiftId { get; set; }
        public Models.Job Job { get; set; }
        public int JobId { get; set; }
        public string Title { get; set; }

        public Employee()
        {
            this.Shift = new Models.Shift();
            this.Job = new Models.Job();
        }
        internal static async Task<Models.Response> Create(Models.Employee employee)
        {
            Models.Response response = new Models.Response();
            string sqlQuery =
                            @"INSERT INTO Employees(EmployeeId, Id, FirstName, LastName, HireDate, JobsId, ShiftsId) " +
                            @"VALUES(@EmployeeId, @Id, @FirstName, @LastName, @HireDate, @JobsId, @ShiftsId) ";

            using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {

                    cmd.Parameters.Add("@EmployeeId", SqlDbType.NVarChar);
                    cmd.Parameters["@EmployeeId"].Value = employee.Id;

                    cmd.Parameters.Add("@Id", SqlDbType.NVarChar);
                    cmd.Parameters["@Id"].Value = employee.UserId;

                    cmd.Parameters.Add("@FirstName", SqlDbType.NVarChar);
                    cmd.Parameters["@FirstName"].Value = employee.FirstName;

                    cmd.Parameters.Add("@LastName", SqlDbType.NVarChar);
                    cmd.Parameters["@LastName"].Value = employee.LastName;

                    cmd.Parameters.Add("@HireDate", SqlDbType.DateTime);
                    cmd.Parameters["@HireDate"].Value = employee.HireDate;

                    cmd.Parameters.Add("@JobsId", SqlDbType.Int);
                    cmd.Parameters["@JobsId"].Value = employee.Job.Id;

                    cmd.Parameters.Add("@ShiftsId", SqlDbType.Int);
                    cmd.Parameters["@ShiftsId"].Value = employee.Shift.Id;

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
        internal static async Task<List<Models.Employee>> GetEmployees()
        {
            List<Models.Employee> employees = new List<Models.Employee>();
            string sqlQuery =
                        @"SELECT Employees.EmployeeId, Employees.Id, Employees.FirstName, Employees.LastName, Employees.HireDate, Employees.JobsId, Employees.ShiftsId " +
                        @"FROM Employees ";

            using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    await conn.OpenAsync();


                        SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                        while (rdr.Read())
                        {
                            Models.Employee employee = new Models.Employee();
                            employee.Id = rdr["EmployeeId"].ToString();
                            employee.UserId = rdr["Id"].ToString();
                            employee.FirstName = rdr["FirstName"].ToString();
                            employee.LastName = rdr["LastName"].ToString();
                            employee.HireDate = Convert.ToDateTime(rdr["HireDate"]);
                            employee.ShiftId = Convert.ToInt32(rdr["ShiftsId"]);
                            employee.JobId = Convert.ToInt32(rdr["JobsId"]);
                            employees.Add(employee);
                        }
                    }
            }
            return employees;
        }
    }
}