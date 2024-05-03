namespace ArtGalaxy.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.AspNetCore.Identity;


    public class Artwork : Content
    {
 

        [Required(ErrorMessage = "Please upload an image.")]
        public byte[] Image { get; set; }

    }
}
