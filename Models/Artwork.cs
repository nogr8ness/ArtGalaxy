namespace ArtWebsite.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;


    public class Artwork
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is a required field.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Please upload an image.")]
        public byte[] Image { get; set; }

        [Display(Name = "Description (optional)")]
        public string? Description { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        [Required(ErrorMessage = "Publish status is required.")] 
        public bool isPublished { get; set; }

        public int Views { get; set; }

        public int Likes { get; set; }


        //Foreign Key

        [Required]
        public User User { get; set; }

        public ICollection<Comment> Comments { get; set; }

    }
}
