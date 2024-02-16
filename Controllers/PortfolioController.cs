using ArtWebsite.Data;
using ArtWebsite.Models;
using ArtWebsite.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArtWebsite.Controllers
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


        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["StatusMessage"] = "You need to be logged in to perform that action.";
                return LocalRedirect("/Identity/Account/Login");
            }
                

            var user = await _userManager.GetUserAsync(User);
            var userId = user.Id;

            List<Artwork> artworks = _db.Artworks
                .Where(a => a.User.Id == user.Id)
                .ToList();

            List<Story> stories = _db.Stories
                .Where(s => s.User.Id == user.Id)
                .ToList();

            var viewModel = new PortfolioViewModel { Artworks = artworks, Stories = stories };

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
                viewModel.Artwork = await _db.Artworks
                    .Include(a => a.User)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (viewModel.Artwork == null)
                    return LocalRedirect("/Home");

                var user = await _userManager.GetUserAsync(User);

                //Check if liked
                if(user != null)
                {
                    isLiked = _db.Likes.Any(l => l.User.Id == user.Id && l.Artwork.Id == id);
                }

                //Increase view count
                if (user != null && user.Id != viewModel.Artwork.User.Id)
                {
                    // Use a unique key to store whether the user has viewed this content
                    var viewKey = $"{type}-{id}-Viewed-{user.Id}";

                    // Check if the user has not viewed this content
                    if (!HttpContext.Request.Cookies.ContainsKey(viewKey))
                    {
                        // Increment the view count
                        viewModel.Artwork.Views++;
                        await _db.SaveChangesAsync();

                        // Set a cookie to track that the user has viewed this content
                        HttpContext.Response.Cookies.Append(viewKey, "true", new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(5) // Set the expiration as needed
                        });
                    }
                }


            }
            else if (type == "Story" || type == "story")
            {
                viewModel.Story = await _db.Stories.
                    Include(s => s.User)
                    .FirstOrDefaultAsync(s => s.Id == id);

                var user = await _userManager.GetUserAsync(User);

                //Check if liked
                if (user != null)
                {
                    isLiked = _db.Likes.Any(l => l.User.Id == user.Id && l.Artwork.Id == id);
                }

                //Increase view count
                if (user != null && user.Id != viewModel.Story.User.Id)
                {
                    // Use a unique key to store whether the user has viewed this content
                    var viewKey = $"{type}-{id}-Viewed-{user.Id}";

                    // Check if the user has not viewed this content
                    if (!HttpContext.Request.Cookies.ContainsKey(viewKey))
                    {
                        // Increment the view count
                        viewModel.Story.Views++;
                        await _db.SaveChangesAsync();

                        // Set a cookie to track that the user has viewed this content
                        HttpContext.Response.Cookies.Append(viewKey, "true", new CookieOptions
                        {
                            Expires = DateTime.Now.AddMinutes(5) // Set the expiration as needed
                        });
                    }
                }


                if (viewModel.Story == null)
                    return LocalRedirect("/Home");
            }

            viewModel.StatusMessage = TempData["StatusMessage"] as string;
            viewModel.IsLiked = isLiked;

            return View(viewModel);
        }

        public async Task<IActionResult> EditArt(int id)
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
                    return RedirectToAction("View", new { type = "art", id = existingArtwork.Id });
                }
            }

            // If ModelState is not valid or the artwork doesn't exist, return to the edit page
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditStory(int id)
        {
            var story = await _db.Stories.FirstOrDefaultAsync(s => s.Id == id);

            //If story doesn't exist, exit page
            if (story == null)
                return RedirectToAction("Index");

            //If user didn't upload the story, exit page
            var user = await _userManager.GetUserAsync(User);
            if (story.User.Id != user.Id)
            {
                return RedirectToAction("Index");
            }

            var viewModel = new EditStoryViewModel
            {
                Id = story.Id,
                Title = story.Title,
                Description = story.Description,
                Content = story.Content,
                IsPublished = story.isPublished
            };


            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStory(EditStoryViewModel viewModel)
        {
            if (viewModel.Id == 0)
                return RedirectToAction("Index");

            if(ModelState.IsValid)
            {
                //Retrieve existing story
                var existingStory = await _db.Stories.FindAsync(viewModel.Id);

                if(existingStory != null)
                {
                    //Update properties
                    existingStory.Title = viewModel.Title.Trim('\n', '\r');

                    if(!string.IsNullOrEmpty(viewModel.Description))
                        existingStory.Description = viewModel.Description.Trim('\n', '\r');

                    existingStory.Content = viewModel.Content.Trim('\n', '\r');
                    existingStory.ReadingTime = CalculateReadingTime(existingStory.Content);
                    existingStory.isPublished = (bool) viewModel.IsPublished;

                    await _db.SaveChangesAsync();

                    TempData["StatusMessage"] = "Your changes were saved.";

                    //View the story after edits happen
                    return RedirectToAction("View", new { type = "story", id = existingStory.Id });
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
            if (type == "story" || type == "Story")
            {
                // Retrieve the artwork from the database based on the artworkId
                var story = _db.Stories.Find(id);

                // Query the likes table to see if there's a match for the user and story
                var like = await _db.Likes
                                            .FirstOrDefaultAsync(l => l.User.Id == currentUserId && l.Story.Id == id);

                //If user has liked already, exit method
                if (like != null)
                    return View();

                // User hasn't liked the story, so add a new like
                var newLike = new Like
                {
                    User = user,
                    Story = story
                };

                _db.Likes.Add(newLike);
                story.Likes++;

                // Save changes to the database
                await _db.SaveChangesAsync();

                // Return the updated like count (or any other relevant data)
                return Json(new { likes = story.Likes });
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
            if (type == "story" || type == "Story")
            {
                // Retrieve the artwork from the database based on the artworkId
                var story = _db.Stories.Find(id);

                // User has already liked the artwork, so remove the like
                var likeToRemove = _db.Likes
                    .Single(l => l.User.Id == currentUserId && l.Story.Id == id);

                if (likeToRemove == null)
                    return View();

                _db.Likes.Remove(likeToRemove);
                story.Likes--;
               

                // Save changes to the database
                await _db.SaveChangesAsync();

                // Return the updated like count (or any other relevant data)
                return Json(new { likes = story.Likes });
            }

            return View();
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

            if(type == "story")
            {
                var story = await _db.Stories.FindAsync(id);
                _db.Stories.Remove(story);
                await _db.SaveChangesAsync();

                TempData["StatusMessage"] = "Deletion Successful";

                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }
    }
}
