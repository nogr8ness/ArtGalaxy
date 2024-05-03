using System.ComponentModel.DataAnnotations;

namespace ArtGalaxy.Models.ViewModels
{
    public class DisplayViewModel
    {

        public Content Content { get; set; }

        public string Type { get; set; }

        //Artwork specific
        public byte[] Image { get; set; }

        //Literature specific
        public string LiteratureContent { get; set; }
        public int ReadingTime { get; set; }

        public string StatusMessage { get; set; }

        public bool IsLiked { get; set; }

        [Required(ErrorMessage = "Comment is a required field.")]
        public string NewCommentText { get; set; }

        public Comment ParentComment { get; set; }
    }
}
