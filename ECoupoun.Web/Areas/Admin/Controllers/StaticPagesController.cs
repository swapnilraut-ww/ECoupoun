using ECoupoun.Common;
using ECoupoun.Common.Enums;
using ECoupoun.Data;
using System.Web.Mvc;
using System.Linq;
using System.Web;
using System.IO;
namespace ECoupoun.Web.Areas.Admin.Controllers
{
    public class StaticPagesController : BaseController
    {
        //
        // GET: /Admin/PageContent/
        public ActionResult Index(string pageName)
        {
            StaticPage page = db.StaticPages.Where(x => x.PageName == pageName).SingleOrDefault();
            if (page != null)
            {
                ViewBag.PageHtml = page.PageHtml;
            }
            ViewBag.PageList = Pages.About.ToSelectList();
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(FormCollection form)
        {
            string pageId = form["PageId"];
            StaticPage page = db.StaticPages.Where(x => x.PageName == pageId).SingleOrDefault();
            if (page == null)
            {
                page = new StaticPage();
                page.PageName = form["PageId"];
                page.PageHtml = form["editor1"];
                db.StaticPages.Add(page);
            }
            else
            {
                page.PageHtml = form["editor1"];
            }

            db.SaveChanges();
            ViewBag.PageList = Pages.About.ToSelectList();
            return View();
        }

        public ActionResult StaticContent()
        {
            PageContent pageContent = db.PageContents.SingleOrDefault();
            if (pageContent == null)
                pageContent = new PageContent();
            return View(pageContent);
        }

        [HttpPost]
        public ActionResult SaveData(HttpPostedFileBase file, PageContent pageContent)
        {
            PageContent pContent = db.PageContents.SingleOrDefault();
            if (pContent == null)
                pContent = new PageContent();

            if (file != null)
            {
                var fileName = Path.GetFileName(file.FileName);
                string path = Server.MapPath("/Uploads/logo/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filePath = System.IO.Path.Combine(path, fileName);
                file.SaveAs(filePath);

                string fl = filePath.Substring(path.LastIndexOf("\\"));
                string[] split = fl.Split('\\');
                string newpath = split[1];
                string imagepath = "/uploads/logo/" + newpath;
                pContent.Logo = imagepath;
            }

            if (!string.IsNullOrWhiteSpace(pageContent.Contact1))
            {
                pContent.Contact1 = pageContent.Contact1;
            }

            if (!string.IsNullOrWhiteSpace(pageContent.Contact2))
            {
                pContent.Contact2 = pageContent.Contact2;
            }

            if (!string.IsNullOrWhiteSpace(pageContent.Facebook))
            {
                pContent.Facebook = pageContent.Facebook;
            }

            if (!string.IsNullOrWhiteSpace(pageContent.Twitter))
            {
                pContent.Twitter = pageContent.Twitter;
            }

            if (!string.IsNullOrWhiteSpace(pageContent.GooglePlus))
            {
                pContent.GooglePlus = pageContent.GooglePlus;
            }

            if (!string.IsNullOrWhiteSpace(pageContent.Email))
            {
                pContent.Email = pageContent.Email;
            }

            if (pContent.PageContentId == 0)
                db.PageContents.Add(pContent);

            db.SaveChanges();
            return RedirectToAction("StaticContent");
        }
    }
}