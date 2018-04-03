using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace WarehouseManagementSystem.Models
{
    public class Shift
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ShiftDescription { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        internal static async Task<List<Models.Shift>> GetShifts()
        {
            List<Models.Shift> shifts = new List<Models.Shift>();

            string sqlQuery =
                    @"SELECT Shifts.ShiftsId, Shifts.ShiftDescription, Shifts.CreatedDate, Shifts.ModifiedDate " +
                    @"FROM Shifts ";
                    //@"INNER JOIN Employees " +
                    //@"ON Shifts.ShiftsId = Employees.ShiftsId ";

            using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    await conn.OpenAsync();

                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                    while (rdr.Read())
                    {
                        Models.Shift shift = new Models.Shift();
                        shift.Id = Convert.ToInt32(rdr["ShiftsId"]);
                        shift.ShiftDescription = rdr["ShiftDescription"].ToString();
                        shift.CreatedDate = Convert.ToDateTime(rdr["CreatedDate"]);
                        shift.ModifiedDate = Convert.ToDateTime(rdr["ModifiedDate"]);
                        //shift.UserId = rdr["Id"].ToString();
                        shifts.Add(shift);
                    }
                }
            }
            return shifts;
        }
    }
}