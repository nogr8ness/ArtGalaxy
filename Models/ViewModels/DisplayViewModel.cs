namespace ArtWebsite.Models.ViewModels
{
    public class DisplayViewModel
    {

        public Artwork Artwork { get; set; }
        public Story Story { get; set; }

        public string StatusMessage { get; set; }

        public bool IsLiked { get; set; }

    }
}
