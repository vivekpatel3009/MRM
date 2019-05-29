using MRM.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MRM.Web.DBMethods;
using MRM.Helper.Helpers;
using System.Data.Entity;
using MRM.DbEntity;
using MRM.Web.Helpers;
using System.IO;

namespace MRM.Web.Controllers
{
    [CustomAuthorize("2")]
    public class FloorController : Controller
    {
        #region Constant
        FloorMethods _FloorMethods = new FloorMethods();
        RoomMeetingManagementDbEntities dc = new RoomMeetingManagementDbEntities();
        #endregion
        // GET: Floor
        #region Index
        public ActionResult Index()
        {         
            try
            {
                ViewBag.message = "succesfully";
                return View();
            }
            catch (Exception ex)
            {
                string filePath = Server.MapPath("~/Rotativa/Bug/Error.txt");
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("Date : " + DateTime.Now.ToString());
                    writer.WriteLine();

                    while (ex != null)
                    {
                        writer.WriteLine(ex.GetType().FullName);
                        writer.WriteLine("Message : " + ex.Message);
                        writer.WriteLine("StackTrace : " + ex.StackTrace);
                        writer.WriteLine("InnerException : " + ex.InnerException);
                        ex = ex.InnerException;
                    }
                }
            }
            return View();
        }
        #endregion 
        #region List
        public ActionResult List()
        {
            try
            {               
                List<ViewModelFloor> Model = new List<ViewModelFloor>();
                Model = _FloorMethods.GetAllFloorList();
                return PartialView("_PartialFloorListView", Model);
            }
            catch (Exception ex)
            {
                string filePath = Server.MapPath("~/Rotativa/Bug/Error.txt");
                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("-----------------------------------------------------------------------------");
                    writer.WriteLine("Date : " + DateTime.Now.ToString());
                    writer.WriteLine();

                    while (ex != null)
                    {
                        writer.WriteLine(ex.GetType().FullName);
                        writer.WriteLine("Message : " + ex.Message);
                        writer.WriteLine("StackTrace : " + ex.StackTrace);
                        writer.WriteLine("InnerException : " + ex.InnerException);
                        ex = ex.InnerException;
                    }
                }
                return RedirectToAction("Index", "Login", new { ReturnUrl = "/Floor" });
            }
        }
        #endregion   
 
        #region new approach
        public ActionResult Create(ViewModelFloor Model)
        {
            ModelState.Clear();
            return View(Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateFloor(ViewModelFloor Model)
        {
            var userDetail = SessionHelper.GetUserDetailFromSession();
            if (userDetail == null) {
              return RedirectToAction("Index", "Login", new { ReturnUrl = "/Floor" });
            }
            var UserId = Convert.ToInt32(userDetail.user.id);
            Guid g = Guid.NewGuid();
            if (ModelState.IsValid)
            {
                Floor fl = new Floor();
                fl.FloorName = Model.FloorName;
                fl.FloorNumber = Model.FloorNumber;
                fl.Description = Model.Description;
                fl.CreatedBy = UserId;
                fl.UpdatedBy = UserId;
                fl.FloorId = g;
                fl.IsActive = true;
                fl.Status = 1;
                fl.CreatedDate = DateTime.Now;
                fl.UpdatedDate = DateTime.Now;
                dc.Floors.Add(fl);
                dc.SaveChanges();

                MRMLog _MRMLog = new MRMLog();
                _MRMLog.UserId = Convert.ToInt32(UserId);
                _MRMLog.Action = "Create";
                _MRMLog.Module = "Floor Create Operation";
                _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has Create Floor having id= " +g;
                _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                _MRMLog.CraetedDate = DateTime.Now;
                dc.MRMLogs.Add(_MRMLog);
                dc.SaveChanges();
                TempData["create"] = "donecrate";
                return RedirectToAction("Index","Floor");
            }
            return View("FloorForm", Model);
        }

        public ActionResult Edit(Guid id)
        {
            if (id == null)
            {
                return null;
            }
            ViewModelFloor Floor = _FloorMethods.GetFloorDetailsById(id);
            if (Floor == null)
            {
                return null;
            }
            return View("FloorForm", Floor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ViewModelFloor Model)
        {
            var userDetail = SessionHelper.GetUserDetailFromSession();
            if (userDetail == null) { return RedirectToAction("Index", "Login", new { ReturnUrl = "/Floor" }); }
            var UserId = Convert.ToInt32(userDetail.user.id);
            if (ModelState.IsValid)
            {
                try
                {
                    var data = dc.Floors.Where(x => x.FloorId == Model.FloorId && x.IsActive == true).FirstOrDefault();
                    if (data != null)
                    {
                        data.FloorName = Model.FloorName;
                        data.FloorNumber = Model.FloorNumber;
                        data.Description = Model.Description;
                        data.CreatedBy = UserId;
                        data.UpdatedBy = UserId;
                        data.FloorId = Model.FloorId;
                        data.CreatedDate = DateTime.Now;
                        data.UpdatedDate = DateTime.Now;
                        data.Status = 1;
                        data.IsActive = true;
                        dc.SaveChanges();

                        MRMLog _MRMLog = new MRMLog();
                        _MRMLog.UserId = Convert.ToInt32(UserId);
                        _MRMLog.Action = "Update";
                        _MRMLog.Module = "Floor Update Operation";
                        _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has Updated Floor having id= " + Model.FloorId;
                        _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                        _MRMLog.CraetedDate = DateTime.Now;
                        dc.MRMLogs.Add(_MRMLog);
                        dc.SaveChanges();
                        TempData["update"] = "doneupdate";
                    }
                    return RedirectToAction("Index","Floor");
                }
                catch
                {
                    return RedirectToAction("Index", "Login", new { ReturnUrl = "/Floor" });
                }
            }
            return View(Model);
        }
        public ActionResult CheckFloorReference(Guid FloorId)
        {
            var data = _FloorMethods.checkFloorReferenceID(FloorId);
            if (data)
            {
                return Json(new { result = "ReferenceExist" });
            }
            else
            {
                return Json(new { result = "ReferenceNotExist" });
            }
        }
        public ActionResult CheckFloorReferenceforDeactive(Guid FloorId)
        {
            var data = _FloorMethods.checkFloorReferenceID(FloorId);
            if (data)
            {
                return Json(new { result = "ReferenceExist" });
            }
            else
            {
                return Json(new { result = "ReferenceNotExist" });
            }
        }
        public ActionResult DeleteFloorReference(Guid FloorId)
        {
            var data = _FloorMethods.DeleteFloorReferenceID(FloorId);
            if (data)
            {
                return Json(new { result = "ReferenceDeleted", url = Url.Action("Index", "Floor") });
            }
            else
            {
                return RedirectToAction("Index", "Login", new { ReturnUrl = "/Floor" });
            }
        }
        public ActionResult DeActivateFloorReference(Guid FloorId)
        {
            var data = _FloorMethods.DeActivateFloorReferenceID(FloorId);
            if (data)
            {
                return Json(new { result = "DeactFloor", url = Url.Action("Index", "Floor") });
            }
            else
            {
                return RedirectToAction("Index", "Login", new { ReturnUrl = "/Floor" });
            }
        }
        public ActionResult ActivateFloorReference(Guid FloorId)
        {
            var data = _FloorMethods.ActivateFloorReferenceID(FloorId);
            if (data)
            {
                return Json(new { result = "actFloor", url = Url.Action("Index", "Floor") });
            }
            else
            {
                return RedirectToAction("Index", "Login", new { ReturnUrl = "/Floor" });
            }
        }

        #endregion
    }
    public class NLogExceptionHandlerAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            // log error to NLog
            base.OnException(filterContext);
        }
    }
}