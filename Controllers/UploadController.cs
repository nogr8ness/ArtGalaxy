using ArtWebsite.Data;
using ArtWebsite.Models;
using ArtWebsite.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ArtWebsite.Controllers
{
    public class UploadController : Controller
    {
        private ApplicationDbContext _db;
        private UserManager<User> _userManager;

        public UploadController(ApplicationDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        //GET
        public IActionResult Artwork()
        {
            if (!User.Identity.IsAuthenticated)
                return LocalRedirect("/Identity/Account/Login");

            return View();
        }

        public IActionResult Story()
        {
            if (!User.Identity.IsAuthenticated)
                return LocalRedirect("/Identity/Account/Login");

            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Artwork(UploadArtViewModel model)
        {

            if (ModelState.IsValid)
            {
                Artwork art = new Artwork();

                art.User = await _userManager.GetUserAsync(User);

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        await model.ImageFile.CopyToAsync(ms);
                        art.Image = ms.ToArray();
                    }
                }

                art.Title = model.Title.Trim('\n', '\r');

                if(!string.IsNullOrEmpty(model.Description))
                    art.Description = model.Description.Trim('\n', '\r');

                art.isPublished = model.IsPublished;

                art.DatePosted = DateTime.Now;

                _db.Artworks.Add(art);
                _db.SaveChanges();

                TempData["StatusMessage"] = "Upload Successful!";

                return LocalRedirect($"/View/Art/{art.Id}");
            }

            

            return View(model);
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Story(UploadStoryViewModel model)
        {
            if(ModelState.IsValid)
            {
                Story story = new Story();

                story.User = await _userManager.GetUserAsync(User);
                story.DatePosted = DateTime.Now;

                story.Title = model.Title.Trim('\n', '\r');

                if(!string.IsNullOrEmpty(model.Description))
                    story.Description = model.Description.Trim('\n', '\r');

                story.Content = model.Content.Trim('\n', '\r');
                story.isPublished = model.IsPublished;

                story.ReadingTime = CalculateReadingTime(story.Content);

                _db.Stories.Add(story);
                _db.SaveChanges();

                TempData["StatusMessage"] = "Upload Successful!";

                return LocalRedirect($"/View/Story/{story.Id}");
            }

            return View(model);
        }

        public int CalculateReadingTime(string text)
        {
            //Find word count for est reading time
            int wordCount = 0, index = 0;

            // skip whitespace until first word
            while (index < text.Length && char.IsWhiteSpace(text[index]))
                index++;

            while (index < text.Length)
            {
                // check if current char is part of a word
                while (index < text.Length && !char.IsWhiteSpace(text[index]))
                    index++;

                wordCount++;

                // skip whitespace until next word
                while (index < text.Length && char.IsWhiteSpace(text[index]))
                    index++;
            }

            return wordCount / 200;
        }
    }
}
