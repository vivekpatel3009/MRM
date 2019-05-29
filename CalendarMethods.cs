using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MRM.DbEntity;
using MRM.Web.ViewModel;
using System.Web.Mvc;
using MRM.Helper.Helpers;
using System.Data.Entity.Core.Objects;
using System.Globalization;

namespace MRM.Web.DBMethods
{
    public class CalendarMethods
    {
        RoomMeetingManagementDbEntities _Db = new RoomMeetingManagementDbEntities();
        //public ViewModelCalendar GetCalAddDetails()
        //{
        //    try
        //    {
        //        ViewModelCalendar Model = new ViewModelCalendar();        //       
        //        return Model;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}
        public ViewModelCalendarRoomDetail GetRoomDetails()
        {
            ViewModelCalendarRoomDetail CM = new ViewModelCalendarRoomDetail();
            CM.RoomDetailListCal = GetAllRoomDetails();
            return CM;
        }
        public List<ViewModelCalendarRoomDetail> GetAllRoomDetails()
        {
            List<ViewModelCalendarRoomDetail> Model = new List<ViewModelCalendarRoomDetail>();

            var data = _Db.Rooms.Where(x => x.IsActive == true && x.Status == 1).OrderByDescending(x => x.DId).ToList();
            if (data.Count > 0)
            {
                var Count = 0;
                foreach (var item in data)
                {
                    ViewModelCalendarRoomDetail mm = new ViewModelCalendarRoomDetail();
                    Count++;
                    mm.DId = item.DId;
                    mm.RoomId = item.RoomId;
                    mm.RoomName = item.RoomName;
                    mm.RoomNumber = item.RoomNumber.ToString();
                    mm.Capacity = item.Capacity;
                    var floordetails = getfloor_ListValueById((Guid)item.FloorId);
                    var Roomtypedetails = getRoomtype_ListValueById((Guid)item.RoomTypeId);
                    var Roomdetails = getRoom_ListValueById((Guid)item.RoomId);
                    if (floordetails != null)
                    {
                        mm.FloorName = floordetails.FloorName;
                    }
                    if (Roomtypedetails != null)
                    {
                        mm.RoomTypeName = Roomtypedetails.RoomtypeName;
                    }
                    mm.RoomPicture = getRoomPictureMapping_ById(item.RoomId);
                    Model.Add(mm);
                }
            }
            return Model;
        }
        public List<ViewModelCalendarRoomDetail> calendardetailtomodel()
        {
            List<ViewModelCalendarRoomDetail> Model = new List<ViewModelCalendarRoomDetail>();

            var data = _Db.BookRooms.Where(x => x.IsActive == true ).OrderByDescending(x => x.DId).ToList();
            if (data.Count > 0)
            {
                var Count = 0;
                foreach (var item in data)
                {
                    ViewModelCalendarRoomDetail mm = new ViewModelCalendarRoomDetail();
                    Count++;
                    mm.DId = item.DId;
                    mm.RoomId = (Guid)item.RoomId;
                    mm.Capacity = (int)item.Capacity;
                    mm.Description = item.Description;
                    mm.StartDate = item.StartDate;
                    mm.EndDate = item.EndDate;                    
                    var floordetails = getfloor_ListValueById((Guid)item.FloorId);
                    var Roomtypedetails = getRoomtype_ListValueById((Guid)item.RoomTypeId);
                    var Roomdetails = getRoom_ListValueById((Guid)item.RoomId);
                    var PictureDetails= getRoomPictureMapping_ByIdList((Guid)item.RoomId);
                    if (floordetails != null)
                    {
                        mm.FloorName = floordetails.FloorName;
                    }
                    if (Roomtypedetails != null)
                    {
                        mm.RoomTypeName = Roomtypedetails.RoomtypeName;
                    }
                    if (PictureDetails != null)
                    {
                        mm.RoomPicturelist = PictureDetails;
                        //mm.FileName = PictureDetail.FileName;
                    }
                    if (Roomdetails != null)
                    {
                        mm.RoomNumber = Convert.ToString(Roomdetails.RoomNumber);
                        mm.RoomName = Roomdetails.RoomName;
                    }
                    //mm.RoomPicture = getRoomPictureMapping_ById(item.RoomId);
                    Model.Add(mm);
                }
            }
            return Model;
        }
        public ViewModelCalendarRoomDetail GetRoomsFilterDetails(ViewModelCalendarRoomDetail Model)
        {
            ViewModelCalendarRoomDetail CM = new ViewModelCalendarRoomDetail();
            CM.RoomDetailListCal = GetAllRoomsFilterDetails(Model);
            return CM;

        }
        public List<ViewModelCalendarRoomDetail> GetAllRoomsFilterDetails(ViewModelCalendarRoomDetail ModelCM)
        {
            List<ViewModelCalendarRoomDetail> Model = new List<ViewModelCalendarRoomDetail>();
            dynamic data = "";

            if (ModelCM.Start == null && ModelCM.End == null)
            {
                data = _Db.GetAvailableRooms(DateTime.Now, DateTime.Now, ModelCM.FloorId, ModelCM.RoomTypeId,ModelCM.MaxCapacity,ModelCM.MinCapacity).ToList();
            }
            else
            {
                CultureInfo culture = new CultureInfo("en-US");
                DateTime StartDate = DateTime.ParseExact(ModelCM.Start, "dd/MM/yyyy", culture);
                DateTime EndDate = DateTime.ParseExact(ModelCM.End, "dd/MM/yyyy", culture);
                DateTime Start = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, ModelCM.sHour, ModelCM.sMin, StartDate.Second, 0);
                DateTime End = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, ModelCM.eHour, ModelCM.eMin, EndDate.Second, 0);
            
