using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Friends.Models;
using Microsoft.EntityFrameworkCore;  //needed for Include; previously Microsoft.Data.Entity;


namespace Friends.Controllers
{
    public class FriendsController : Controller
    {

        private FriendsContext _context;

        public FriendsController(FriendsContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var people = _context.Users
                .Include(a => a.Wall).ThenInclude(b => b.Posts)
                .Include(a => a.Posts)
                .Include(a => a.UserTags).ThenInclude(c => c.Tag);
   
            var peopleList = people.ToList();

            return View(peopleList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Post post)
        {
            if (ModelState.IsValid)
            {
                post.DatePosted = DateTime.Now;

                var authors = _context.Users.Where(u => u.Name.Equals(post.User.Name));
                User author = null;
                if (authors.Any())
                    author = authors.First();
                else
                    author = _context.Users.Add(new Models.User { Name = post.Wall.User.Name, Wall = new Models.Wall() }).Entity;
                post.User = author;

                var walls = _context.Walls.Where(w => w.User.Name.Equals(post.Wall.User.Name));
                if (walls.Any())
                    post.Wall = walls.First();
                else
                {
                    User newUser = _context.Users.Add(new Models.User { Name = post.Wall.User.Name, Wall = new Models.Wall() }).Entity;
                    post.Wall = newUser.Wall;
                }
                _context.Posts.Add(post);

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(post);
        }


        public IActionResult DeleteUser(int? userId)
        {
            var entity = _context.Users
                .Include( w => w.Wall)
                .Where(c => c.UserId == userId).SingleOrDefault();

            var wall = entity.Wall;
            if (entity == null)
            {
                //       return new HttpStatusCodeResult(404);
            }

            _context.Users.Remove(entity);

            if (wall != null)
            {
                _context.Walls.Remove(wall);
            }
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
