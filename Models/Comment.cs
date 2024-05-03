namespace ArtGalaxy.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;


    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        public int Likes { get; set; }


        //Foreign Keys

        [Required]
        public User User { get; set; }

        public Artwork? Artwork { get; set; }

        public Literature? Literature { get; set; }

        //Replies

        public int? ParentId;

        public Comment? Parent { get; set; }

        public ICollection<Comment> Replies { get; set; }
    }
}
