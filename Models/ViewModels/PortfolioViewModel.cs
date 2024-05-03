namespace ArtGalaxy.Models.ViewModels
{
    public class PortfolioViewModel
    {
        public List<Artwork> Artworks { get; set; }
        public List<Literature> Stories { get; set; }

        public string PortfolioDisplayName { get; set; }

        public bool IsSelf { get; set; }

        public string StatusMessage { get; set; }
    }
}
