using ArtGalaxy.Data;
using ArtGalaxy.Models;
using ArtGalaxy.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtGalaxy.Controllers
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

        public IActionResult Literature()
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
        public async Task<IActionResult> Literature(UploadLiteratureViewModel model)
        {
            if(ModelState.IsValid)
            {
                Literature literature = new Literature();

                literature.User = await _userManager.GetUserAsync(User);
                literature.DatePosted = DateTime.Now;

                literature.Title = model.Title.Trim('\n', '\r');

                if(!string.IsNullOrEmpty(model.Description))
                    literature.Description = model.Description.Trim('\n', '\r');

                literature.Content = model.Content.Trim('\n', '\r');
                literature.isPublished = model.IsPublished;

                literature.ReadingTime = CalculateReadingTime(literature.Content);

                _db.Stories.Add(literature);
                _db.SaveChanges();

                TempData["StatusMessage"] = "Upload Successful!";

                return LocalRedirect($"/View/Literature/{literature.Id}");
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

        [HttpPost]
        public async Task<IActionResult> Comment(DisplayViewModel viewModel)
        {
            if(string.IsNullOrWhiteSpace(viewModel.NewCommentText))
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }

            //If user isn't logged in, exit the method
            if(!User.Identity.IsAuthenticated)
            {
                TempData["StatusMessage"] = "Your comment was not added. Please contact us if this keeps occurring.";

                return Redirect(Request.Headers["Referer"].ToString());
            }

            Artwork artwork = null;
            Literature literature = null;

            //Whichever type the upload is, assign the corresponding variable to store in comment
            if(viewModel.Type == "Artwork")
                artwork = await _db.Artworks.FirstOrDefaultAsync(a => a.Id == viewModel.Content.Id);

            if(viewModel.Type == "Literature")
                literature = await _db.Stories.FirstOrDefaultAsync(s => s.Id == viewModel.Content.Id);

            Comment comment = new Comment
            {
                Content = viewModel.NewCommentText,
                DatePosted = DateTime.Now,
                User = await _userManager.GetUserAsync(User),
                Artwork = artwork,
                Literature = literature,
                Parent = viewModel.ParentComment
            };

            _db.Comments.Add(comment);
            _db.SaveChanges();

            TempData["StatusMessage"] = "Your comment was successfully added.";

            return Redirect(Request.Headers["Referer"].ToString());
        }

        // Reply to comment
        [HttpPost]
        public async Task<IActionResult> ReplyToComment(int parentId, string replyText)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["StatusMessage"] = "You need to be logged in to perform that action.";
                return LocalRedirect("/Identity/Account/Login");
            }

            if(string.IsNullOrEmpty(replyText))
            {
                TempData["StatusMessage"] = "Please do not submit a blank reply.";
                return Redirect(Request.Headers["Referer"].ToString());
            }

            var parentComment = await _db.Comments
                .Include(c => c.Artwork) 
                .Include(c => c.Literature)
                .FirstOrDefaultAsync(c => c.Id == parentId);

            // Create a new comment as a reply
            var newComment = new Comment
            {
                Content = replyText,
                DatePosted = DateTime.Now,
                ParentId = parentId,
                Parent = parentComment,
                User = user,
                Artwork = parentComment.Artwork,
                Literature = parentComment.Literature,
            };

            _db.Comments.Add(newComment);
            parentComment.Replies.Add(newComment);

            await _db.SaveChangesAsync();

            TempData["StatusMessage"] = "Your comment was successfully added.";
            return Redirect(Request.Headers["Referer"].ToString()); // Redirect to the appropriate page after replying
        }
    }
}
