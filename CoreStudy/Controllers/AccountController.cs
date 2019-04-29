using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreStudy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CoreStudy.Controllers
{
    public class AccountController : Controller
    {
        #region DI
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        #endregion


        // GET: Account/Register
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            return View();
        }


        // POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Email", "Password", "ConfirmPassword")] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser() { Email = model.Email, UserName = model.Email };

                //trying to create and save new IdentityUser inside "Identity" DB
                IdentityResult result = await userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (IdentityError err in result.Errors)
                    {
                        ModelState.AddModelError(String.Empty, err.Description);
                    }
                }
            }

            return View(model);
        }
    }
}