
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using ArtGalaxy.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace ArtGalaxy.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;

        private readonly UserManager<User> _userManager;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public ConfirmEmailModel(
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ProviderDisplayName { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        public string StatusMessage { get; set; }

        public string _userId;

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>

            [Display(Name = "Bio - tell us about yourself!")]
            public string Bio { get; set; }

            public IFormFile ProfilePic { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string userId, string code, string returnUrl = null) {

            if(userId == null || code == null)
            {
                return RedirectToPage("/Home");
            }

            var user = await _userManager.FindByIdAsync(userId);

            if(user == null)
            {
                return RedirectToPage("/Home");
            }

            //Confirm email
            user.EmailConfirmed = true;
            var result = await _userManager.UpdateAsync(user);

            if(result.Succeeded)
            {
                StatusMessage = "Welcome! Your email has been confirmed. Feel free to enhance your profile by adding a short bio " +
                "and profile picture. These are optional and can be updated later in your Account Settings.";

                _userId = userId;

                return Page();
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return RedirectToPage("/Home");
            }
            
        }
        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
                var userIdFromForm = Request.Form["userId"];
                var user = await _userManager.FindByIdAsync(userIdFromForm);

                user.Bio = Input.Bio;
                //user.ProfilePic = Input.ProfilePic;

                if (Input.ProfilePic != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        await Input.ProfilePic.CopyToAsync(ms);
                        user.ProfilePic = ms.ToArray();
                    }
                }
                else
                {
                    string imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "blank-profile-pic.png");
                    byte[] imageData = System.IO.File.ReadAllBytes(imagePath);

                    using (MemoryStream ms = new MemoryStream(imageData))
                    {
                        //Input.ProfilePic = new FormFile(ms, 0, ms.Length, "blank-profile-pic.png", "blank-profile-pic.png");
                        user.ProfilePic = ms.ToArray();
                    }
                }


                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                    
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ReturnUrl = returnUrl;
            return RedirectToPage("./Home");

        }


    }
}

