namespace ArtGalaxy.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;

    [Table("Literature")]
    public class Literature : Content
    {

        [Required(ErrorMessage = "Content is a required field.")]
        public string Content { get; set; }

        [Required]
        public int ReadingTime { get; set; }
    }
}

