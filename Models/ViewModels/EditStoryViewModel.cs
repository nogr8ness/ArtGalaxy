using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ArtWebsite.Models.ViewModels
{
    public class EditStoryViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Title is a required field.")]
        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Content is a required field.")]
        [DisplayName("Content")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Publish status is required.")]
        public bool IsPublished { get; set; }
    }
}
