using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageStoreAndAnalyze.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ImagesController : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Images(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
    }
}
