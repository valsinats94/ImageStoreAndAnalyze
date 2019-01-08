using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Controllers
{
    [Route("[controller]/[action]")]
    public class RequestsController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Requests(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
    }
}
