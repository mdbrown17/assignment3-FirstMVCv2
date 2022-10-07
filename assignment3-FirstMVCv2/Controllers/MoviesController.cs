using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using assignment3_FirstMVCv2.Data;
using assignment3_FirstMVCv2.Models;
using Assignment3_FirstMVCv2.Models;
using Tweetinvi;
using VaderSharp2;

namespace assignment3_FirstMVCv2.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MoviesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> GetPoster(int id)
        {
            var movie = await _context.Movie
                 .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            var imageData = movie.Poster;

            return File(imageData, "image/jpg");
        }

        // GET: Movies
        public async Task<IActionResult> Index()
        {
            return View(await _context.Movie.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            MovieDetailsVM movieDetailsVM = new MovieDetailsVM();
            movieDetailsVM.movie = movie;
            movieDetailsVM.Tweets = new List<MovieTweets>();

            var userClient = new TwitterClient("w5bxWbjaGX5SVQadf5BydfKxU", "Bpg9117FFprnciUGb4b0q4I1fnbb3fN8lpSzS7N4vJInCpNxRw", "1577812753317875712-on1iqcHDltw2Z0ESzXPDw1jV63803B", "4mp0ICs4Xht4hCZcXvOs23BOyKRYF0cR8zMsBb5b3ymGR");
            var searchResponse = await userClient.SearchV2.SearchTweetsAsync(movie.Title);
            var tweets = searchResponse.Tweets;
            var analyzer = new SentimentIntensityAnalyzer();

            foreach (var tweetText in tweets)
            {
                var tweet = new MovieTweets();
                tweet.TweetText = tweetText.Text;
                var results = analyzer.PolarityScores(tweet.TweetText);
                tweet.Sentiment = results.Compound;
                movieDetailsVM.Tweets.Add(tweet);
            }

            return View(movieDetailsVM);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,ImdbHyperlink,Genre,RealeaseYear")] Movie movie, IFormFile Poster)
        {
            if (ModelState.IsValid)
            {
                if (Poster != null && Poster.Length > 0)
                {
                    var memoryStream = new MemoryStream();
                    await Poster.CopyToAsync(memoryStream);
                    movie.Poster = memoryStream.ToArray();
                }
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,ImdbHyperlink,Genre,RealeaseYear,Poster")] Movie movie)
        {
            if (id != movie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movie == null)
            {
                return NotFound();
            }

            var movie = await _context.Movie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movie == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Movie'  is null.");
            }
            var movie = await _context.Movie.FindAsync(id);
            if (movie != null)
            {
                _context.Movie.Remove(movie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
            return _context.Movie.Any(e => e.Id == id);
        }
    }
}