                data = _Db.GetAvailableRooms(Start, End, ModelCM.FloorId, ModelCM.RoomTypeId, ModelCM.MaxCapacity, ModelCM.MinCapacity).ToList();
            }
            if (data.Count > 0)
            {
                var Count = 0;
                foreach (var item in data)
                {
                    ViewModelCalendarRoomDetail mm = new ViewModelCalendarRoomDetail();
                    Count++;
                    mm.Count = Count;
                    mm.DId = item.DId;
                    mm.RoomId = item.RoomId;
                    mm.RoomName = item.RoomName;
                    mm.RoomNumber = item.RoomNumber.ToString();
                    mm.Capacity = item.Capacity;
                    mm.MaxCapacity = item.MaxCapacity;
                    mm.MinCapacity = item.MinCapacity;
                    //mm.IsBooked = Convert.ToString(item.IsBooked);
                    mm.NewIsBooked = item.IsBooked;
                    var floordetails = getfloor_ListValueById((Guid)item.FloorId);
                    var Roomtypedetails = getRoomtype_ListValueById((Guid)item.RoomTypeId);
                    var Roomdetails = getRoom_ListValueById((Guid)item.RoomId);
                    var RoomPicDetails = getRoomPictureMapping_ById(item.RoomId);
                    getRoomPictureMapping_ByIdList(item.RoomId);
                    if (floordetails != null)
                    {
                        mm.FloorName = floordetails.FloorName;
                    }
                    if (Roomtypedetails != null)
                    {
                        mm.RoomTypeName = Roomtypedetails.RoomtypeName;
                    }
                    if (RoomPicDetails != null)
                    {
                        mm.RoomPicture = RoomPicDetails;
                    }
                    Model.Add(mm);
                }
            }
            return Model;
        }
        public int SaveCalEventInfo(ViewModelCalendarRoomDetail Model)
        {
            var userDetail = SessionHelper.GetUserDetailFromSession();
            if (userDetail == null) { return 3; }
            var UserId = Convert.ToInt32(userDetail.user.id);  
            var Roomdetails = getRoom_ListValueById((Guid)Model.RoomId);
            try
            {

                //ObjectParameter returnId = new ObjectParameter("Exists", typeof(int));
                ObjectParameter returnId = new ObjectParameter("Exists", typeof(int));
                //var exist= _Db.CheckRoomExist(Model.Start, Model.End, Model.RoomId);
                CultureInfo culture = new CultureInfo("en-US");
                DateTime StartDate = DateTime.ParseExact(Model.Start, "dd/MM/yyyy", culture);
                DateTime EndDate = DateTime.ParseExact(Model.End, "dd/MM/yyyy", culture);
                DateTime Start = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, Model.sHour, Model.sMin, StartDate.Second, 0);
                DateTime End = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, Model.eHour, Model.eMin, EndDate.Second, 0);
                if (Start > End) {
                    return 5;
                }
                var  exist = _Db.CheckRoomExist1(Start, End, Model.RoomId, returnId).ToList();
                int recordexist = Convert.ToInt32(returnId.Value);

                if (recordexist == 0)
                {

                    BookRoom bm = new BookRoom();
                    Guid RoomId = Guid.NewGuid();
                    bm.RoomId = Model.RoomId;
                    Guid BookRoomId = Guid.NewGuid();
                    bm.BookRoomId = BookRoomId;
                    bm.UserId = UserId;
                    bm.DId = 1;
                    bm.StartDate = Start;
                    bm.EndDate = End;
                    bm.Description = Model.Description;
                    //bm.FloorId = Roomdetails.FloorId;
                    //bm.RoomTypeId = Roomdetails.RoomTypeId;
                    bm.Capacity = Model.Capacity;
                    bm.CreatedDate = DateTime.Now;
                    bm.UpdatedDate = DateTime.Now;
                    bm.CreatedBy = UserId;
                    bm.UpdatedBy = UserId;
                    if (Roomdetails != null)
                    {
                        bm.FloorId = Roomdetails.FloorId;
                        bm.RoomTypeId = Roomdetails.RoomTypeId;
                    }
                    bm.IsActive = true;
                    bm.IsSyncOutlookCal = false;
                    bm.IsSyncGoogleCal = false;
                    bm.IsSyncIcal = false;
                    _Db.BookRooms.Add(bm);
                    _Db.SaveChanges();
                    return 1;
                }
                else if (recordexist ==1)
                { return 2; }
                else { return 4; }
            }
            catch (Exception ex) { return 0; }
        }
        public List<ViewModelCalendar> GetAllEventList()
        {
            List<ViewModelCalendar> Model = new List<ViewModelCalendar>();

            var data = _Db.BookRooms.Where(x => x.IsActive == true).OrderByDescending(x => x.DId).ToList();
            if (data.Count > 0)
            {
                var count = 0;
                foreach (var item in data)
                {
                    ViewModelCalendar model = new ViewModelCalendar();
                    count++;
                    var floordetails = getfloor_ListValueById((Guid)item.FloorId);
                    var Roomtypedetails = getRoomtype_ListValueById((Guid)item.RoomTypeId);
                    var Roomdetails = getRoom_ListValueById((Guid)item.RoomId);
                    model.BookRoomId = item.BookRoomId;
                    // model.Count = count;
                    model.Start = item.StartDate;
                    model.End = item.EndDate;
                    model.Description = item.Description;
                    model.Capacity = (int)item.Capacity;
                    //model.MinCapacity = (int)item.Capacity;
                    //model.MaxCapacity = (int)item.Capacity;
                    if (floordetails != null)
                    {
                        model.Floor = floordetails.FloorName;
                    }
                    if (Roomdetails != null)
                    {
                        model.Room = Roomdetails.RoomName;
                    }
                    if (Roomtypedetails != null)
                    {
                        model.RoomType = Roomtypedetails.RoomtypeName;
                    }

                    Model.Add(model);
                }
            }
            return Model;
        }
        public Floor getfloor_ListValueById(Guid Id)
        {
            return _Db.Floors.Where(x => x.FloorId == Id).FirstOrDefault();
        }
        public RoomType getRoomtype_ListValueById(Guid Id)
        {
            return _Db.RoomTypes.Where(x => x.RoomTypeId == Id).FirstOrDefault();
        }
        public Room getRoom_ListValueById(Guid Id)
        {
            return _Db.Rooms.Where(x => x.RoomId == Id).FirstOrDefault();
        }
        public RoomPictureMapping getRoomPictureMapping_ById(Guid RoomId)
        {
            return _Db.RoomPictureMappings.Where(x => x.RoomId == RoomId).FirstOrDefault();
        }
        public List<RoomPictureMapping> getRoomPictureMapping_ByIdList(Guid RoomId)
        {
            return _Db.RoomPictureMappings.Where(x => x.RoomId == RoomId).ToList();
        }
    }
}