using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Books.Models
{
    public class Book
    {
        //public int BookId { get; set; }

        //[Key]
        public string Key { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public double Price { get; set; }


        // public int AuthorId { get; set; }
        public string Author_Key{ get; set; }

        //[ForeignKey("Author_Key")]
        public Author Author { get; set; }


        //public int CategoryId { get; set; }
        public string Category_Key { get; set; }

        //[ForeignKey("Category_Key")]
        public Category Category { get; set; }
    }

    public class Category
    {
        //public int CategoryId { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }

        public List<Book> Books { get; set; }
    }

    public class Author
    {
        //public int AuthorId { get; set; }

        public string Key { get; set; }
        public string Name { get; set; }
        public virtual List<Book> Books { get; set; }
    }
}
