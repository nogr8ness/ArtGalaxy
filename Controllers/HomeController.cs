using ArtGalaxy.Data;
using ArtGalaxy.Models;
using ArtGalaxy.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using SendGrid;
using System.Diagnostics;
using Content = ArtGalaxy.Models.Content;
using Microsoft.AspNetCore.Identity;

namespace ArtGalaxy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, IConfiguration configuration, UserManager<User> userManager)
        {
            _logger = logger;
            _db = db;
            _configuration = configuration;
            _userManager = userManager;
        }


        public IActionResult Index(string type = "all", string sort = "random", int page = 1, int pageSize = 25, string query = "")
        {            
            if(page <= 0)
            {
                page = 1;
            }

            IQueryable<Content> filteredContent;

            // Filter by art/literature - NAMES SHOULD ALWAYS MATCH WHAT'S IN FRONTEND
            switch (type)
            {
                case "artworks":
                    filteredContent = _db.Artworks.Where(a => a.isPublished)
                        .Include(a => a.User)
                        .Include(a => a.Comments)
                        .Select(a => (Content)a);
                    break;
                case "stories":
                    filteredContent = _db.Stories.Where(s => s.isPublished)
                        .Include(s => s.User)
                        .Include(s => s.Comments)
                        .Select(s => (Content)s);
                    break;
                default:
                    var artworks = _db.Artworks.Where(a => a.isPublished)
                        .Include(a => a.User)
                        .Include(a => a.Comments)
                        .Select(artwork => (Content)artwork).ToList();

                    var stories = _db.Stories.Where(s => s.isPublished)
                        .Include(s => s.User)
                        .Include(s => s.Comments)
                        .Select(literature => (Content)literature).ToList();

                    filteredContent = artworks.Concat(stories).AsQueryable();
                    break;
            }

            // Sorting - NAMES SHOULD ALWAYS MATCH WHAT'S IN FRONTEND
            switch (sort)
            {
                case "newest":
                    filteredContent = filteredContent.OrderByDescending(c => c.DatePosted);
                    break;
                case "mostliked":
                    filteredContent = filteredContent.OrderByDescending(c => c.Likes);
                    break;
                case "mostviewed":
                    filteredContent = filteredContent.OrderByDescending(c => c.Views);
                    break;
                default:
                    // Random order
                    filteredContent = filteredContent.OrderBy(a => Guid.NewGuid());
                    break;
            }

            // Handle searches
            if (!string.IsNullOrWhiteSpace(query))
            {
                // Perform case-insensitive search on Title or Description
                filteredContent = filteredContent.Where(c => c.Title.ToLower().Contains(query.ToLower()) ||
                                    !string.IsNullOrEmpty(c.Description) && c.Description.ToLower().Contains(query.ToLower()) ||
                                    c.User.DisplayName.ToLower().Contains(query.ToLower()));
            }


            List<Content> contentList = new List<Content>();

            //Get highest page # based on number of uploads and page size
            int count = filteredContent.Count();
            int maxPage = count / pageSize + 1;
            if (count % pageSize == 0)
                maxPage--;

            if (page > maxPage)
                page = maxPage;

            //Calculate how many uploads to skip based on page # and page size
            int skipCount = (page - 1) * pageSize;

            //Gets content on certain page
            if(filteredContent.Count() > 0)
                contentList = filteredContent.Skip(skipCount).Take(pageSize).ToList();

            
            

            ViewData["PageNumber"] = page;
            ViewData["MaxPage"] = maxPage;

            ViewData["Query"] = query;

            return View(contentList);
        }


        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> Contact()
        {
            var viewModel = new ContactViewModel();
            
            if(User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);

                viewModel.Name = user.DisplayName;
                viewModel.Email = user.NormalizedEmail.ToLower();
            }

            viewModel.StatusMessage = TempData["StatusMessage"] as string;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Contact(ContactViewModel viewModel)
        {
            // Validate the model
            if (!ModelState.IsValid)
            {
                return View(viewModel); // Return the view with validation errors
            }

            // Send email using SendGrid
            var apiKey = _configuration.GetValue<string>("SendGridAPI");
            var client = new SendGridClient(apiKey);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress("info@artgalaxy.io", "Art Galaxy Support Request"),
                Subject = viewModel.Subject,
                HtmlContent = "<b>Email: </b>" + viewModel.Email + "<br><b>Name: </b>" + viewModel.Name + "<br><br><b>Message: </b><br>" + viewModel.Message,
            };

            msg.AddTo(new EmailAddress("help@artgalaxy.io")); // Replace with your email address

            var response = await client.SendEmailAsync(msg);

            // Check if email was sent successfully
            if (response.IsSuccessStatusCode)
            {
                // Email sent successfully
                TempData["StatusMessage"] = "Your message has been successfully sent. " +
                    "Thank you for reaching out to us! We'll get back to you as soon as possible.";

                return RedirectToPage("/Home/Contact"); // Redirect to same page
            }
            else
            {
                // Failed to send email
                TempData["StatusMessage"] = "Failed to send. Please try again later.";
                return View(viewModel); // Return the view with error message
            }
        }
    

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}