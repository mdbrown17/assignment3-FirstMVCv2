﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using assignment3_FirstMVCv2.Data;
using assignment3_FirstMVCv2.Models;
using Assignment3_FirstMVC.Models;
using Tweetinvi;
using VaderSharp2;

namespace assignment3_FirstMVCv2.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActorsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> GetPhoto(int id)
        {
            var actor = await _context.Actor
                 .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }
            var imageData = actor.Photo;

            return File(imageData, "image/jpg");
        }
        // GET: Actors
        public async Task<IActionResult> Index()
        {
              return View(await _context.Actor.ToListAsync());
        }

        // GET: Actors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Actor == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            ActorDetailsVM actorDetailsVM = new ActorDetailsVM();
            actorDetailsVM.actor = actor;
            actorDetailsVM.Tweets = new List<ActorTweets>();

            var userClient = new TwitterClient("w5bxWbjaGX5SVQadf5BydfKxU", "Bpg9117FFprnciUGb4b0q4I1fnbb3fN8lpSzS7N4vJInCpNxRw", "1577812753317875712-on1iqcHDltw2Z0ESzXPDw1jV63803B", "4mp0ICs4Xht4hCZcXvOs23BOyKRYF0cR8zMsBb5b3ymGR");
            var searchResponse = await userClient.SearchV2.SearchTweetsAsync(actor.Name);
            var tweets = searchResponse.Tweets;
            var analyzer = new SentimentIntensityAnalyzer();

            foreach (var tweetText in tweets)
            {
                var tweet = new ActorTweets();
                tweet.TweetText = tweetText.Text;
                var results = analyzer.PolarityScores(tweet.TweetText);
                tweet.Sentiment = results.Compound;
                actorDetailsVM.Tweets.Add(tweet);
            }

            return View(actorDetailsVM);
        }

        // GET: Actors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Gender,Age,ImdbHyperLink")] Actor actor, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                if (Photo != null && Photo.Length > 0)
                {
                    var memoryStream = new MemoryStream();
                    await Photo.CopyToAsync(memoryStream);
                    actor.Photo = memoryStream.ToArray();
                }
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(actor);
        }

        // GET: Actors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Actor == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return View(actor);
        }

        // POST: Actors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Gender,Age,ImdbHyperLink,Photo")] Actor actor)
        {
            if (id != actor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActorExists(actor.Id))
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
            return View(actor);
        }

        // GET: Actors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Actor == null)
            {
                return NotFound();
            }

            var actor = await _context.Actor
                .FirstOrDefaultAsync(m => m.Id == id);
            if (actor == null)
            {
                return NotFound();
            }

            return View(actor);
        }

        // POST: Actors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Actor == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Actor'  is null.");
            }
            var actor = await _context.Actor.FindAsync(id);
            if (actor != null)
            {
                _context.Actor.Remove(actor);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActorExists(int id)
        {
          return _context.Actor.Any(e => e.Id == id);
        }
    }
}
