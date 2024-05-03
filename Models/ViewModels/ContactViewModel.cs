using System.ComponentModel.DataAnnotations;

namespace ArtGalaxy.Models.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "This field is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        [EmailAddress]
        public string Email { get; set; }

        public string? Subject { get; set; }

        [Required(ErrorMessage = "This field is required.")]
        public string Message { get; set; }

        public string? StatusMessage { get; set; }
    }
}
