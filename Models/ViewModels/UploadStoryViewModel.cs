using System.ComponentModel.DataAnnotations;

namespace ArtWebsite.Models.ViewModels
{
    public class UploadStoryViewModel
    {

        [Required(ErrorMessage = "Title is a required field.")]
        public string Title { get; set; }

        [Display(Name = "Description (optional)")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Content is a required field.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Publish status is required.")]
        public bool IsPublished { get; set; } = true;
    }
}
