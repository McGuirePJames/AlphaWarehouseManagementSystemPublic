using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WarehouseManagementSystem.Models
{
    public class Report
    {
        public Models.Employee CreatedEmployee { get; set; }
        public Models.Employee ModifiedEmployee { get; set; }
        public int ReportID { get; set; }
        public string ReportName { get; set; }
        public string ReportType { get; set; }
        public string ReportQuery { get; set; }
        public string CreatedUserID { get; set; }
        public string ModifiedUserID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<Parameters> _Parameters { get; set; }
        public Schedule _Schedule { get; set; }

        public Report()
        {
            this.CreatedEmployee = new Models.Employee();
            this.ModifiedEmployee = new Models.Employee();
        }
        internal static async Task<List<Models.Report>> GetReports()
        {
            string sqlQuery =
                "SELECT c.firstname as 'ModifiedFirstName', c.lastname as 'ModifiedLastName', b.firstname as 'CreatedFirstName', b.lastname as 'CreatedLastName', reportsid, name, reportquery, createduserid, modifieduserid, convert(date, createdDate) as 'CreatedDate', convert(date, modifieddate) as 'ModifiedDate' " +
                @"FROM reports a " +
                @"INNER JOIN employees b " +
                @"ON a.createduserid = b.id " +
                @"INNER JOIN employees c " +
                @"ON a.modifieduserid = c.id " +
                @"ORDER BY a.name ";

            List<Models.Report> reportList = new List<Models.Report>();

            using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    await conn.OpenAsync();

                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (rdr.Read())
                    {
                        Models.Report report = new Models.Report();
                        report.CreatedEmployee.FirstName = rdr["CreatedFirstName"].ToString();
                        report.CreatedEmployee.LastName = rdr["CreatedLastName"].ToString();
                        report.ModifiedEmployee.FirstName = rdr["ModifiedFirstName"].ToString();
                        report.ModifiedEmployee.LastName = rdr["ModifiedLastName"].ToString();
                        report.ReportID = Convert.ToInt32(rdr["reportsid"]);
                        report.ReportName = rdr["name"].ToString();
                        report.ReportQuery = rdr["reportquery"].ToString();
                        report.CreatedUserID = rdr["createduserid"].ToString();
                        report.ModifiedUserID = rdr["modifieduserid"].ToString();
                        report.CreatedDate = Convert.ToDateTime(rdr["CreatedDate"]);
                        report.ModifiedDate = Convert.ToDateTime(rdr["ModifiedDate"]);
                        reportList.Add(report);
                    }

                }
            }
            return reportList;
        }
        internal static async Task<Models.Report> GetReportById(int reportId)
        {

            Models.Report report = new Report();
            string sqlQuery =
                        "select c.firstname as 'ModifiedFirstName', c.lastname as 'ModifiedLastName', b.firstname as 'CreatedFirstName', b.lastname as 'CreatedLastName', reportsid, name, reportquery, createduserid, modifieduserid, convert(date, createdDate) as 'CreatedDate', convert(date, modifieddate) as 'ModifiedDate' " +
                        "from reports a " +
                        "inner join employees b " +
                        "on a.createduserid = b.id " +
                        "inner join employees c " +
                        "on a.modifieduserid = c.id " +
                        "WHERE a.ReportsId = @ReportId ";


            using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.Add("@ReportId", SqlDbType.Int);
                    cmd.Parameters["@ReportId"].Value = reportId;

                    await conn.OpenAsync();

                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                    while (rdr.Read())
                    {
                        report.CreatedEmployee.FirstName = rdr["CreatedFirstName"].ToString();
                        report.CreatedEmployee.LastName = rdr["CreatedLastName"].ToString();
                        report.ModifiedEmployee.FirstName = rdr["ModifiedFirstName"].ToString();
                        report.ModifiedEmployee.LastName = rdr["ModifiedLastName"].ToString();
                        report.ReportID = Convert.ToInt32(rdr["reportsid"]);
                        report.ReportName = rdr["name"].ToString();
                        report.ReportQuery = rdr["reportquery"].ToString();
                        report.CreatedUserID = rdr["createduserid"].ToString();
                        report.ModifiedUserID = rdr["modifieduserid"].ToString();
                        report.CreatedDate = Convert.ToDateTime(rdr["CreatedDate"]);
                        report.ModifiedDate = Convert.ToDateTime(rdr["ModifiedDate"]);
                    }

                }
            }
            return report;
        }
        internal static async Task<WarehouseManagementSystem.Models.Response> Create(Report report)
        {
            Models.Response response = new Models.Response();
            string sqlQuery =
                        @"INSERT INTO Reports(Name, ReportQuery, CreatedUserId, ModifiedUserId, CreatedDate, ModifiedDate) " +
                        @"VALUES(@Name, @ReportQuery, @CreatedUserId, @ModifiedUserId, @CreatedDate, @ModifiedDate) ";

            using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    cmd.Parameters.Add("@Name", SqlDbType.NVarChar);
                    cmd.Parameters["@Name"].Value = report.ReportName;

                    cmd.Parameters.Add("@ReportQuery", SqlDbType.NVarChar);
                    cmd.Parameters["@ReportQuery"].Value = report.ReportQuery;

                    cmd.Parameters.Add("@CreatedUserId", SqlDbType.NVarChar);
                    cmd.Parameters["@CreatedUserId"].Value = report.CreatedUserID;

                    cmd.Parameters.Add("@ModifiedUserId", SqlDbType.NVarChar);
                    cmd.Parameters["@ModifiedUserId"].Value = report.CreatedUserID;

                    cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime);
                    cmd.Parameters["@CreatedDate"].Value = report.CreatedDate;

                    cmd.Parameters.Add("@ModifiedDate", SqlDbType.DateTime);
                    cmd.Parameters["@ModifiedDate"].Value = report.ModifiedDate;

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
        internal static WarehouseManagementSystem.Models.Response Update(Report report)
        {
            return new WarehouseManagementSystem.Models.Response() { Success = false, Message = "Update of report failed" };
        }
        public static async Task<Models.Response> DeleteReport(int reportID)
        {
            Models.Response response = new Models.Response();

            string sqlQuery =
                "delete from reports where reportsID = @ReportID";
            using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))

            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))

                {
                    cmd.Parameters.Add("@ReportID", SqlDbType.Int);
                    cmd.Parameters["@ReportID"].Value = reportID;

                    await conn.OpenAsync();

                    SqlTransaction tran = conn.BeginTransaction();

                    cmd.Transaction = tran;

                    try
                    {
                        await cmd.ExecuteNonQueryAsync();
                        tran.Commit();
                        response.Success = true;
                        response.Message = "Successful";
                    }
                    catch (SqlException sqlEx)
                    {
                        tran.Rollback();
                        response.Message = sqlEx.ToString();
                        response.Success = false;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        response.Message = ex.ToString();
                        response.Success = false;
                    }
                    return response;
                }
            }
        }
        internal static async Task<List<Models.Excel.Workbook.Worksheet.Row>> RunReport(string reportQuery)
        {
            List<Models.Excel.Workbook.Worksheet.Row> rows = new List<Models.Excel.Workbook.Worksheet.Row>();



            using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
            {
                using (SqlCommand cmd = new SqlCommand(reportQuery, conn))
                {
                    await conn.OpenAsync();

                    SqlDataReader rdr = await cmd.ExecuteReaderAsync();

                    int rowCount = 1;

                    Models.Excel.Workbook.Worksheet.Row headerRow = new Models.Excel.Workbook.Worksheet.Row();
                    headerRow.RowNumber = rowCount;

                    for (int i = 1; i <= rdr.FieldCount; i++)
                    {
                        Models.Excel.Workbook.Worksheet.Cell cell = new Models.Excel.Workbook.Worksheet.Cell();
                        cell.ColumnNumber = i;
                        cell.RowNumber = rowCount;
                        cell.Value = rdr.GetName(i - 1);
                        headerRow.Cells.Add(cell);
                    }
                    rows.Add(headerRow);
                    rowCount++;
                    while (rdr.Read())
                    {

                        Models.Excel.Workbook.Worksheet.Row row = new Models.Excel.Workbook.Worksheet.Row();
                        row.RowNumber = rowCount;

                        for (int i = 1; i <= rdr.FieldCount; i++)
                        {
                            Models.Excel.Workbook.Worksheet.Cell cell = new Models.Excel.Workbook.Worksheet.Cell();
                            cell.ColumnNumber = i;
                            cell.RowNumber = rowCount;
                            cell.Value = rdr[i - 1].ToString();

                            row.Cells.Add(cell);
                        }
                        rows.Add(row);
                        rowCount++;
                    }
                }
            }
            return rows;
        }
        public class Parameters
        {
            public string Name { get; set; }
            public Type _Type { get; set; }
            public string Value { get; set; }

            public enum Type
            {
                Integer,
                Nvarchar,
                DateTime
            }
        }

        public class Schedule
        {
            public int ScheduleId { get; set; }
            public int ReportId { get; set; }
            public string HangfireJobId { get; set; }
            public string Name { get; set; }
            public string _Frequency { get; set; }
            public DateTime StartDate { get; set; }
            public List<UserNotifications> UsersToNotify { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime ModifiedDate { get; set; }
            public Models.Report Report { get; set; }

            public enum Frequency
            {
                Minutely,
                Hourly,
                Daily,
                Weekly,
                Monthly
            }

            internal static async Task<List<Models.Report.Schedule>> GetSchedules()
            {
                string sqlQuery =
                            @"SELECT ReportSchedules.ReportSchedulesId, ReportSchedules.ReportsId, ReportSchedules.HangfireJobId, ReportSchedules.Name, ReportSchedules.Frequency, ReportSchedules.StartDate, ReportSchedules.CreatedDate, ReportSchedules.ModifiedDate " +
                            @"FROM ReportSchedules ";
      
                List<Models.Report.Schedule> schedules = new List<Models.Report.Schedule>();

                using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        await conn.OpenAsync();

                        SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                        while (rdr.Read())
                        {
                            Frequency outFrequency = new Frequency();

                            Enum.TryParse(rdr["Frequency"].ToString(), out outFrequency);
                            Models.Report.Schedule schedule = new Models.Report.Schedule();
                            schedule.ScheduleId = Convert.ToInt32(rdr["ReportSchedulesId"]);
                            schedule.ReportId = Convert.ToInt32(rdr["ReportsId"]);
                            schedule.HangfireJobId = rdr["HangfireJobId"].ToString();
                            schedule.Name = rdr["Name"].ToString();
                            schedule._Frequency = rdr["Frequency"].ToString();
                            schedule.StartDate = Convert.ToDateTime(rdr["StartDate"]);
                            schedule.CreatedDate = Convert.ToDateTime(rdr["CreatedDate"]);
                            schedule.ModifiedDate = Convert.ToDateTime(rdr["ModifiedDate"]);
                            schedules.Add(schedule);
                        }
                    }
                }
                return schedules;
            }
            internal static async Task<Models.Report.Schedule> GetScheduleById(int scheduleId)
            {

                Models.Report.Schedule schedule = new Models.Report.Schedule();

                string sqlQuery =
                            @"SELECT ReportSchedules.ReportSchedulesId, ReportSchedules.ReportsId, ReportSchedules.HangfireJobId, ReportSchedules.Name, ReportSchedules.Frequency, ReportSchedules.StartDate, ReportSchedules.CreatedDate, ReportSchedules.ModifiedDate " +
                            @"FROM ReportSchedules " +
                            @"WHERE ReportSchedules.ReportSchedulesId = @ReportSchedulesId";

                using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        cmd.Parameters.Add("@ReportSchedulesId", SqlDbType.Int);
                        cmd.Parameters["@ReportSchedulesId"].Value = scheduleId;

                        await conn.OpenAsync();

                        SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                        while (rdr.Read())
                        {
   
                            string rdrFrequency = rdr["Frequency"].ToString();
                            Frequency frequency = (Frequency)Enum.Parse(typeof(Frequency), rdrFrequency);

                            schedule.ScheduleId = Convert.ToInt32(rdr["ReportSchedulesId"]);
                            schedule.ReportId = Convert.ToInt32(rdr["ReportsId"]);
                            schedule.HangfireJobId = rdr["HangfireJobId"].ToString();
                            schedule.Name = rdr["Name"].ToString();
                            schedule.StartDate = Convert.ToDateTime(rdr["StartDate"]);
                            schedule.CreatedDate = Convert.ToDateTime(rdr["CreatedDate"]);
                            schedule.ModifiedDate = Convert.ToDateTime(rdr["ModifiedDate"]);
                        }
                    }
                }
                return schedule;
            }
            internal static async Task<Models.Response> CreateSchedule(Models.Report.Schedule schedule)
            {
                Models.Response response = new Models.Response();
                string sqlQuery =
                        @"INSERT INTO ReportSchedules " +
                        @"OUTPUT INSERTED.ReportSchedulesId " +
                        @"VALUES(@ReportsId, @HangfireJobId, @Name, @Frequency, @StartDate, @CreatedDate, @ModifiedDate) ";

                using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        cmd.Parameters.Add("@ReportsId", SqlDbType.Int);
                        cmd.Parameters["@ReportsId"].Value = schedule.ReportId;

                        cmd.Parameters.Add("@HangfireJobId", SqlDbType.NVarChar);
                        cmd.Parameters["@HangfireJobId"].Value = schedule.HangfireJobId;

                        cmd.Parameters.Add("@Name", SqlDbType.NVarChar);
                        cmd.Parameters["@Name"].Value = schedule.Name;

                        cmd.Parameters.Add("@Frequency", SqlDbType.NVarChar);
                        cmd.Parameters["@Frequency"].Value = schedule._Frequency;

                        cmd.Parameters.Add("@StartDate", SqlDbType.DateTime);
                        if (schedule.StartDate >= DateTime.Now.AddMinutes(1))
                        {
                            cmd.Parameters["@StartDate"].Value = schedule.StartDate;
                        }
                        else
                        {
                            cmd.Parameters["@StartDate"].Value = DateTime.Now;
                        }
     
                        cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime);
                        cmd.Parameters["@CreatedDate"].Value = DateTime.Now;

                        cmd.Parameters.Add("@ModifiedDate", SqlDbType.DateTime);
                        cmd.Parameters["@ModifiedDate"].Value = DateTime.Now;

                        await conn.OpenAsync();

                        SqlTransaction tran = conn.BeginTransaction();

                        cmd.Transaction = tran;

                        try
                        {
                            //int scheduleId = (int)await cmd.ExecuteScalarAsync();
                            int scheduleId = (int) await cmd.ExecuteScalarAsync();
                            tran.Commit();
                            response.Success = true;
                            response.Message = scheduleId.ToString();
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
            internal static async Task<Models.Response> DeleteScheduleById(int scheduleId)
            {
                Models.Response response = new Models.Response();
                string sqlQuery =
                            @"DELETE " +
                            @"FROM ReportSchedules " +
                            @"WHERE ReportSchedules.ReportSchedulesId = @ScheduleId ";

                using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
                {
                    using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                    {
                        cmd.Parameters.Add("@ScheduleId", SqlDbType.Int);
                        cmd.Parameters["@ScheduleId"].Value = scheduleId;

                        await conn.OpenAsync();

                        SqlTransaction tran = conn.BeginTransaction();

                        cmd.Transaction = tran;

                        try
                        {
                            await cmd.ExecuteNonQueryAsync();
                            tran.Commit();
                            response.Success = true;
                            response.Message = scheduleId.ToString();
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

            public class UserNotifications
            {
                public int Id { get; set; }
                public int ReportSchedulesId { get; set; }
                public string UserId { get; set; }
                public string EmailAddress { get; set; }

                internal static async Task<List<Models.Report.Schedule.UserNotifications>> GetUserNotifications()
                {
                    string sqlQuery =
                                @"SELECT ReportSchedulesUserNotification.ReportSchedulesEmailListId, ReportSchedulesUserNotification.ReportSchedulesId, ReportSchedulesUserNotification.UserId " +
                                @"FROM ReportSchedulesUserNotification ";
                     
                    List<Models.Report.Schedule.UserNotifications> userNotifications = new List<Models.Report.Schedule.UserNotifications>();

                    using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                        {
                            await conn.OpenAsync();

                            SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                            while (rdr.Read())
                            {
                                Frequency outFrequency = new Frequency();

                                Enum.TryParse(rdr["Frequency"].ToString(), out outFrequency);

                                Models.Report.Schedule.UserNotifications userNotification = new Models.Report.Schedule.UserNotifications();
                                userNotification.Id = Convert.ToInt32(rdr["ReportSchedulesEmailListId"]);
                                userNotification.ReportSchedulesId = Convert.ToInt32(rdr["ReportSchedulesId"]);
                                userNotification.UserId = rdr["UserId"].ToString();
                                userNotifications.Add(userNotification);
                            }
                        }
                    }
                    return userNotifications;
                }
                internal static async Task<List<Models.Report.Schedule.UserNotifications>> GetUserNotificationsByScheduleId(int scheduleId)
                {
                    string sqlQuery =
                                @"SELECT ReportSchedulesUserNotification.ReportSchedulesEmailListId, ReportSchedulesUserNotification.ReportSchedulesId, ReportSchedulesUserNotification.UserId, AspNetUsers.Email " +
                                @"FROM ReportSchedulesUserNotification " +
                                @"INNER JOIN AspNetUsers " +
                                @"ON ReportSchedulesUserNotification.UserId = AspNetUsers.Id " +
                                @"WHERE ReportSchedulesUserNotification.ReportSchedulesId = @ReportSchedulesId ";

                    List <Models.Report.Schedule.UserNotifications> userNotifications = new List<Models.Report.Schedule.UserNotifications>();

                    using (SqlConnection conn = new SqlConnection(WarehouseManagementSystem.Models.Database.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                        {
                            cmd.Parameters.Add("@ReportSchedulesId", SqlDbType.Int);
                            cmd.Parameters["@ReportSchedulesId"].Value = scheduleId;

                            await conn.OpenAsync();

                            SqlDataReader rdr = await cmd.ExecuteReaderAsync();
                            while (rdr.Read())
                            {
                                Models.Report.Schedule.UserNotifications userNotification = new Models.Report.Schedule.UserNotifications();
                                userNotification.Id = Convert.ToInt32(rdr["ReportSchedulesEmailListId"]);
                                userNotification.ReportSchedulesId = Convert.ToInt32(rdr["ReportSchedulesId"]);
                                userNotification.UserId = rdr["UserId"].ToString();
                                userNotification.EmailAddress = rdr["Email"].ToString();
                                userNotifications.Add(userNotification);
                            }
                        }
                    }
                    return userNotifications;
                }
                internal static async Task<Models.Response> CreateUserNotification(int reportSchedulesId, string userId)
                {
                    Models.Response response = new Models.Response();
                    string sqlQuery =
                                @"INSERT INTO ReportSchedulesUserNotification " +
                                @"VALUES(@ReportSchedulesId, @UserId) ";

                    using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                        {
                            cmd.Parameters.Add("@ReportSchedulesId", SqlDbType.Int);
                            cmd.Parameters["@ReportSchedulesId"].Value = reportSchedulesId;

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
                                response.Message = "Success!";
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
                internal static async Task<Models.Response> DeleteUsersByScheduleId(int scheduleId)
                {
                    Models.Response response = new Models.Response();
                    string sqlQuery =
                                @"DELETE " +
                                @"FROM ReportSchedulesUserNotification " +
                                @"WHERE ReportSchedulesUserNotification.ReportSchedulesId = @ScheduleId ";

                    using (SqlConnection conn = new SqlConnection(Models.Database.GetConnectionString()))
                    {
                        using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                        {
                            cmd.Parameters.Add("@ScheduleId", SqlDbType.Int);
                            cmd.Parameters["@ScheduleId"].Value = scheduleId;

                            await conn.OpenAsync();

                            SqlTransaction tran = conn.BeginTransaction();

                            cmd.Transaction = tran;

                            try
                            {
                                await cmd.ExecuteNonQueryAsync();
                                tran.Commit();
                                response.Success = true;
                                response.Message = scheduleId.ToString();
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