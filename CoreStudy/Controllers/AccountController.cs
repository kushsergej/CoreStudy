using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreStudy.Models;
using CoreStudy.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace CoreStudy.Controllers
{
    public class AccountController : Controller
    {
        #region DI
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IEmailSender emailSender;

        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailSender = emailSender;
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


        // GET: Account/Login
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }


        // POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email", "Password", "RememberMe")] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                SignInResult result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Invalid Login/Password");
                }
            }

            return View(model);
        }


        // GET: Account/LogOff
        [HttpGet]
        public async Task<IActionResult> LogOff()
        {
            //clear authentication cookies for current IdentityUser
            await signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }


        // GET: Account/ForgetPassword
        [HttpGet]
        public async Task<IActionResult> ForgetPassword()
        {
            return View();
        }


        // POST: Account/ForgetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword([Bind("Email")] ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = await userManager.FindByNameAsync(model.Email);

                if (user != null)
                {
                    string token = await userManager.GeneratePasswordResetTokenAsync(user);
                    string callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);

                    await emailSender.SendAsync(model.Email, "Reset password", $"Link to {callbackUrl}");

                    return View();
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "There is no User with such email");
                }
            }

            return View(model);
        }
    }
}