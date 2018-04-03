using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WarehouseManagementSystem.Controllers
{
    public class LinkHelperController : Controller
    {
        public RedirectResult GitHubLink()
        {
            return Redirect("https://github.com/McGuirePJames/WarehouseManagementSystem");
        }
    }
}