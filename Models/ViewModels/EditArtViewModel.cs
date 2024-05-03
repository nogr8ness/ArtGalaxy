using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ArtGalaxy.Models.ViewModels
{
    public class EditArtViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Title is a required field.")]
        [DisplayName("Title")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Title { get; set; }

        [DisplayName("Description")]
        public string? Description { get; set; }

        public byte[]? Image { get; set; }

        [Required(ErrorMessage = "Publish status is required.")]
        public bool IsPublished { get; set; }
    }
}
