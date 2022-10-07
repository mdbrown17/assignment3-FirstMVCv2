namespace assignment3_FirstMVCv2.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? ImdbHyperlink { get; set; }
        public string? Genre { get; set; }
        public int? RealeaseYear { get; set; }
        public byte[]? Poster { get; set; }
    }
}

