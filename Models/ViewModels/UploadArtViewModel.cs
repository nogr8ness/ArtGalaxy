using System.ComponentModel.DataAnnotations;

namespace ArtWebsite.Models.ViewModels
{
    public class UploadArtViewModel
    {
        [Required(ErrorMessage = "Title is a required field.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please upload an image.")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "Description (optional)")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Publish status is required.")]
        public bool IsPublished { get; set; } = true;
    }
}
