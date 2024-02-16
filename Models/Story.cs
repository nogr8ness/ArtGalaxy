namespace ArtWebsite.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;


    public class Story
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is a required field.")]
        public string Title { get; set; }

        [Display(Name = "Description (optional)")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Content is a required field.")]
        public string Content { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        [Required(ErrorMessage = "Publish status is required.")] 
        public bool isPublished { get; set; }

        [Required]
        public int ReadingTime { get; set; }

        public int Views { get; set; }

        public int Likes { get; set; }


        //Foreign Keys

        [Required]
        public User User { get; set; }

        public ICollection<Comment> Comments { get; set; }
    }
}

