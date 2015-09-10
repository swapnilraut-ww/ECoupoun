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
    public class ProviderController : BaseController
    {
        // GET: /Admin/Provider/
        public ActionResult Index()
        {
            var providers = db.Providers.Include(p => p.ProviderPriority);
            return View(providers.ToList());
        }

        // GET: /Admin/Provider/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Provider provider = db.Providers.Find(id);
            if (provider == null)
            {
                return HttpNotFound();
            }
            return View(provider);
        }

        // GET: /Admin/Provider/Create
        public ActionResult Create()
        {
            ViewBag.ProviderId = new SelectList(db.ProviderPriorities, "ProviderId", "ProviderId");
            return View();
        }

        // POST: /Admin/Provider/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Provider provider)
        {
            if (ModelState.IsValid)
            {
                provider.CreatedOn = System.DateTime.Now;

                db.Providers.Add(provider);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProviderId = new SelectList(db.ProviderPriorities, "ProviderId", "ProviderId", provider.ProviderId);
            return View(provider);
        }

        // GET: /Admin/Provider/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Provider provider = db.Providers.Find(id);
            if (provider == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProviderId = new SelectList(db.ProviderPriorities, "ProviderId", "ProviderId", provider.ProviderId);
            return View(provider);
        }

        // POST: /Admin/Provider/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Provider provider)
        {
            if (ModelState.IsValid)
            {
                provider.UpdatedOn = System.DateTime.Now;

                db.Entry(provider).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProviderId = new SelectList(db.ProviderPriorities, "ProviderId", "ProviderId", provider.ProviderId);
            return View(provider);
        }

        // GET: /Admin/Provider/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Provider provider = db.Providers.Find(id);
            if (provider == null)
            {
                return HttpNotFound();
            }
            return View(provider);
        }

        // POST: /Admin/Provider/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Provider provider = db.Providers.Find(id);
            db.Providers.Remove(provider);
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
