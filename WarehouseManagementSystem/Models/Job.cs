using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace WarehouseManagementSystem.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string JobDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        internal static async Task<List<Models.Job>> GetJobs()
        {
            List<Models.Job> jobs = new List<Models.Job>();

            string sqlQuery =
                        @"SELECT Jobs.JobsId, Jobs.JobDescription, Jobs.CreatedDate, Jobs.ModifiedDate " +
                        @"FROM Jobs ";
                        //@"INNER JOIN Employees " +
                        //@"ON Jobs.JobsId = Employees.ShiftsId ";

            using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    await conn.OpenAsync();

                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                    while (rdr.Read())
                    {
                        Models.Job job = new Models.Job();
                        job.Id = Convert.ToInt32(rdr["JobsId"]);
                        job.JobDescription = rdr["JobDescription"].ToString();
                        job.CreatedDate = Convert.ToDateTime(rdr["CreatedDate"]);
                        job.ModifiedDate = Convert.ToDateTime(rdr["ModifiedDate"]);
                        //job.UserId = rdr["Id"].ToString();
                        jobs.Add(job);
                    }
                }
            }
            return jobs;
        }
    }
}