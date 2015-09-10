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
    public class ProviderApiController : BaseController
    {
        // GET: /Admin/ProviderApi/
        public ActionResult Index()
        {
            var apidetails = db.APIDetails.Include(a => a.Provider).Include(a => a.Category);
            return View(apidetails.ToList());
        }

        // GET: /Admin/ProviderApi/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            APIDetail apidetail = db.APIDetails.Find(id);
            if (apidetail == null)
            {
                return HttpNotFound();
            }
            return View(apidetail);
        }

        // GET: /Admin/ProviderApi/Create
        public ActionResult Create()
        {
            ViewBag.ProviderId = new SelectList(db.Providers, "ProviderId", "Name");
            ViewBag.CategoryId = new SelectList(db.GetCategoriesForAPI(), "CategoryId", "Name");
            return View();
        }

        // POST: /Admin/ProviderApi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(APIDetail apidetail)
        {
            if (ModelState.IsValid)
            {
                apidetail.CreatedOn = System.DateTime.Now;

                db.APIDetails.Add(apidetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProviderId = new SelectList(db.Providers, "ProviderId", "Name", apidetail.ProviderId);
            ViewBag.CategoryId = new SelectList(db.GetCategoriesForAPI(), "CategoryId", "Name", apidetail.CategoryId);
            return View(apidetail);
        }

        // GET: /Admin/ProviderApi/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            APIDetail apidetail = db.APIDetails.Find(id);
            if (apidetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProviderId = new SelectList(db.Providers, "ProviderId", "Name", apidetail.ProviderId);
            ViewBag.CategoryId = new SelectList(db.GetCategoriesForAPI(), "CategoryId", "Name", apidetail.CategoryId);
            return View(apidetail);
        }

        // POST: /Admin/ProviderApi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(APIDetail apidetail)
        {
            if (ModelState.IsValid)
            {
                apidetail.UpdatedOn = System.DateTime.Now;

                db.Entry(apidetail).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProviderId = new SelectList(db.Providers, "ProviderId", "Name", apidetail.ProviderId);
            ViewBag.CategoryId = new SelectList(db.GetCategoriesForAPI(), "CategoryId", "Name", apidetail.CategoryId);
            return View(apidetail);
        }

        // GET: /Admin/ProviderApi/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            APIDetail apidetail = db.APIDetails.Find(id);
            if (apidetail == null)
            {
                return HttpNotFound();
            }
            return View(apidetail);
        }

        // POST: /Admin/ProviderApi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            APIDetail apidetail = db.APIDetails.Find(id);
            db.APIDetails.Remove(apidetail);
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
