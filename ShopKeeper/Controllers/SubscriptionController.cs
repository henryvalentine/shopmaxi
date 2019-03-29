using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ShopKeeper.Controllers
{
    public class SubscriptionController : Controller
    {
      
        public ActionResult Subscribers()
        {
            return View();
        }

       
        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Subscription/Create
        public ViewResult subscribe()
        {
            return View();
        }

        //public ActionResult subscribe()
        //{
        //    return View();
        //}

        //
        // POST: /Subscription/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Subscription/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Subscription/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Subscription/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Subscription/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
