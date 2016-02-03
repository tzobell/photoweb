using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using photo.Models;
using System.Text.RegularExpressions;

namespace photo.Controllers
{
    public class PhotographController : Controller
    {
        private PhotographsDBEntities db = new PhotographsDBEntities();

        // GET: /Photograph/
        public ActionResult Index()
        {
            return View(db.PhotoInfoes.ToList());
        }

        // GET: /Photograph/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhotoInfo photoinfo = db.PhotoInfoes.Find(id);
            if (photoinfo == null)
            {
                return HttpNotFound();
            }
            return View(photoinfo);
        }

        // GET: /Photograph/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Photograph/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FileName,Name,Info")] PhotoInfo photoinfo)
        {
            if (ModelState.IsValid)
            {
                db.PhotoInfoes.Add(photoinfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(photoinfo);
        }

        // GET: /Photograph/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhotoInfo photoinfo = db.PhotoInfoes.Find(id);
            if (photoinfo == null)
            {
                return HttpNotFound();
            }
            return View(photoinfo);
        }

        public ActionResult FillDB()
        {

            DirectoryInfo directory = new DirectoryInfo(Server.MapPath(@"~\Images"));
            var imgs = directory.GetFiles().ToList();
            int x = 1;
            foreach (FileInfo i in imgs)
            {
                PhotoInfo phi = new PhotoInfo();
                phi.Id = x++;
                phi.FileName = i.ToString();
                Match m = Regex.Match(i.ToString(), @"[a-zA-Z0-9\s]*");
                if (m.Success)
                {
                    string n = m.Value;
                    phi.Name = n;
                }
                else { phi.Name = i.ToString(); }

                db.PhotoInfoes.Add(phi);
                db.SaveChanges();
            }



            return Redirect("Index");
        }

        // POST: /Photograph/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FileName,Name,Info")] PhotoInfo photoinfo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(photoinfo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(photoinfo);
        }

        // GET: /Photograph/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PhotoInfo photoinfo = db.PhotoInfoes.Find(id);
            if (photoinfo == null)
            {
                return HttpNotFound();
            }
            return View(photoinfo);
        }

        // POST: /Photograph/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PhotoInfo photoinfo = db.PhotoInfoes.Find(id);
            db.PhotoInfoes.Remove(photoinfo);
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
