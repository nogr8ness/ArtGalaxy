using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ArtWebsite.Models.ViewModels
{
    public class EditArtViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Title is a required field.")]
        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Description")]
        public string? Description { get; set; }

        public byte[]? Image { get; set; }

        [Required(ErrorMessage = "Publish status is required.")]
        public bool IsPublished { get; set; }
    }
}
