using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreStudy.Models;
using Microsoft.AspNetCore.Authorization;

namespace CoreStudy.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: Home/Index/
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}