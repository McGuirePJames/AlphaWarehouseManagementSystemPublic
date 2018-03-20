using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using OfficeOpenXml;
using Microsoft.AspNet.Identity.EntityFramework;
using WarehouseManagementSystem.Models;
using System.Web.UI;

namespace WarehouseManagementSystem.Controllers
{
    public class ReportsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> Add()
        {
            List<Models.Database.Table> tables = await Models.Database.Table.GetTables();
            List<Models.Database.Table.Column> columns = await Models.Database.Table.Column.GetColumns();
            List<Models.Database.Key.ForeignKey> foreignKeys = await Models.Database.Key.ForeignKey.GetForeignKeys();
            List<Models.Database.Key.PrimaryKey> primaryKeys = await Models.Database.Key.PrimaryKey.GetPrimaryKeys();

            //assigns columns, foreign keys, and primary keys to tables
            for (int i = 0; i < tables.Count; i++)
            {
                tables[i].Columns = columns.Where(id => id.TableId == tables[i].Id).ToList();

                for (int j = 0; j < tables[i].Columns.Count; j++)
                {
                    tables[i].Columns[j].ForeignKey = foreignKeys
                                                            .Where(tableId => tableId.ChildTable.Id == tables[i].Id)
                                                            .Where(columnId => columnId.ChildColumnId == tables[i].Columns[j].Id)
                                                            .FirstOrDefault();
                    if (tables[i].Columns[j].ForeignKey != null)
                    {
                        tables[i].Columns[j].IsForeignkey = true;
                    }

                    tables[i].Columns[j].PrimaryKey = primaryKeys
                                        .Where(tableId => tableId.Table.Id == tables[i].Id)
                                        .Where(columnId => columnId.ColumnId == tables[i].Columns[j].Id)
                                        .FirstOrDefault();

                    if (tables[i].Columns[j].PrimaryKey != null)
                    {
                        tables[i].Columns[j].IsPrimaryKey = true;
                    }

                }
            }
            return View(tables);
        }
        public ActionResult Manage()
        {
            return View();
        }
        public async Task<ActionResult> SavedReports()
        {
            List<Models.Report> reportList = await Models.Report.GetReports();
            return View(reportList);
        }
        public async Task<ActionResult> Schedule()
        {
            List<Models.Report> reports = await Models.Report.GetReports();
            List<Models.User> users = await Models.User.GetUsers();
            ViewModels.Reports.Schedule viewModelReports = new ViewModels.Reports.Schedule();
            viewModelReports.Reports = reports;
            viewModelReports.Users = users;

            //Models.HangfireModel.RemoveJob("ID");
            //Models.HangfireModel.CreateUserDefinedReport(1, Models.Report.Schedule.Frequency.Minutely);


            return View(viewModelReports);
        }
        [HttpPost]
        public async Task<ActionResult> CreateDelayedJob(Models.Report.Schedule schedule)
        {
            Models.Response responseCreateSchedule = await Models.Report.Schedule.CreateSchedule(schedule);
            int scheduleId = Convert.ToInt32(responseCreateSchedule.Message);

            foreach (Models.Report.Schedule.UserNotifications userToNotify in schedule.UsersToNotify)
            {
                await Models.Report.Schedule.UserNotifications.CreateUserNotification(scheduleId, userToNotify.UserId);
            }

            if (responseCreateSchedule.Success)
            {
                Models.HangfireModel.CreateDelayedJob(schedule.HangfireJobId, scheduleId, schedule.StartDate, schedule._Frequency.ToString());
                return Json(new { success = true, responseText = scheduleId.ToString() });
            }
            return Json(new { success = false, responseText = responseCreateSchedule.Message });

        }
        [HttpPost]
        public async Task<ActionResult> CreateScheduledReport(Models.Report.Schedule schedule)
        {
            //creates entry in databse to keep track of scheduled jobs
            Models.Response responseCreateSchedule = await Models.Report.Schedule.CreateSchedule(schedule);
            int scheduleId = Convert.ToInt32(responseCreateSchedule.Message);

            foreach (Models.Report.Schedule.UserNotifications userToNotify in schedule.UsersToNotify)
            {
                await Models.Report.Schedule.UserNotifications.CreateUserNotification(scheduleId, userToNotify.UserId);
            }

            if (responseCreateSchedule.Success)
            {
                Models.HangfireModel.CreateScheduledReport(schedule.HangfireJobId, Convert.ToInt32(responseCreateSchedule.Message), schedule._Frequency.ToString());
                return Json(new { success = true, responseText = scheduleId.ToString() });
            }
            return Json(new { success = false, responseText = responseCreateSchedule.Message });
        }
        [HttpGet]
        public async Task<ActionResult> GetUsers()
        {
            List<Models.User> users = await Models.User.GetUsers();

            return Json(users, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> DeleteReport(int reportID)
        {

            Models.Response response = await Models.Report.DeleteReport(reportID);
            if (response.Success)
            {
                return Json(new { success = true, responseText = "Request Success" });
            }

            return Json(new { success = false, responseText = response.Message });
        }
        [HttpPost]
        public ActionResult HelperTestQuery()
        {
            //this method is just to make the ajax call for RunReport function work with downloadable excel files
            return Json(new { success = true });
        }
        public async Task<ActionResult> TestQuery(string sqlQuery)
        {
            List<Models.Excel.Workbook.Worksheet.Row> results = await Models.Database.RunQuery(sqlQuery);
            ExcelPackage package = Models.Excel.CreateExcelPackage();
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            Models.Excel.Workbook.Worksheet.WriteRowsToWorksheet(worksheet, results);

            string fileName = string.Format("{0} {1}.xlsx", "TestReport", DateTime.Now.ToString("MMddyyyy"));
            var stream = new System.IO.MemoryStream();
            package.SaveAs(stream);
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            stream.Position = 0;
            return File(stream, contentType, fileName);
        }
        [HttpPost]
        public ActionResult HelperRunReport()
        {
            //this method is just to make the ajax call for RunReport function work with downloadable excel files
            return Json(new { success = true });
        }
        public async Task<ActionResult> RunReport(int reportId)
        {
            Models.Report report = await Models.Report.GetReportById(reportId);
            List<Models.Excel.Workbook.Worksheet.Row> results = await Models.Report.RunReport(report.ReportQuery);

            ExcelPackage package = Models.Excel.CreateExcelPackage();
            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
            Models.Excel.Workbook.Worksheet.WriteRowsToWorksheet(worksheet, results);

            string fileName = string.Format("{0} {1}.xlsx", report.ReportName, DateTime.Now.ToString("MMddyyyy"));
            var stream = new System.IO.MemoryStream();
            package.SaveAs(stream);
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            stream.Position = 0;
            return File(stream, contentType, fileName);
        }
        [HttpPost]
        public async Task<ActionResult> SaveReport(string reportName, string reportQuery)
        {
            IdentityUser identityUser = await Models.User.GetCurrentUser();

            if (identityUser == null)
            {
                return Json(new { success = false, responseText = "Please log in" });
            }
            Models.Report report = new Models.Report();
            report.ReportName = reportName;
            report.ReportQuery = reportQuery;
            report.CreatedUserID = identityUser.Id;
            report.CreatedDate = DateTime.Now;
            report.ModifiedDate = DateTime.Now;

            Models.Response resultCreateReport = await Models.Report.Create(report);

            if (!resultCreateReport.Success)
            {
                return Json(new { success = false, responseText = resultCreateReport.Message });
            }
            return Json(new { success = true, responseText = "Success!" });

        }
        [HttpGet]
        public async Task<ActionResult> GetScheduledReports()
        {
            List<Models.Report> reports = await Models.Report.GetReports();
            List<Models.Report.Schedule> scheduledReports = await Models.Report.Schedule.GetSchedules();

            for(int i = 0; i < scheduledReports.Count; i++)
            {
                scheduledReports[i].Report = reports.Where(report => report.ReportID == scheduledReports[i].ReportId).FirstOrDefault();
            }


            return Json(scheduledReports, JsonRequestBehavior.AllowGet);
        }
        [HttpDelete]
        public async Task<ActionResult> DeleteScheduledReport(int scheduleId)
        {
            Models.Report.Schedule schedule = await Models.Report.Schedule.GetScheduleById(scheduleId);
            Models.Response deleteUserNotifications = await Models.Report.Schedule.UserNotifications.DeleteUsersByScheduleId(scheduleId);

            if (deleteUserNotifications.Success)
            {
                Models.Response deleteScheduleResponse = await Models.Report.Schedule.DeleteScheduleById(scheduleId);
                if (deleteScheduleResponse.Success)
                {
                    HangfireModel.RemoveJob(schedule.HangfireJobId);

                    return Json(new { success = true, responseText = "Success!" });
                }
                else
                {
                    return Json(new { success = false, responseText = deleteScheduleResponse.Message });
                }
            }
            else
            {
                return Json(new { success = false, responseText = deleteUserNotifications.Message });
            }
        }
    }
}