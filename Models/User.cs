namespace ArtWebsite.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;


    public class User : IdentityUser
    {

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Required]
        public virtual string DisplayName { get; set; }

        public byte[] ProfilePic { get; set; }

        public string Bio { get; set; }


        //Foreign keys
        public ICollection<Artwork> Artworks { get; set; }

        public ICollection<Story> Stories { get; set; }

        public ICollection<Comment> Comments { get; set; } 
    }
}
