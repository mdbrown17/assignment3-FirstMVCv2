using System.ComponentModel.DataAnnotations.Schema;

namespace assignment3_FirstMVCv2.Models
{
    public class ActorMovie
    {
        public int Id { get; set; }
        [ForeignKey("Actor")]
        public int ActorId { get; set; }
        public Actor? actor { get; set; }

        [ForeignKey("Movie")]
        public int MovieId { get; set; }
        public Movie? movie { get; set; }

    }
}
