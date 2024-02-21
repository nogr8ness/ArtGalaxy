namespace ArtWebsite.Models
{
    public interface IContent
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime DatePosted { get; set; }
        public bool isPublished { get; set; }
        public int Views { get; set; }
        public int Likes { get; set; }
        public User User { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
