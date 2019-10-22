using madozdev_blog.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace madozdev_blog.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public ActionResult Index(int? page, string searchStr)
        {
            ViewBag.Search = searchStr;
            var blogSearch = IndexSearch(searchStr).ToList();

            int pageSize = 2;
            int pageNumber = (page ?? 1);
           // return View(db.BlogPosts.Where(blog => blog.Published).OrderByDescending(blog => blog.Created).ToList());

            return View(blogSearch.Where(blog => blog.Published).OrderByDescending(b => b.Created).ToPagedList(pageNumber, pageSize));
        }

        public IQueryable<BlogPost> IndexSearch(string searchHomeStr)
        {
            IQueryable<BlogPost> result = null;
            if (searchHomeStr != null)
            {
                result = db.BlogPosts.AsQueryable();
                result = result.Where(p => p.Title.Contains(searchHomeStr) ||
                                      p.Body.Contains(searchHomeStr) ||
                                      p.Comments.Any(c => c.CommentBody.Contains(searchHomeStr) ||
                                                     c.Author.FirstName.Contains(searchHomeStr) ||
                                                     c.Author.LastName.Contains(searchHomeStr) ||
                                                     c.Author.DisplayName.Contains(searchHomeStr) ||
                                                     c.Author.Email.Contains(searchHomeStr)));
            }
            else
            {
                result = db.BlogPosts.AsQueryable();
            }

            return result.OrderByDescending(p => p.Created);
        }



        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            EmailModel model = new EmailModel();


            return View(model);
        }

        public ActionResult ContactConfirm()
        {
            


            return View();
        }





        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact(EmailModel model)
        {
           if(ModelState.IsValid)
            {
                try
                {
                    var body = "<p>Email From: <bold>{0}</bold>({1})</p><p>Message:</p><p>{2}</p>";

                    var from = "madozc@gmail.com";
                    model.Body = model.Body;
                    var email = new MailMessage(from, ConfigurationManager.AppSettings["emailto"])
                    {
                      Subject = "Portfolio Contact Email",
                      Body = string.Format(body,model.FromName,model.FromEmail,model.Body),
                      IsBodyHtml = true
                      
                    
                    };

                    var svc = new PersonalEmail();
                    await svc.SendAsync(email);
                    return RedirectToAction("ContactConfirm", "Home");
                    //return View(new EmailModel());
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    await Task.FromResult(0);
                }
            }

            return View(model);
        }
    }
}