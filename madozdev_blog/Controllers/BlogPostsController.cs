using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using madozdev_blog.Helpers;
using madozdev_blog.Models;
using PagedList;
using PagedList.Mvc;



namespace madozdev_blog.Controllers
{
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts
        public ActionResult Index(int? page,string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogSearch = IndexSearch(searchStr).ToList();

            int pageSize = 2;
            int pageNumber = (page ?? 1);
            return View(blogSearch.OrderByDescending(b =>b.Created).ToPagedList(pageNumber,pageSize));//Allways order before using ToPagedList
        }

        public IQueryable<BlogPost> IndexSearch(string searchStr)
        {
            IQueryable<BlogPost> result = null;
            if(searchStr != null)
            {
                result = db.BlogPosts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchStr) ||
                                      p.Body.Contains(searchStr) ||
                                      p.Comments.Any(c => c.CommentBody.Contains(searchStr) ||
                                                     c.Author.FirstName.Contains(searchStr) ||
                                                     c.Author.LastName.Contains(searchStr)  ||
                                                     c.Author.DisplayName.Contains(searchStr) ||
                                                     c.Author.Email.Contains(searchStr)));
            }
            else
            {
                result = db.BlogPosts.AsQueryable();
            }

            return result.OrderByDescending(p => p.Created);
        }

        // GET: BlogPosts/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BlogPost blogPost = db.BlogPosts.Find(id);
        //    if (blogPost == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(blogPost);
        //}

        public ActionResult Details(string Slug)
        {
            if(String.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug);
            if(blogPost == null)
            {
                return
                HttpNotFound();
            }
            return
            View(blogPost);
        }



        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles ="Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Created,Updated,Title,Abstract,Slug,Body,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if(ImageUploadValidator.IsWebFriendyIMage(image))
                {



                

                if (image != null && image.ContentLength > 0)
                {
                   
                    var fileName = Path.GetFileName(image.FileName);
                    var extension = Path.GetExtension(image.FileName);
                    var newFileName = Path.GetFileNameWithoutExtension(image.FileName) + DateTime.Now.Ticks + extension;


                    var path = Path.Combine(Server.MapPath("~/Uploads"), newFileName);
                    image.SaveAs(path);
                    blogPost.MediaURL = newFileName;
                    
                }

                var Slug =  StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title");
                    return View(blogPost);
                }
                if (db.BlogPosts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique");
                    return View(blogPost);
                }
                blogPost.Slug = Slug;
                blogPost.Created = DateTimeOffset.Now;
               

                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");

              }//End of image validation if statement
              else
                {
                    ModelState.AddModelError("MediaURL", "The photo is too big or is of a wrong type.");
                }

            }

            return View(blogPost);
        }

                             

        // GET: BlogPosts/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    BlogPost blogPost = db.BlogPosts.Find(id);
        //    if (blogPost == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(blogPost);
        //}


        // GET: BlogPosts/Edit/5
        public ActionResult Edit(string slug)
        {
            if (slug == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(b => b.Slug == slug);

            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }


        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Slug,Title,MediaURL,Abstract,Body,Published,Created")] BlogPost blogPost,HttpPostedFileBase MediaURL)
        {
            if (ModelState.IsValid)
            {

                if (MediaURL != null && MediaURL.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(MediaURL.FileName);

                    var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                    MediaURL.SaveAs(path);
                    blogPost.MediaURL = fileName;

                }

                blogPost.Updated = DateTime.Now;
                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
