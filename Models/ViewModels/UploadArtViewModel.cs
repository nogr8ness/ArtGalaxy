using System.ComponentModel.DataAnnotations;

namespace ArtGalaxy.Models.ViewModels
{
    public class UploadArtViewModel
    {
        [Required(ErrorMessage = "Title is a required field.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please upload an image.")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "Description (optional)")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Publish status is required.")]
        public bool IsPublished { get; set; } = true;
    }
}
