

using MRM.DbEntity;
using MRM.Web.DBMethods;
using MRM.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;
using System.IO;
using System.Data.Entity;
using MRM.Web.Helpers;
using System.Globalization;
using Ical.Net.Interfaces;
using System.Text;
using Ical.Net.DataTypes;
using DDay.iCal;
using DDay.iCal.Serialization;
using MRM.Helper.Helpers;

namespace MRM.Web.Controllers
{
    [CustomAuthorize("2", "3")]
    public class BookRoomController : Controller
    {
        calmethod CM = new calmethod();
        //DBContextRoom _db = new DBContextRoom();
        RoomMeetingManagementDbEntities db = new RoomMeetingManagementDbEntities();
        public ActionResult Index(ViewModelCalendarRoomDetail Model)
        {
            try
            {
                RoomMeetingManagementDbEntities _db = new RoomMeetingManagementDbEntities();
                ViewBag.RoomtypeListVB = _db.RoomTypes.Where(x => x.IsActive == true && x.Status == 1).ToList();
                ViewBag.FloorListVB = _db.Floors.Where(x => x.IsActive == true && x.Status == 1).ToList();
                ViewBag.StatusMsg = TempData["StatusMsg"];
                return View();
            }
            catch (Exception ex)
            {
                string filePath = Server.MapPath("~/Rotativa/Bug/Error.txt");// @"D:\Error.txt";
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
        public JsonResult GetEvents()
        {
            ViewModelCalendar model = new ViewModelCalendar();
            using (RoomMeetingManagementDbEntities dc = new RoomMeetingManagementDbEntities())
            {
                var caldetail = CM.calendardetailtomodel();
                var rows2 = caldetail.ToList();
                return new JsonResult { Data = rows2, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            }
        }
        public ActionResult OpenEvent()
        {
            ViewModelCalendar Model = new ViewModelCalendar();
            ViewBag.RoomtypeListVB = db.RoomTypes.Where(x => x.IsActive == true && x.Status == 1).ToList();
            ViewBag.FloorListVB = db.Floors.Where(x => x.IsActive == true && x.Status == 1).ToList();
            // Model = CM.GetCalAddDetails();
            return PartialView("_CalendarAddEditViewModel", Model);
        }
        public ActionResult SaveBookRoomDetail(ViewModelCalendarRoomDetail Model)
        {
            var data = CM.SaveCalEventInfo(Model);
            if (data == 1)
            {
                ViewBag.StatusMsg = "SuccessRoom";
                return Json(new { result = "Redirect", url = Url.Action("Index", "BookRoom") });
            }
            else if (data == 2)
            {
                ViewBag.StatusMsg = "Exist";
                return Json(new { result = "RedirectExist", url = Url.Action("Index", "BookRoom") });
            }
            else if (data == 3)
            {
                return Json(new { result = "Expire", url = Url.Action("Index", "Login", new { ReturnUrl = "/BookRoom" }) });
            }
            else if (data == 5)
            {
                ViewBag.StatusMsg = "Greter";
                return Json(new { result = "Greter", url = Url.Action("Index", "BookRoom") });
            }
            else
            {
                return Json(new { result = "Error", url = Url.Action("Index", "BookRoom") });
            }
        }

        public ActionResult FillRoomDetail(ViewModelCalendarRoomDetail Model)
        {
            try
            {
                List<ViewModelCalendarRoomDetail> model = new List<ViewModelCalendarRoomDetail>();
                //  model = CM.GetSyncAllRoomDetails(); 
                // return PartialView("_SyncFillBookRoomDetail", model);
                Model = CM.GetRemSyncAllRoomDetails(Model);
                return PartialView("_SyncFillBookRoomDetail", Model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login", new { ReturnUrl = "/BoomRoom" });
            }
        }

        //[HttpPost]
        //public ActionResult SaveSnapshot()
        //{
        //    bool saved = false;
        //    if (Request.Form["base64data"] != null)
        //    {
        //        string image = Request.Form["base64data"].ToString();
        //        byte[] data = Convert.FromBase64String(image);
        //        var path = Path.Combine(Server.MapPath("~/Upload"), "snapshot.png");
        //        System.IO.File.WriteAllBytes(path, data);
        //        saved = true;
        //    }

        //    return Json(saved ? "image saved" : "image not saved");
        //}
        public ActionResult BookRoomFilter(ViewModelCalendarRoomDetail Model)
        {
          
            try
            {
                if (Model.BookRoomId != null)
                {
                    Model = CM.GetRoomsForFill(Model);
                    ViewBag.smin = Model.sMin;
                    ViewBag.shour = Model.sHour;
                    ViewBag.emin = Model.eMin;
                    ViewBag.ehour = Model.eHour;
                    ViewBag.startdatepik = Model.StartDate.Date;
                    ViewBag.enddatepik = Model.EndDate.Date;
                    ViewBag.RoomtypeListVB = db.RoomTypes.Where(x => x.IsActive == true && x.Status == 1).ToList();
                    ViewBag.FloorListVB = db.Floors.Where(x => x.IsActive == true && x.Status == 1).ToList();
                    return View("BookRoom", Model);
                }
                else
                {
                    ViewBag.RoomtypeListVB = db.RoomTypes.Where(x => x.IsActive == true && x.Status == 1).ToList();
                    ViewBag.FloorListVB = db.Floors.Where(x => x.IsActive == true && x.Status == 1).ToList();
                    return View("BookRoom", Model);
                }
            }
            catch
            {
                return RedirectToAction("Index", "Login", new { ReturnUrl = "/BoomRoom" });
            }
        }
        public ActionResult PartialFilterRoom(ViewModelCalendarRoomDetail Model)
        {
            try
            {              
                Model = CM.GetRoomsFilterDetails(Model);              
                return PartialView("_AvailableRoom", Model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Login", new { ReturnUrl = "/BoomRoom" });
            }
        }
        [HttpPost]
        public ActionResult SavePDFOnDirectory()
        {

            var FileTime = DateTime.Now.ToFileTime();
            string pdfFilePath = Server.MapPath("~/Rotativa/PDF");
            string FileName = FileTime + ".pdf";
            string fullFilePath = pdfFilePath + "\\" + FileName;
            for (int i = 0; i < Request.Files.Count; i++)
            {
                var Imagefile = Request.Files[i];
                var fileName = Path.GetFileName(Imagefile.FileName + ".png");
                var Imagepath = Path.Combine(Server.MapPath("~/Rotativa/blob"), fileName);
                Imagefile.SaveAs(Imagepath);
                Guid guid = Guid.NewGuid();
                Document doc = new Document(PageSize.A4_LANDSCAPE, 10f, 10f, 5f, 10f);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(fullFilePath, FileMode.Create));
                doc.Open();
                try
                {
                    Paragraph paragraph = new Paragraph("Calender Event Details");
                    iTextSharp.text.Image Png = iTextSharp.text.Image.GetInstance(Imagepath);
                    iTextSharp.text.pdf.PdfPCell cell = new iTextSharp.text.pdf.PdfPCell(Png);
                    iTextSharp.text.pdf.PdfPTable table = new iTextSharp.text.pdf.PdfPTable(1);
                    cell.FixedHeight = 250;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.VerticalAlignment = Element.ALIGN_CENTER;
                    Png.ScaleAbsolute(550, 350);
                    table.AddCell(cell);
                    doc.Add(paragraph);
                    doc.Add(Png);
                    doc.Close();
                }
                catch (Exception ex)
                { return RedirectToAction("Index", "Login", new { ReturnUrl = "/BoomRoom" }); }
            }
            return Json(new { fileName = FileName }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DownloadCalPdf(string file)
        {
            try
            {
                string fullPath = Path.Combine(Server.MapPath("~/Rotativa/PDF"), file);
                return File(fullPath, System.Net.Mime.MediaTypeNames.Application.Pdf, file);
            }
            catch { return RedirectToAction("Index", "Login", new { ReturnUrl = "/BoomRoom" }); }
        }
        public ActionResult ExportSolution()
        {

            var userDetail = SessionHelper.GetUserDetailFromSession();
            if (userDetail == null) {
                return RedirectToAction("Index", "Login", new { ReturnUrl = "/BookRoom" });
            }
            int UserId = Convert.ToInt32(userDetail.user.id);
            var calendarevents = db.BookRooms.Where(x => x.UserId == UserId && x.IsSyncIcal == false && x.IsActive == true).ToList();
           // var calendarevents = CM.calendardetailtomodel();
            var FileTime = DateTime.Now.ToFileTime();
            var ext = ".ics";
            DDay.iCal.iCalendar iCal = new DDay.iCal.iCalendar();
            foreach (var demoEvent in calendarevents)
            {
                Event evt = iCal.Create<Event>();
                evt.Start = new iCalDateTime(demoEvent.StartDate);
                evt.End = new iCalDateTime(demoEvent.EndDate);
                evt.Description = demoEvent.Description;
                evt.Summary = demoEvent.Description;
                var datas = db.BookRooms.Where(x => x.BookRoomId == demoEvent.BookRoomId).FirstOrDefault();
                if (datas != null)
                {
                    datas.IsSyncIcal = true;
                    db.SaveChanges();//update
                    //TempData["StatusMsg"] = "Success";
                }
            }
            ISerializationContext ctx = new SerializationContext();
            ISerializerFactory factory = new DDay.iCal.Serialization.iCalendar.SerializerFactory();
            // Get a serializer for our object
            IStringSerializer serializer = factory.Build(iCal.GetType(), ctx) as IStringSerializer;
            string output = serializer.SerializeToString(iCal);
            var contentType = "text/calendar";
            var bytes = Encoding.UTF8.GetBytes(output);
            return File(bytes, contentType, FileTime + ext);
        }
    }
}