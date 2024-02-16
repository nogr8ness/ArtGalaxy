namespace ArtWebsite.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Like
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime LikedAt { get; set; } = DateTime.Now;

        //Foreign Keys
        [Required]
        public User User { get; set; }

        public int? ArtworkId { get; set; }

        public Artwork? Artwork { get; set; }

        public int? StoryId { get; set; }

        public Story? Story { get; set; }
    }
}
