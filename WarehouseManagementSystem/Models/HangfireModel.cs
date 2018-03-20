using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using Hangfire;
using Microsoft.AspNet.Identity;
using OfficeOpenXml;
using SendGrid;
using SendGrid.Helpers;
using SendGrid.Helpers.Mail;
using System.Net.Http;


namespace WarehouseManagementSystem.Models
{
    public class HangfireModel
    {
        public static void CreateDelayedJob(string recurringJobId, int scheduleId, DateTime startDate, string frequency)
        {


            double minutesToWait = (startDate - DateTime.Now).Minutes;
            BackgroundJob.Schedule(() => CreateScheduledReport(recurringJobId, scheduleId, frequency), TimeSpan.FromMinutes(minutesToWait));


        }
        public static void CreateScheduledReport(string recurringJobId, int scheduleId, string frequency)
        {
            if (frequency == "Daily")
            {
                RecurringJob.AddOrUpdate(recurringJobId, () => RunScheduledReport(scheduleId), Cron.Daily);
            }
            else if(frequency == "Weekly")
            {
                RecurringJob.AddOrUpdate(recurringJobId, () => RunScheduledReport(scheduleId), Cron.Weekly);
            }
            else if(frequency == "Monthly")
            {
                RecurringJob.AddOrUpdate(recurringJobId, () => RunScheduledReport(scheduleId), Cron.Monthly);
            }

        }
        public static void RemoveJob(string jobId)
        {
            RecurringJob.RemoveIfExists(jobId.ToString().Trim());
        }
        public static async Task RunScheduledReport(int scheduleId)
        {
            //gathers all the data for the attached report
            Models.Report.Schedule schedule = await Models.Report.Schedule.GetScheduleById(scheduleId);
            Models.Report report = await Models.Report.GetReportById(schedule.ReportId);
            List<Models.Report.Schedule.UserNotifications> userNotifications = await Models.Report.Schedule.UserNotifications.GetUserNotificationsByScheduleId(scheduleId);

            schedule.UsersToNotify = userNotifications;
            report._Schedule = schedule;

            //writes report results to an excel worksheet
            List<Excel.Workbook.Worksheet.Row> result = await Database.RunQuery(report.ReportQuery);
            ExcelPackage package = Models.Excel.CreateExcelPackage();

            Models.Excel.Workbook.Worksheet.WriteRowsToWorksheet(package.Workbook.Worksheets[1], result);

            //begins email process
            string key = SendGridModel.GetAPIKey();

            string base64ExcelRepresentation = Excel.ConvertPackageToBase64(package);

            foreach (Report.Schedule.UserNotifications userToNotify in report._Schedule.UsersToNotify)
            {
                var client = new SendGridClient(key);
                var from = new EmailAddress("mcguirepjamessendgrid@gmail.com", "Example User");
                var subject = report.ReportName;
                var to = new EmailAddress(userToNotify.EmailAddress, " ");
                var plainTextContent = "";
                var htmlContent = "<div></div>";
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                msg.AddAttachment("Attachment", base64ExcelRepresentation, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                var response = await client.SendEmailAsync(msg);
            }
        }
        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

    }
}