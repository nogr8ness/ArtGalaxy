using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtGalaxy.Models
{
    public class Content
    {
        public Content()
        {
            Comments = new List<Comment>(); // Initialize the Comments collection
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is a required field.")]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Title { get; set; }

        [Display(Name = "Description (optional)")]
        public string? Description { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        [Required(ErrorMessage = "Publish status is required.")]
        public bool isPublished { get; set; }

        public int Views { get; set; }

        public int Likes { get; set; }

        // Navigation property for comments
        public ICollection<Comment> Comments { get; set; }

        //Get number of comments
        public int CommentCount 
        { 
            get
            
            {
                return Comments.Count;
            }
        }


        //Foreign Keys
        [Required]
        public User User { get; set; }
    }
}
