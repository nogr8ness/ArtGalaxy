using ArtGalaxy.Data;
using ArtGalaxy.Models;
using ArtGalaxy.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtGalaxy.Controllers
{
    public class PortfolioController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<User> _userManager;

        public PortfolioController(ApplicationDbContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager; 
        }

        public async Task<IActionResult> Index(string name)
        {
            var existingUser = _db.Users.Where(u => u.DisplayName == name);

            //If user does not exist, go back to home page 
            if (!existingUser.Any())
                return LocalRedirect("/Home");

            var user = await _userManager.GetUserAsync(User);

            //If user is accessing their own portfolio, load all items. Else, only load published items
            List<Artwork> artworks = new List<Artwork>();
            List<Literature> stories = new List<Literature>();
            bool isSelf = false;

            if(user != null && name == user.DisplayName)
            {
                artworks = _db.Artworks
                    .Where(a => a.User.Id == user.Id)
                    .ToList();

                stories = _db.Stories
                    .Where(s => s.User.Id == user.Id)
                    .ToList();

                isSelf = true;
            }
            else
            {
                artworks = _db.Artworks
                    .Where(a => a.User.DisplayName == name && a.isPublished)
                    .ToList();

                stories = _db.Stories
                    .Where(s => s.User.DisplayName == name && s.isPublished)
                    .ToList();
            }


            var viewModel = new PortfolioViewModel { Artworks = artworks, Stories = stories, 
                PortfolioDisplayName = name, IsSelf = isSelf };

            viewModel.StatusMessage = TempData["StatusMessage"] as string;

            return View(viewModel);
        }

        public async Task<IActionResult> View(string type, int id)
        {
            ViewData["Type"] = type;
            ViewData["Id"] = id;

            DisplayViewModel viewModel = new DisplayViewModel();

            bool isLiked = false;
            
            if(type == "Art" || type == "art")
            {
                var content = await _db.Artworks
                    .Include(a => a.User)
                    .Include(a => a.Comments)
                    .FirstOrDefaultAsync(a => a.Id == id);

                viewModel.Content = content;

                if (viewModel.Content == null)
                    return LocalRedirect("/Home");

                //await LoadReplies(content.Comments);

                viewModel.Type = "Artwork";
                viewModel.Image = content.Image;

                var user = await _userManager.GetUserAsync(User);

                //Check if liked
                if(user != null)
                {
                    isLiked = _db.Likes.Any(l => l.User.Id == user.Id && l.Artwork.Id == id);
                }

                //Increase view count
                if (user != null && user.Id != viewModel.Content.User.Id)
                {
                    // Use a unique key to store whether the user has viewed this content
                    var viewKey = $"{type}-{id}-Viewed-{user.Id}";

                    // Check if the user has not viewed this content
                    if (!HttpContext.Request.Cookies.ContainsKey(viewKey))
                    {
                        // Increment the view count
                        viewModel.Content.Views++;
                        await _db.SaveChangesAsync();

                        // Set a cookie to track that the user has viewed this content
                        HttpContext.Response.Cookies.Append(viewKey, "true", new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(5) // Set the expiration as needed
                        });
                    }
                }


            }
            else if (type == "Literature" || type == "literature")
            {
                var content = await _db.Stories
                    .Include(s => s.User)
                    .Include(s => s.Comments)
                    .FirstOrDefaultAsync(s => s.Id == id);

                viewModel.Content = content;

                var user = await _userManager.GetUserAsync(User);

                if (viewModel.Content == null)
                    return LocalRedirect("/Home");

                //await LoadReplies(content.Comments);

                viewModel.Type = "Literature";
                viewModel.LiteratureContent = content.Content;
                viewModel.ReadingTime = content.ReadingTime;

                //Check if liked
                if (user != null)
                {
                    isLiked = _db.Likes.Any(l => l.User.Id == user.Id && l.Literature.Id == id);
                }

                //Increase view count
                if (user != null && user.Id != viewModel.Content.User.Id)
                {
                    // Use a unique key to store whether the user has viewed this content
                    var viewKey = $"{type}-{id}-Viewed-{user.Id}";

                    // Check if the user has not viewed this content
                    if (!HttpContext.Request.Cookies.ContainsKey(viewKey))
                    {
                        // Increment the view count
                        viewModel.Content.Views++;
                        await _db.SaveChangesAsync();

                        // Set a cookie to track that the user has viewed this content
                        HttpContext.Response.Cookies.Append(viewKey, "true", new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(5) // Set the expiration as needed
                        });
                    }
                }


                if (viewModel.Content == null)
                    return LocalRedirect("/Home");
            }

            viewModel.StatusMessage = TempData["StatusMessage"] as string;
            viewModel.IsLiked = isLiked;

            return View(viewModel);
        }

        private async Task LoadReplies(IEnumerable<Comment> comments)
        {
            foreach (Comment comment in comments)
            {
                await _db.Entry(comment)
                    .Collection(c => c.Replies)
                    .Query()
                    .Include(r => r.User) // Include the user for each reply
                    .LoadAsync();

                // Recursively load replies for this comment
                await LoadReplies(comment.Replies);
            }
        }

        public async Task<IActionResult> EditArtwork(int id)
        {
            if(!User.Identity.IsAuthenticated)
            {
                TempData["StatusMessage"] = "You need to be logged in to perform that action.";
                return LocalRedirect("/Identity/Account/Login");
            }

            var artwork = await _db.Artworks
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);


            //If art doesn't exist, exit page
            if (artwork == null)
                return RedirectToAction("Index");


            //If user didn't upload the art, exit page
            var user = await _userManager.GetUserAsync(User);
            if(artwork.User.Id != user.Id)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new EditArtViewModel {
                Id = artwork.Id,
                Title = artwork.Title,
                Description = artwork.Description,
                Image = artwork.Image,
                IsPublished = artwork.isPublished
            };

            return View(viewModel);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditArt(EditArtViewModel viewModel)
        {

            if (viewModel.Id == 0)
                return RedirectToAction("Index");

            // Check if ModelState is valid
            if (ModelState.IsValid)
            {
                

                // Retrieve the existing artwork from the database
                var existingArtwork = await _db.Artworks.FindAsync(viewModel.Id);

                // Check if the artwork exists
                if (existingArtwork != null)
                {
                    // Update properties of the existing artwork
                    existingArtwork.Title = viewModel.Title.Trim('\n', '\r');

                    if(!string.IsNullOrEmpty(viewModel.Description))
                        existingArtwork.Description = viewModel.Description.Trim('\n', '\r');

                    existingArtwork.isPublished = (bool) viewModel.IsPublished;

                    // Save changes to the database
                    await _db.SaveChangesAsync();

                    TempData["StatusMessage"] = "Your changes were saved.";

                    // Redirect to the appropriate View page after successful update
                    return RedirectToAction("View", new { type = "Art", id = existingArtwork.Id });
                }
            }

            // If ModelState is not valid or the artwork doesn't exist, return to the edit page
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditLiterature(int id)
        {
            var literature = await _db.Stories.FirstOrDefaultAsync(s => s.Id == id);

            //If literature doesn't exist, exit page
            if (literature == null)
                return RedirectToAction("Index");

            //If user didn't upload the literature, exit page
            var user = await _userManager.GetUserAsync(User);
            if (user == null || literature.User.Id != user.Id)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new EditLiteratureViewModel
            {
                Id = literature.Id,
                Title = literature.Title,
                Description = literature.Description,
                Content = literature.Content,
                IsPublished = literature.isPublished
            };


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLiterature(EditLiteratureViewModel viewModel)
        {
            if (viewModel.Id == 0)
                return RedirectToAction("Index");

            if(ModelState.IsValid)
            {
                //Retrieve existing literature
                var existingLiterature = await _db.Stories.FindAsync(viewModel.Id);

                if(existingLiterature != null)
                {
                    //Update properties
                    existingLiterature.Title = viewModel.Title.Trim('\n', '\r');

                    if(!string.IsNullOrEmpty(viewModel.Description))
                        existingLiterature.Description = viewModel.Description.Trim('\n', '\r');

                    existingLiterature.Content = viewModel.Content.Trim('\n', '\r');
                    existingLiterature.ReadingTime = CalculateReadingTime(existingLiterature.Content);
                    existingLiterature.isPublished = (bool) viewModel.IsPublished;

                    await _db.SaveChangesAsync();

                    TempData["StatusMessage"] = "Your changes were saved.";

                    //View the literature after edits happen
                    return RedirectToAction("View", new { type = "Literature", id = existingLiterature.Id });
                }
            }

            return RedirectToAction("Index");
        }

        private int CalculateReadingTime(string text)
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




        //Add a like

        [HttpPost]
        public async Task<IActionResult> Like(int id, string type)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["StatusMessage"] = "You need to be logged in to perform that action.";
                return LocalRedirect("/Identity/Account/Login");
            }
                

            var currentUserId = user.Id;

            if (type == "art" || type == "Art")
            {
                var artwork = await _db.Artworks.FindAsync(id);

                // Query the likes table to see if there's a match for the user and artwork
                var like = await _db.Likes
                                            .FirstOrDefaultAsync(l => l.User.Id == currentUserId && l.Artwork.Id == id);

                //If user has liked already, exit method
                if (like != null)
                    return View();

                // User hasn't liked the artwork, so add a new like
                var newLike = new Like
                {
                    User = user,
                    Artwork = artwork
                };

                _db.Likes.Add(newLike);
                artwork.Likes++;

                // Save changes to the database
                await _db.SaveChangesAsync();

                // Return the updated like count (or any other relevant data)
                return Json(new { likes = artwork.Likes });
            }
            if (type == "literature" || type == "Literature")
            {
                // Retrieve the artwork from the database based on the artworkId
                var literature = await _db.Stories.FindAsync(id);

                // Query the likes table to see if there's a match for the user and literature
                var like = await _db.Likes
                                            .FirstOrDefaultAsync(l => l.User.Id == currentUserId && l.Literature.Id == id);

                //If user has liked already, exit method
                if (like != null)
                    return View();

                // User hasn't liked the literature, so add a new like
                var newLike = new Like
                {
                    User = user,
                    Literature = literature
                };

                _db.Likes.Add(newLike);
                literature.Likes++;

                // Save changes to the database
                await _db.SaveChangesAsync();

                // Return the updated like count (or any other relevant data)
                return Json(new { likes = literature.Likes });
            }

            return View();
        }

        //Remove a like

        [HttpPost]
        public async Task<IActionResult> Unlike(int id, string type)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                //TempData["StatusMessage"] = "You need to be logged in to perform that action.";
                return LocalRedirect("/Identity/Account/Login");
            }


            var currentUserId = user.Id;

            if (type == "art" || type == "Art")
            {
                var artwork = await _db.Artworks.FindAsync(id);

                // User has already liked the artwork, so remove the like
                var likeToRemove = _db.Likes
                    .Single(l => l.User.Id == currentUserId && l.Artwork.Id == id);

                if (likeToRemove == null)
                    return View();

                _db.Likes.Remove(likeToRemove);
                artwork.Likes--;

                // Save changes to the database
                await _db.SaveChangesAsync();

                // Return the updated like count (or any other relevant data)
                return Json(new { likes = artwork.Likes });
            }
            if (type == "literature" || type == "Literature")
            {
                // Retrieve the artwork from the database based on the artworkId
                var literature = _db.Stories.Find(id);

                // User has already liked the artwork, so remove the like
                var likeToRemove = _db.Likes
                    .Single(l => l.User.Id == currentUserId && l.Literature.Id == id);

                if (likeToRemove == null)
                    return View();

                _db.Likes.Remove(likeToRemove);
                literature.Likes--;
               

                // Save changes to the database
                await _db.SaveChangesAsync();

                // Return the updated like count (or any other relevant data)
                return Json(new { likes = literature.Likes });
            }

            return View();
        }


        //COMMENTS


        //Like a comment

        [HttpPost]
        public async Task<IActionResult> LikeComment(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                TempData["StatusMessage"] = "You need to be logged in to perform that action.";
                return LocalRedirect("/Identity/Account/Login");
            }


            var currentUserId = user.Id;

            var comment = await _db.Comments.FindAsync(id);

            // Query the likes table to see if there's a match for the user and comment
            var like = await _db.Likes.FirstOrDefaultAsync(l => l.User.Id == user.Id && l.Comment.Id == id);

            // If user has already liked the comment, remove the like
            if (like != null)
            {
                _db.Likes.Remove(like);
                comment.Likes--;
            }
            else
            {
                // User hasn't liked the comment, so add a new like
                var newLike = new Like
                {
                    User = user,
                    Comment = comment
                };

                _db.Likes.Add(newLike);
                comment.Likes++;
            }

            // Save changes to the database
            await _db.SaveChangesAsync();

            // Return the updated like count
            return Json(new { likes = comment.Likes });
        }

        //Unlike a comment

        [HttpPost]
        public async Task<IActionResult> UnlikeComment(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                //TempData["StatusMessage"] = "You need to be logged in to perform that action.";
                return LocalRedirect("/Identity/Account/Login");
            }


            var currentUserId = user.Id;

            var comment = await _db.Comments.FindAsync(id);

            // User has already liked the artwork, so remove the like
            var likeToRemove = _db.Likes
                .Single(l => l.User.Id == currentUserId && l.Comment.Id == id);

            if (likeToRemove == null)
                return View();

            _db.Likes.Remove(likeToRemove);
            comment.Likes--;

            // Save changes to the database
            await _db.SaveChangesAsync();

            // Return the updated like count (or any other relevant data)
            return Json(new { likes = comment.Likes });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int id)
        {
            var comment = await _db.Comments.FindAsync(id);

            //Remove comment

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string type, int id)
        {
            if (type == "art")
            {
                var art = await _db.Artworks.FindAsync(id);

                //Remove likes


                _db.Artworks.Remove(art);
                await _db.SaveChangesAsync();


                TempData["StatusMessage"] = "Deletion Successful";

                return RedirectToAction("Index");
            }

            if(type == "literature")
            {
                var literature = await _db.Stories.FindAsync(id);
                _db.Stories.Remove(literature);
                await _db.SaveChangesAsync();

                TempData["StatusMessage"] = "Deletion Successful";

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }


        
    }
}
