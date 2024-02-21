using ArtWebsite.Data;
using ArtWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ArtWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public IActionResult Index(string type = "all", string sort = "random")
        {
            IQueryable<IContent> filteredContent;

            // Filter by art/story
            switch (type)
            {
                case "art":
                    filteredContent = _db.Artworks.Select(a => (IContent)a);
                    break;
                case "story":
                    filteredContent = _db.Stories.Select(s => (IContent)s);
                    break;
                default:
                    var artworks = _db.Artworks.Select(artwork => (IContent)artwork).ToList();
                    var stories = _db.Stories.Select(story => (IContent)story).ToList();

                    filteredContent = artworks.Concat(stories).AsQueryable();
                    break;
            }

            // Sorting
            switch (sort)
            {
                case "newest":
                    filteredContent = filteredContent.OrderByDescending(c => c.DatePosted);
                    break;
                case "mostliked":
                    filteredContent = filteredContent.OrderByDescending(c => c.Likes);
                    break;
                default:
                    // Random order
                    filteredContent = filteredContent.OrderBy(a => Guid.NewGuid());
                    break;
            }

            var contentList = filteredContent.ToList();

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}