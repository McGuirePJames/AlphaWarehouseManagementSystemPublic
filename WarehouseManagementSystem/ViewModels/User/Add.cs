using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WarehouseManagementSystem.ViewModels.User
{
    public class Add
    {
        public string Test { get; set; }
        public List<Models.Role> Roles { get; set; }
        public List<Models.Job> Jobs { get; set; }
        public List<Models.Shift> Shifts { get; set; }
    }
}