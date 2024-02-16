namespace ArtWebsite.Models.ViewModels
{
    public class PortfolioViewModel
    {
        public List<Artwork> Artworks { get; set; }
        public List<Story> Stories { get; set; }

        public string StatusMessage { get; set; }
    }
}
