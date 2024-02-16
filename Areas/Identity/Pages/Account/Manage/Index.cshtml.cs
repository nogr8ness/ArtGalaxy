// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using ArtWebsite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArtWebsite.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Display(Name = "Current Email")]
        public string Email { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public string Base64ProfilePic { get; set; }

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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            
            [Display(Name = "Display Name (Username)")]
            public string DisplayName { get; set; }

            [Display(Name = "Bio - tell us about yourself!")]
            public string Bio { get; set; }

            public IFormFile ProfilePic { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var email = await _userManager.GetEmailAsync(user);
            var displayName = user.DisplayName;
            var bio = user.Bio;

            Email = email;


            Input = new InputModel
            {
                DisplayName = displayName,
                Bio = bio,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            string x = Base64ProfilePic;
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var _bio = user.Bio;
            if(Input.Bio != _bio)
            {
                user.Bio = Input.Bio;
                var setBio = await _userManager.UpdateAsync(user);
                if(!setBio.Succeeded)
                {
                    StatusMessage = "Something went wrong with updating your bio. Please try again.";
                    return RedirectToPage();
                }
            }

            //THIS IS HAVING ERRORS - FIX THIS
            if (Input.ProfilePic != null)
            {
                using (var ms = new MemoryStream())
                {
                    await Input.ProfilePic.CopyToAsync(ms);
                    user.ProfilePic = ms.ToArray();
                }

                var setProfilePic = await _userManager.UpdateAsync(user);
                if (!setProfilePic.Succeeded)
                {
                    StatusMessage = "Something went wrong with updating your profile picture. Please try again.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
