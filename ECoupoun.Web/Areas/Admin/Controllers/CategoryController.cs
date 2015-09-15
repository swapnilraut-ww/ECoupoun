using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ECoupoun.Data;

namespace ECoupoun.Web.Areas.Admin.Controllers
{
    [Authorize]
    public class CategoryController : BaseController
    {
        // GET: /Admin/Category/
        public ActionResult Index(int? id)
        {
            ViewBag.ParentCategoryId = id;
            return View(db.Categories.Where(x => x.CategoryParentId == id).ToList());
        }

        // GET: /Admin/Category/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // GET: /Admin/Category/Create
        public ActionResult Create(int? parentCategoryId)
        {
            Category category = new Category();
            category.CategoryParentId = parentCategoryId;
            ViewBag.CategoryList = new SelectList(db.Categories.ToList(), "CategoryId", "Name", parentCategoryId);
            return View(category);
        }

        // POST: /Admin/Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string ext = System.IO.Path.GetExtension(image.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/Areas/Admin/Uploads"), System.DateTime.Now.Ticks.ToString() + ext);

                    // file is uploaded
                    category.Image = System.IO.Path.GetFileName(path);
                    image.SaveAs(path);
                }

                category.CreatedOn = DateTime.Now;

                db.Categories.Add(category);
                db.SaveChanges();
                return RedirectToAction("Index", new { id = category.CategoryParentId });
            }

            return View(category);
        }

        // GET: /Admin/Category/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }

            ViewBag.CategoryList = new SelectList(db.Categories.ToList(), "CategoryId", "Name", category.CategoryParentId);
            return View(category);
        }

        // POST: /Admin/Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    string tempPath = Server.MapPath("~/Areas/Admin/Uploads/" + category.Image);
                    if (!string.IsNullOrWhiteSpace(category.Image))
                        System.IO.File.Delete(Server.MapPath("~/Areas/Admin/Uploads/" + category.Image));

                    string ext = System.IO.Path.GetExtension(image.FileName);
                    string path = System.IO.Path.Combine(
                                           Server.MapPath("~/Areas/Admin/Uploads"), System.DateTime.Now.Ticks.ToString() + ext);

                    // file is uploaded
                    category.Image = System.IO.Path.GetFileName(path);
                    image.SaveAs(path);
                }

                db.Entry(category).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(category);
        }

        // GET: /Admin/Category/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        // POST: /Admin/Category/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
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

        public ActionResult IsMappingNameAvailable(string mappingName, string InitialMappingName)
        {
            if (mappingName.ToLower() != InitialMappingName.ToLower())
            {
                Category category = db.Categories.Where(x => x.MappingName == mappingName).SingleOrDefault();
                if (category == null)
                    return Json(true, JsonRequestBehavior.AllowGet);
                else
                    return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }

        }
    }
}
