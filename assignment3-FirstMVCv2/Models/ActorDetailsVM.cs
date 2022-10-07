using assignment3_FirstMVCv2.Models;

namespace Assignment3_FirstMVC.Models
{
    public class ActorDetailsVM
    {
        public Actor actor { get; set; }
        public List<ActorTweets>? Tweets { get; set; }
        public double AverageTweetSentiment()
        {
            if (Tweets == null) return 0;

            int validTweets = 0;
            double totalTweetScore = 0;
            foreach (ActorTweets tweet in Tweets)
            {
                if (tweet.Sentiment != 0)
                {
                    validTweets++;
                    totalTweetScore += tweet.Sentiment;
                }
            }
            return totalTweetScore / validTweets;
        }
    }
}
