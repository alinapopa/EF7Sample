using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Books.Models
{
    public class BooksContext : DbContext
    {
       

        public BooksContext(DbContextOptions<BooksContext> options)
            : base(options)
        {

        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Books1;Trusted_Connection=True;");
        //}


    protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                modelBuilder.Entity(entity.ClrType).HasKey("Key");
            }

            modelBuilder.Entity<Book>()
               .HasOne(pt => pt.Author)
               .WithMany(p => p.Books)
               .HasForeignKey(pt => pt.Author_Key)
                .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);

            modelBuilder.Entity<Book>()
               .HasOne(pt => pt.Category)
               .WithMany(p => p.Books)
               .HasForeignKey(pt => pt.Category_Key)
               .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Cascade);
        }

        public void SeedData()
        {
            Book book1 = new Models.Book { Title = "Wolf Road", DateAdded = DateTime.Now, Price = 20 , Key = "Wolf Road" };

            //var fiction = Categories.Add(new Category { Name = "Fiction" }).Entity;   
            //Error: Unable to create or track an entity of type 'Category' because it has a null primary or alternate key value.
            var fiction = Categories.Add(new Category { Name = "Fiction" , Key = "Fiction" }).Entity;
            Category cat2 = new Category { Name = "Children", Key = "Children" };
            Category cat3 = new Category { Name = "Cooking", Key = "Cooking" };
            Categories.AddRange(cat2, cat3);
            book1.Category = fiction;

            var author1 = Authors.Add(new Models.Author { Name = "Beth Lewis", Key = "Beth Lewis" }).Entity;
            Authors.Add(new Models.Author { Name = "Unknown", Key = "Unknown" });
            book1.Author = author1;

            Books.Add(book1);

            SaveChanges();
        }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}

