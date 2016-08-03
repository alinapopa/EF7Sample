using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Books.Models;
using Microsoft.EntityFrameworkCore;  //needed for Include; previously Microsoft.Data.Entity;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Books.Controllers
{
    public class BooksController : Controller
    {

        private BooksContext _context;

        public BooksController(BooksContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var books = _context.Books
                .Include(a => a.Author)
                .Include(c => c.Category);

            var booklist = books.ToList();

            var categories = _context.Categories.ToList();
            ViewBag.categories = categories;

            var authors = _context.Authors.ToList();
            ViewBag.authors = authors;

            return View(booklist);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Book book)
        {
            if (ModelState.IsValid)
            {
                var authors = _context.Authors.Where(a => a.Name.Equals(book.Author.Name));
                Author author = null;
                if (authors.Any())
                    author = authors.First();
                else
                    author = _context.Authors.Add(new Models.Author { Name = book.Author.Name }).Entity;
                book.Author = author;

                var categories = _context.Categories.Where(a => a.Name.Equals(book.Category.Name));
                if (categories.Any())
                    book.Category = categories.First();
                else
                    book.Category = _context.Categories.Add(new Category { Name = book.Category.Name }).Entity;

                _context.Books.Add(book);

                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(book);
        }


        public IActionResult DeleteCategory(string categoryKey)
        {
            var entity = _context.Categories.Where(c => c.Key.Equals(categoryKey)).SingleOrDefault();

            _context.Categories.Remove(entity);
            _context.SaveChanges();


            return RedirectToAction("Index");
        }

        public IActionResult DeleteAuthor(string authorKey)
        {
            var entity = _context.Authors.Where(c => c.Key.Equals(authorKey)).SingleOrDefault();

            _context.Authors.Remove(entity);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
