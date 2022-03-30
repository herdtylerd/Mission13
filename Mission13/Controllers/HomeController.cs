using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mission13.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Mission13.Controllers
{
    public class HomeController : Controller
    {
        private IBowlersRepository repo { get; set; }

        public HomeController(IBowlersRepository temp)
        {
            repo = temp;
        }

        public IActionResult Index(int team)
        {
            // Check if filter by team was used
            var bowlers = 
                team == 0 ? repo.Bowlers.ToList()
                : repo.Bowlers.Where(x => x.TeamID == team).ToList();

            ViewBag.TeamName =
                team == 0 ? null
                : repo.Teams.Single(x => x.TeamID == team);

            return View(bowlers);
        }

        [HttpGet]
        public IActionResult Bowler(int bowlerId)
        {
            if (bowlerId == 0) // Add a bowler
            {
                
                ViewBag.Id = bowlerId;

                ViewBag.Teams = repo.Teams.ToList();

                return View(new Bowler());
            }
            else // Edit a bowler
            {
                ViewBag.Id = bowlerId;
                ViewBag.Teams = repo.Teams.ToList();
                var bowler = repo.Bowlers.Single(x => x.BowlerID == bowlerId);

                return View(bowler);
            }
            
        }

        [HttpPost]
        public IActionResult Bowler(Bowler b)
        {
            if (b.BowlerID == 0)
            {
                if (ModelState.IsValid)
                {
                    var maxId = repo.Bowlers.Max(x => x.BowlerID);
                    b.BowlerID = maxId + 1; // Add one to the current highest ID
                    repo.CreateBowler(b);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Teams = repo.Teams.ToList();
                    ViewBag.Id = 0;
                    return View();
                }
            }
            else
            {
                if (ModelState.IsValid)
                {
                    repo.SaveBowler(b);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Teams = repo.Teams.ToList();
                    return View();
                }
            }
            
        }

        public IActionResult Delete(int bowlerId)
        {
            var b = repo.Bowlers.Single(x => x.BowlerID == bowlerId);
            repo.DeleteBowler(b);
            return RedirectToAction("Index");
        }
    }
}
