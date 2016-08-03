using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;


namespace Friends.Models
{
    public class StateListener : IEntityStateListener
    {
        public void StateChanging(InternalEntityEntry entry, EntityState newState)
        {
            if (newState == EntityState.Added)
            {
                Post post = entry.Entity as Post;
                if (post != null)
                {
  //                  post.Content += " StateListener has appended some text!";
                }
            }
        }

        public void StateChanged(InternalEntityEntry entry, EntityState oldState, bool skipInitialFixup, bool fromQuery)
        {
     
        }
    }

    public class MyValidator : RelationalModelValidator
    {
        public MyValidator(
            ILogger<RelationalModelValidator> loggerFactory,
            IRelationalAnnotationProvider relationalExtensions,
            IRelationalTypeMapper typeMapper)
            : base(loggerFactory, relationalExtensions, typeMapper)
        { }

        public override void Validate(IModel model)
        {
            base.Validate(model);

            var longTables = model.GetEntityTypes()
                .Where(e => e.Relational().TableName.Length > 30)
                .ToList();

            if (longTables.Any())
            {
                throw new NotSupportedException(
                    "The following types are mapped to table names that exceed 30 characters; "
                    + string.Join(", ", longTables.Select(e => $"{e.ClrType.Name} ({e.Relational().TableName})")));
            }

            var longColumns = model.GetEntityTypes()
                .SelectMany(e => e.GetProperties())
                .Where(p => p.Relational().ColumnName.Length > 30)
                .ToList();

            if (longColumns.Any())
            {
                throw new NotSupportedException(
                    "The following properties are mapped to column names that exceed 30 characters; "
                    + string.Join(", ", longColumns.Select(p => $"{p.DeclaringEntityType.Name}.{p.Name} ({p.Relational().ColumnName})")));
            }
        }
    }

    public class FriendsContext : DbContext
    {
        private static readonly IServiceProvider _serviceProvider
       = new ServiceCollection()
           .AddEntityFrameworkSqlServer()
           .AddSingleton<IEntityStateListener>(new StateListener())
           .AddScoped<RelationalModelValidator, MyValidator>()
           .BuildServiceProvider();

        public FriendsContext(DbContextOptions<FriendsContext> options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     => optionsBuilder
         .UseInternalServiceProvider(_serviceProvider)
         .UseSqlServer(@"Server = (localdb)\mssqllocaldb; Database=FriendsDb;Trusted_Connection=True;");


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //UserTag
            modelBuilder.Entity<UserTag>()
                .HasKey(t => new { t.UserId, t.TagId });

            modelBuilder.Entity<UserTag>()
                .HasOne(pt => pt.User)
                .WithMany(p => p.UserTags)
                .HasForeignKey(pt => pt.UserId);

            modelBuilder.Entity<UserTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.UserTags)
                .HasForeignKey(pt => pt.TagId);

            //User/Wall one to one relationship
            modelBuilder.Entity<User>()
               .HasOne(p => p.Wall)
               .WithOne(i => i.User)
               .HasForeignKey<Wall>(b => b.UserId)
               .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.SetNull);
        }

        public void SeedData()
        {
            Tag tag1 = Tags.Add(new Tag { Name = "Artist" }).Entity;
            Tag tag2 = Tags.Add(new Tag { Name = "Likes hiking" }).Entity;
            Tag tag3 = Tags.Add(new Tag { Name = "Likes cooking" }).Entity;
            Tag tag4 = Tags.Add(new Tag { Name = "Vegan" }).Entity;

            User mary = Users.Add(new User { Name = "Mary", Wall = new Models.Wall() }).Entity;
            User john = Users.Add(new User { Name = "John", Wall = new Models.Wall(), }).Entity;
            User louise = Users.Add(new User { Name = "Louise", Wall = new Models.Wall() }).Entity;

            UserTag maryTag1 = new UserTag { User = mary, Tag = tag1 };
            UserTag maryTag2 = new UserTag { User = mary, Tag = tag2 };
            UserTag louiseTag2 = new UserTag { User = louise, Tag = tag2 };
            UserTag louiseTag3 = new UserTag { User = louise, Tag = tag3 };

            mary.UserTags.Add(maryTag1);
            mary.UserTags.Add(maryTag2);

            louise.UserTags.AddRange(new UserTag[] { louiseTag2, louiseTag3 });

            Post post1 = new Models.Post { Content = "hello", User = mary, DatePosted = DateTime.Now, Wall = john.Wall };
            Post post2 = new Models.Post { Content = "hi!", User = mary, DatePosted = DateTime.Now, Wall = louise.Wall };
            Post post3 = new Models.Post { Content = "hello again!", User = john, DatePosted = DateTime.Now, Wall = mary.Wall };
            Post post4 = new Models.Post { Content = "post4", User = john, DatePosted = DateTime.Now, Wall = louise.Wall };

            Posts.AddRange(post1, post2, post3, post4);
            UserTags.AddRange(maryTag1, maryTag2, louiseTag2, louiseTag3);
            SaveChanges();
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Wall> Walls { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<UserTag> UserTags { get; set; }
        public DbSet<SomeTable> SomeTables { get; set; }
    }
}
