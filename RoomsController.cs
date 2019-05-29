using MRM.Web.DBMethods;
using MRM.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using MRM.DbEntity;
using System.Data.Entity;
using MRM.Helper.Helpers;
using System.Data.Entity.Core.Objects;
using MRM.Web.Helpers;
using System.Collections.Specialized;

namespace MRM.Web.Controllers
{
    [CustomAuthorize("2")]
    public class RoomsController : Controller
    {
        // GET: Rooms
        #region Constant
        RoomMethods _RoomMethods = new RoomMethods();
        //DBContextRoom _db = new DBContextRoom();
        RoomMeetingManagementDbEntities dc = new RoomMeetingManagementDbEntities();
        #endregion
        #region index
        public ActionResult Index(string returnUrl = "")
        {
            try{
                
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
                List<ViewModelRoom> Model = new List<ViewModelRoom>();
                Model = _RoomMethods.GetAllRoomList();
                return PartialView("_PartialRoomListView", Model);
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
                return RedirectToAction("Index", "Login", new { ReturnUrl = "/Rooms" });
            }
        }
        #endregion    
        #region new approch 
        public ActionResult Create(ViewModelRoom Model)
        {
            try
            {
                ViewBag.RoomtypeListVB = dc.RoomTypes.Where(x => x.IsActive == true && x.Status == 1).ToList();
                ViewBag.FloorListVB = dc.Floors.Where(x => x.IsActive == true && x.Status == 1).ToList();
                ModelState.Clear();
                return View(Model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login", new { ReturnUrl = "/Rooms" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRoom(ViewModelRoom Model)
        {
            try
            {
                if (Model.MinCapacity > Model.MaxCapacity) {
                    ViewBag.RoomtypeListVB = dc.RoomTypes.Where(x => x.IsActive == true && x.Status == 1).ToList();
                    ViewBag.FloorListVB = dc.Floors.Where(x => x.IsActive == true && x.Status == 1).ToList();
                    ViewBag.MinMax = "MinMax";
                    return View("Create",Model);
                }
                var userDetail = SessionHelper.GetUserDetailFromSession();
                if (userDetail == null)
                {
                    return RedirectToAction("Index", "Login", new { ReturnUrl = "/Rooms" });
                }

                ObjectParameter returnId = new ObjectParameter("Exists", typeof(int));
                var exist = dc.IsExistRooms(Model.RoomName,Model.RoomNumber, returnId).ToList();
                int Roomexist = Convert.ToInt32(returnId.Value);
                if (Roomexist == 0)
                {
                    var UserId = Convert.ToInt32(userDetail.user.id);
                    Guid g = Guid.NewGuid();
                    if (ModelState.IsValid)
                    {
                        List<ViewModelRoomPictureMapping> fileDetails = new List<ViewModelRoomPictureMapping>();

                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            var file = Request.Files[i];

                            if (file != null && file.ContentLength > 0)
                            {
                                RoomPictureMapping rpc = new RoomPictureMapping();
                                var ActualfileName = Path.GetFileName(file.FileName);
                                var FileTime = DateTime.Now.ToFileTime();
                                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + FileTime + Path.GetExtension(file.FileName);
                                rpc.FileName = fileName;
                                rpc.ActualFileName = ActualfileName;
                                rpc.Extension = Path.GetExtension(fileName);
                                rpc.RoomPictureMappingId = Guid.NewGuid();
                                rpc.RoomId = g;
                                rpc.CreatedDate = DateTime.Now;
                                rpc.UpdatedDate = DateTime.Now;
                                rpc.CreatedBy = UserId;
                                rpc.UpdatedBy = UserId;
                                dc.RoomPictureMappings.Add(rpc);
                                dc.SaveChanges();
                                var path = Path.Combine(Server.MapPath("~/Rotativa/RoomImage/"), Path.GetFileNameWithoutExtension(file.FileName) + "_" + FileTime + Path.GetExtension(file.FileName));
                                file.SaveAs(path);
                            }
                        }
                        Room rm = new Room();
                        rm.RoomId = g;
                        rm.IsActive = true;
                        rm.Status = 1;
                        rm.CreatedDate = DateTime.Now;
                        rm.UpdatedDate = DateTime.Now;
                        rm.FloorId = Model.FloorId;
                        rm.RoomTypeId = Model.RoomTypeId;
                        rm.Capacity = Model.Capacity;
                        rm.MinCapacity = Model.MinCapacity;
                        rm.MaxCapacity = Model.MaxCapacity;
                        rm.RoomNumber = Model.RoomNumber;
                        rm.RoomName = Model.RoomName;
                        rm.Description = Model.Description;
                        rm.CreatedBy = UserId;
                        rm.UpdatedBy = UserId;
                        dc.Rooms.Add(rm);
                        dc.SaveChanges();

                        MRMLog _MRMLog = new MRMLog();
                        _MRMLog.UserId = Convert.ToInt32(UserId);
                        _MRMLog.Action = "Create";
                        _MRMLog.Module = "Room Create Operation";
                        _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has Create Room having id= " + g;
                        _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                        _MRMLog.CraetedDate = DateTime.Now;
                        dc.MRMLogs.Add(_MRMLog);
                        dc.SaveChanges();
                        TempData["create"] = "donecrate";
                        return RedirectToAction("Index", "Rooms");
                    }
                    return View("Create", Model);
                }
                else {
                    ViewBag.RoomtypeListVB = dc.RoomTypes.Where(x => x.IsActive == true && x.Status == 1).ToList();
                    ViewBag.FloorListVB = dc.Floors.Where(x => x.IsActive == true && x.Status == 1).ToList();
                    Model.IsExist = true;
                    ViewBag.IsExistRoom = 1;
                    return View("Create", Model);
                }
            }
            catch(Exception ex) { return RedirectToAction("Index", "Login", new { ReturnUrl = "/Rooms" });
            }
        }

        public ActionResult Edit(Guid id)
        {
            try
            {
                if (id == null)
                {
                    return null;
                }
                ViewModelRoom Room = _RoomMethods.GetRoomDetailsById(id);
              // ViewModelRoom Room = dc.Rooms.Include(s => s.RoomPictureMappings).SingleOrDefault(x => x.RoomId == id);
                ViewBag.RoomtypeListVB = dc.RoomTypes.Where(x=>x.IsActive==true && x.Status == 1).ToList();
                ViewBag.FloorListVB = dc.Floors.Where(x => x.IsActive == true && x.Status == 1).ToList();
                if (Room == null)
                {
                    return HttpNotFound();
                }
                return View("RoomForm", Room);
            }
            catch {
            return RedirectToAction("Index", "Login", new { ReturnUrl = "/Rooms" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ViewModelRoom Model)
        {
            try
            {
                if (Model.MinCapacity > Model.MaxCapacity)
                {
                    ViewBag.RoomtypeListVB = dc.RoomTypes.Where(x => x.IsActive == true && x.Status == 1).ToList();
                    ViewBag.FloorListVB = dc.Floors.Where(x => x.IsActive == true && x.Status == 1).ToList();
                    ViewBag.MinMax = "MinMax";
                    return View("RoomForm", Model);
                }

                var userDetail = SessionHelper.GetUserDetailFromSession();
                if (userDetail == null)
                {
                    return RedirectToAction("Index", "Login", new { ReturnUrl = "/Rooms" });
                }

                    var UserId = Convert.ToInt32(userDetail.user.id);
                if (ModelState.IsValid)
                    {
                        //New Files
                        for (int i = 0; i < Request.Files.Count; i++)
                        {
                            var file = Request.Files[i];

                            if (file != null && file.ContentLength > 0)
                            {
                            RoomPictureMapping rpc = new RoomPictureMapping();
                            var ActualfileName = Path.GetFileName(file.FileName);
                                var FileTime = DateTime.Now.ToFileTime();
                                string fileName = Path.GetFileNameWithoutExtension(file.FileName) + "_" + FileTime + Path.GetExtension(file.FileName);
                                rpc.FileName = fileName;
                                rpc.ActualFileName = ActualfileName;
                                rpc.Extension = Path.GetExtension(fileName);
                                rpc.RoomPictureMappingId = Guid.NewGuid();
                                rpc.RoomId = Model.RoomId;
                                rpc.CreatedDate = DateTime.Now;
                                rpc.UpdatedDate = DateTime.Now;
                                rpc.CreatedBy = UserId;
                                rpc.UpdatedBy = UserId;
                                dc.RoomPictureMappings.Add(rpc);
                                dc.SaveChanges();
                                var path = Path.Combine(Server.MapPath("~/Rotativa/RoomImage/"), Path.GetFileNameWithoutExtension(file.FileName) + "_" + FileTime + Path.GetExtension(file.FileName));
                                file.SaveAs(path);
                            }
                        }
                        var data = dc.Rooms.Where(x => x.RoomId == Model.RoomId).FirstOrDefault();
                        if (data != null)
                        {
                            data.FloorId = Model.FloorId;
                            data.RoomTypeId = Model.RoomTypeId;
                            data.RoomName = Model.RoomName;
                            data.RoomNumber = (int)Model.RoomNumber;
                            data.Description = Model.Description;
                            data.Capacity = Model.Capacity;
                            data.MinCapacity = Model.MinCapacity;
                            data.MaxCapacity = Model.MaxCapacity;
                            data.UpdatedBy = UserId;
                            data.UpdatedDate = DateTime.Now;
                            Model.Status = 1;
                            Model.IsActive = true;
                         TempData["update"] = "doneupdate";
                        dc.SaveChanges();

                        MRMLog _MRMLog = new MRMLog();
                        _MRMLog.UserId = Convert.ToInt32(UserId);
                        _MRMLog.Action = "Update";
                        _MRMLog.Module = "Room Update Operation";
                        _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has Updated Room having id= " + Model.RoomId;
                        _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                        _MRMLog.CraetedDate = DateTime.Now;
                        dc.MRMLogs.Add(_MRMLog);
                        dc.SaveChanges();
                    }
                    return RedirectToAction("Index", "Rooms");
                }
                    return View("Roomform",Model);              
            }
            catch(Exception ex) {
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
                return RedirectToAction("Index", "Login", new { ReturnUrl = "/Rooms" }); };
            }
        //deleteFilerecord

        [HttpPost]
        public JsonResult DeleteFile(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return Json(new { Result = "Error" });
            }
            try
            {
                Guid guid = new Guid(id);
                RoomPictureMapping fileDetail = dc.RoomPictureMappings.Find(guid);
                if (fileDetail == null)
                {
                    return Json(new { Result = "Error" });
                }
                //Remove from database
                dc.RoomPictureMappings.Remove(fileDetail);
                dc.SaveChanges();
                //Delete file from the file system
                var path = Path.Combine(Server.MapPath("~/Rotativa/RoomImage/"), fileDetail.RoomPictureMappingId + fileDetail.Extension);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error" });
            }
        }

        public ActionResult CheckRoomReference(Guid RoomId)
        {
            var data = _RoomMethods.checkRoomReferenceID(RoomId);
            if (data)
            {
                int? status = dc.Rooms.Where(x => x.RoomId == RoomId && x.IsActive == true).Select(x => x.Status).FirstOrDefault();
                if (status == 1) { return Json(new { result = "ReferenceExist", status="WantDACT"}); }
                else
                {
                    return Json(new { result = "ReferenceExist", status = "WantACT" });
                }
            }
            else
            {
                int? status = dc.Rooms.Where(x => x.RoomId == RoomId && x.IsActive == true).Select(x => x.Status).FirstOrDefault();
                if (status == 1) { return Json(new { result = "ReferenceNotExist", status = "WantDACT" }); }
                else
                {
                    return Json(new { result = "ReferenceNotExist", status = "WantACT" });
                }
            }
        }
        public ActionResult DeleteRoomReference(Guid RoomId)
        {
            var data = _RoomMethods.DeleteRoomReferenceID(RoomId);
            if (data)
            {
                return Json(new { result = "ReferenceDeleted", url = Url.Action("Index", "Rooms") });
            }
            else
            {
                return RedirectToAction("Index", "Login", new { ReturnUrl = "/Rooms" });
            }
        }
        public ActionResult DeActivateRoomReference(Guid RoomId)
        {
            var data = _RoomMethods.DeActivateRoomReferenceID(RoomId);
            if (data)
            {
             
                    return Json(new { result = "roomStatusupdated", url = Url.Action("Index", "Rooms") });
              //  }
                
            }
            else
            {
                return RedirectToAction("Index", "Login", new { ReturnUrl = "/Rooms" });
            }
        }
        
        //delete Room and related uploaded image datail
        //[HttpPost]
        //public JsonResult Delete(Guid id)
        //{
        //    try
        //    {
        //        ViewModelRoom Room = _db.Rooms.Find(id);
        //        if (Room == null)
        //        {
        //            //Response.StatusCode = (int)HttpStatusCode.NotFound;
        //            return Json(new { Result = "Error" });
        //        }
        //        //delete files from the file system
        //        foreach (var item in Room.RoomPictureMappings)
        //        {
        //            String path = Path.Combine(Server.MapPath("~/Rotativa/RoomImage/"), item.RoomPictureMappingId + item.Extension);
        //            if (System.IO.File.Exists(path))
        //            {
        //                System.IO.File.Delete(path);
        //            }
        //        }
        //        _db.Rooms.Remove(Room);
        //        _db.SaveChanges();
        //        return Json(new { Result = "OK" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { Result = "ERROR", Message = ex.Message });
        //    }
        //}

        #endregion
    }
}