using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace madozdev_blog.Models
{
    public class Comment
    {
            
        public int id { get; set; }
        public int BlogPostId { get; set; }
        public string AuthorId { get; set; }
        [AllowHtml]
        public string CommentBody { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public string UpdateReason { get; set; }

        //VIrtual Navigation section
        public virtual BlogPost BlogPost { get; set; }
        public virtual ApplicationUser Author { get; set; }
    
}
}