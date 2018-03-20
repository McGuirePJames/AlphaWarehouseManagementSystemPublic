using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseManagementSystem.ViewModels.User
{
    public class Manage
    {
        public List<Models.User> Users { get; set; }
        public List<Models.Job> Jobs { get; set; }
        public List<Models.Shift> Shifts { get; set; }
    }
}