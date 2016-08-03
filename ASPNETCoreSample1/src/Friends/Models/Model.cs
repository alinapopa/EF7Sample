using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Friends.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public Wall Wall { get; set; }

        public List<Post> Posts { get; set; } = new List<Post>();

        public List<UserTag> UserTags { get; set; } = new List<UserTag>();
    }

    public class Wall
    {
        public int WallId { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }

        public List<Post> Posts { get; set; } = new List<Post>();
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public DateTime DatePosted { get; set; }

        //author of the post
        public int UserId { get; set; }
        public User User { get; set; }

        //wall on which the post is posted
        public int WallId { get; set; }
        public Wall Wall { get; set; }
    }


    public class Tag
    {
        public int TagId { get; set; }

        public string Name { get; set; }

        public List<UserTag> UserTags { get; set; } = new List<UserTag>();
    }

    public class UserTag
    {
        public int UserId { get; set; }
        public User User { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }

    //[Table("ThisTableHasOneVeryCrazySillyLongName")]
    public class SomeTable
    {
        public int SomeTableId { get; set; }

        // [Column("ThisColumnHasOneVeryCrazySillyLongName")]
        public string Name { get; set; }

    }
}
