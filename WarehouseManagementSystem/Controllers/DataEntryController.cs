using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace WarehouseManagementSystem.Controllers
{
    public class DataEntryController : Controller
    {
        [Authorize]
        public ActionResult Selection()
        {
            return View(@"~/Views/DataEntry/Selection.cshtml");
        }
        [Authorize]
        public async Task<ActionResult> ProductEntry()
        {
            List<string> products = new List<string>();
            Models.Products productsModel = new Models.Products();
            products = await productsModel.GetProducts();
            return View(@"~/Views/DataEntry/DataEntryForms/ProductEntry.cshtml", products);
        }
    }
}