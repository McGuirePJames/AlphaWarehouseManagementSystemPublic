using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseManagementSystem.ViewModels.Reports
{
    public class Schedule
    {
        public List<Models.Report> Reports { get; set; }
        public List<Models.User> Users { get; set; }
    }
}