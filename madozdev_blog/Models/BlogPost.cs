using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace madozdev_blog.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
        public DateTimeOffset Created { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMMM dd yyyy}")]
        public DateTimeOffset? Updated { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public string Slug { get; set; }
        [AllowHtml]
        public string Body { get; set; }
        public string MediaURL { get; set; }
        public bool Published { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }

        public BlogPost()
        {
            this.Comments = new HashSet<Comment>();
        }
    }
}