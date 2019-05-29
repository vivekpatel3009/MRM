using MRM.DbEntity;
using MRM.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MRM.Helper.Helpers;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MRM.Web.DBMethods
{
    #region Constant
    // ViewModelFloor vm = new ViewModelFloor();
    #endregion

    #region Methods
    public class FloorMethods
    {
      
        public List<ViewModelFloor> GetAllFloorList()
        {
            List<ViewModelFloor> Model = new List<ViewModelFloor>();
            RoomMeetingManagementDbEntities _db = new RoomMeetingManagementDbEntities();
            var data = _db.Floors.Where(x => x.IsActive == true).OrderByDescending(x => x.DId).ToList();
            if (data.Count > 0)
            {
                var count = 0;
                foreach (var item in data)
                {
                    ViewModelFloor model = new ViewModelFloor();
                    count++;
                    model.Count = count;
                    model.DId = item.DId;
                    model.FloorId = item.FloorId;                 
                    model.FloorName = item.FloorName;
                    model.Description = item.Description;
                    model.Status = (int)item.Status;
                    model.CreatedBy = (int)item.CreatedBy;
                    model.UpdatedBy = (int)item.UpdatedBy;
                    model.CreatedDate = item.CreatedDate;
                    model.UpdatedDate = item.UpdatedDate;
                    Model.Add(model);
                }
            }
            return Model;
        }

        public int SaveFloorInfo(ViewModelFloor Model)
        {
            RoomMeetingManagementDbEntities _db = new RoomMeetingManagementDbEntities();
            var userDetail = SessionHelper.GetUserDetailFromSession();
            if (userDetail != null)
            {
                var UserId = Convert.ToInt32(userDetail.user.id);

                if (1 == 1)
                {
                    var data = _db.Floors.Where(x => x.FloorName == Model.FloorName || x.FloorId == Model.FloorId).FirstOrDefault();
                    if (data == null)
                    {
                        return 0;
                    }
                    else
                    {
                        var datas = _db.Floors.Where(x => x.FloorId == Model.FloorId).FirstOrDefault();
                        if (datas != null)
                        {
                            datas.FloorName = Model.FloorName;
                            datas.FloorNumber = Model.FloorNumber;
                            datas.Description = Model.Description;
                            datas.Status = Model.Status;
                            datas.CreatedBy = Model.CreatedBy;
                            datas.UpdatedBy = Convert.ToInt32(UserId);
                            datas.UpdatedDate = DateTime.Now;
                            _db.SaveChanges();//update

                            MRMLog _MRMLog = new MRMLog();
                            _MRMLog.UserId = Convert.ToInt32(UserId);
                            _MRMLog.Action = "Update";
                            _MRMLog.Module = "Floor Update Operation";
                            _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has Updated Floor having id= " + datas.FloorId;
                            _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                            _MRMLog.CraetedDate = DateTime.Now;
                            _db.MRMLogs.Add(_MRMLog);
                            _db.SaveChanges();
                        }
                        return 1;
                    }
                }
                else
                {
                    var data = _db.Floors.Where(x => x.FloorName == Model.FloorName && x.IsActive == true).FirstOrDefault();
                    if (data != null)
                    {
                        return 0;
                    }
                    else
                    {
                        Floor _floordata = new Floor();
                        Guid g = Guid.NewGuid();
                        _floordata.FloorId = g;
                        _floordata.FloorName = Model.FloorName;
                        _floordata.FloorNumber = Model.FloorNumber;
                        _floordata.Description = Model.Description;
                        _floordata.Status = Model.Status;
                        _floordata.IsActive = true;
                        _floordata.CreatedBy = Convert.ToInt32(UserId);
                        _floordata.UpdatedBy = Convert.ToInt32(UserId);
                        _floordata.CreatedDate = DateTime.Now;
                        _floordata.UpdatedDate = DateTime.Now;
                        _db.Floors.Add(_floordata);
                        _db.SaveChanges();
                        return 1;
                    }
                }
            }
            else { return 2; }
        }

        public ViewModelFloor GetFloorDetailsById(Guid Id)
        {
            ViewModelFloor model = new ViewModelFloor();
            RoomMeetingManagementDbEntities _db = new RoomMeetingManagementDbEntities();           
            var data = _db.Floors.Where(x => x.FloorId == Id && x.IsActive==true).FirstOrDefault();
            if (data != null)
            {
               // model.Count = 1;
                model.DId = data.DId;
                model.FloorId = data.FloorId;
                model.FloorName = data.FloorName;
                model.FloorNumber = data.FloorNumber;
                model.Description = data.Description;
                model.Status = (int)data.Status;
                //model.IsEdit = 1;
            }
            return model;
        }
        //public bool DeleteFloorInfo(Guid Id)
        //{
        //    RoomMeetingManagementDbEntities _db = new RoomMeetingManagementDbEntities();
        //    var data = _db.Floors.Where(x => x.FloorId == Id).FirstOrDefault();
        //    if (data != null)
        //    {
        //        data.IsActive = false;
        //        data.UpdatedDate = DateTime.Now;
        //        _db.SaveChanges();
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        public bool checkFloorReferenceID(Guid Id)
        {
            RoomMeetingManagementDbEntities _db = new RoomMeetingManagementDbEntities();
            var data = _db.Rooms.Where(x => x.FloorId == Id && x.IsActive == true && x.Status==1).ToList();
            if (data.Count>0)
            {              
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DeleteFloorReferenceID(Guid Id)
        {
            var userDetail = SessionHelper.GetUserDetailFromSession();
            if (userDetail == null)
            {
                return false;
            }
            else
            {
                var UserId = Convert.ToInt32(userDetail.user.id);

                RoomMeetingManagementDbEntities _db = new RoomMeetingManagementDbEntities();
                var Roomdata = _db.Rooms.Where(x => x.FloorId == Id && x.IsActive == true).ToList();
                var BookRoomdata = _db.BookRooms.Where(x => x.FloorId == Id && x.IsActive == true).ToList();
                var floordata = _db.Floors.Where(x => x.FloorId == Id && x.IsActive == true).FirstOrDefault();
                if (Roomdata != null)
                {
                    foreach (var item in Roomdata)
                    {
                        item.IsActive = false;
                        item.UpdatedBy = UserId;
                        item.UpdatedDate = DateTime.Now;
                        _db.SaveChanges();

                        MRMLog _MRMLog = new MRMLog();
                        _MRMLog.UserId = Convert.ToInt32(UserId);
                        _MRMLog.Action = "Delete";
                        _MRMLog.Module = "Floor Delete Operation";
                        _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has deleted Room having id= " + item.RoomId;
                        _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                        _MRMLog.CraetedDate = DateTime.Now;
                        _db.MRMLogs.Add(_MRMLog);
                        _db.SaveChanges();
                    }

                }
                if (BookRoomdata != null)
                {
                    foreach (var item in BookRoomdata)
                    {
                        item.IsActive = false;
                        item.UpdatedDate = DateTime.Now;
                        item.UpdatedBy = UserId;
                        _db.SaveChanges();

                        MRMLog _MRMLog = new MRMLog();
                        _MRMLog.UserId = Convert.ToInt32(UserId);
                        _MRMLog.Action = "Delete";
                        _MRMLog.Module = "Floor Delete Operation";
                        _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has deleted BookRoom having id=  " + item.BookRoomId;
                        _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                        _MRMLog.CraetedDate = DateTime.Now;
                        _db.MRMLogs.Add(_MRMLog);
                        _db.SaveChanges();
                    }
                }
                if (floordata != null)
                {
                    floordata.IsActive = false;
                    floordata.UpdatedBy = UserId;
                    floordata.UpdatedDate = DateTime.Now;
                    _db.SaveChanges();

                    MRMLog _MRMLog = new MRMLog();
                    _MRMLog.UserId = Convert.ToInt32(UserId);
                    _MRMLog.Action = "Delete";
                    _MRMLog.Module = "Floor Delete Operation";
                    _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has deleted Floor having id=  " + floordata.FloorId;
                    _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                    _MRMLog.CraetedDate = DateTime.Now;
                    _db.MRMLogs.Add(_MRMLog);
                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool DeActivateFloorReferenceID(Guid Id)
        {
            var userDetail = SessionHelper.GetUserDetailFromSession();
            if (userDetail == null)
            {
                return false;
            }
            else
            {
                var UserId = Convert.ToInt32(userDetail.user.id);
                RoomMeetingManagementDbEntities _db = new RoomMeetingManagementDbEntities();
                var Roomdata = _db.Rooms.Where(x => x.FloorId == Id && x.IsActive == true && x.Status == 1).ToList();
                var BookRoomdata = _db.BookRooms.Where(x => x.FloorId == Id && x.IsActive == true).ToList();
                var floordata = _db.Floors.Where(x => x.FloorId == Id && x.IsActive == true && x.Status == 1).FirstOrDefault();
                if (Roomdata != null)
                {
                    foreach (var item in Roomdata)
                    {
                        item.Status = 0;
                        item.UpdatedBy = UserId;
                        item.UpdatedDate = DateTime.Now;
                        _db.SaveChanges();

                        MRMLog _MRMLog = new MRMLog();
                        _MRMLog.UserId = Convert.ToInt32(UserId);
                        _MRMLog.Action = "Deactive";
                        _MRMLog.Module = "Floor Deactive Operation";
                        _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has Deactive Floor having id=  " + item.RoomId;
                        _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                        _MRMLog.CraetedDate = DateTime.Now;
                        _db.MRMLogs.Add(_MRMLog);
                        _db.SaveChanges();
                    }
                }
                if (BookRoomdata != null)
                {
                    foreach (var item in BookRoomdata)
                    {
                        item.IsActive = false;
                        item.UpdatedBy = UserId;
                        item.UpdatedDate = DateTime.Now;
                        _db.SaveChanges();

                        MRMLog _MRMLog = new MRMLog();
                        _MRMLog.UserId = Convert.ToInt32(UserId);
                        _MRMLog.Action = "Deactive";
                        _MRMLog.Module = "Floor Deactive Operation";
                        _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has Deactive BookRoom having id=  " + item.BookRoomId;
                        _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                        _MRMLog.CraetedDate = DateTime.Now;
                        _db.MRMLogs.Add(_MRMLog);
                        _db.SaveChanges();
                    }
                }
                if (floordata != null)
                {
                    floordata.Status = 0;
                    floordata.UpdatedBy = UserId;
                    floordata.UpdatedDate = DateTime.Now;
                    _db.SaveChanges();

                    MRMLog _MRMLog = new MRMLog();
                    _MRMLog.UserId = Convert.ToInt32(UserId);
                    _MRMLog.Action = "Deactive";
                    _MRMLog.Module = "Floor Deactive Operation";
                    _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has Deactive Floor having id=  " + floordata.FloorId;
                    _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                    _MRMLog.CraetedDate = DateTime.Now;
                    _db.MRMLogs.Add(_MRMLog);
                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool ActivateFloorReferenceID(Guid Id)
        {
            var userDetail = SessionHelper.GetUserDetailFromSession();
            if (userDetail == null)
            {
                return false;
            }
            else
            {
                var UserId = Convert.ToInt32(userDetail.user.id);
                RoomMeetingManagementDbEntities _db = new RoomMeetingManagementDbEntities();
                // var Roomdata = _db.Rooms.Where(x => x.RoomId == Id && x.IsActive == true && x.Status == 0).ToList();
                // var BookRoomdata = _db.BookRooms.Where(x => x.RoomId == Id && x.IsActive == true).ToList();
                var floordata = _db.Floors.Where(x => x.FloorId == Id && x.IsActive == true && x.Status == 0).FirstOrDefault();

                if (floordata != null)
                {
                    floordata.Status = 1;
                    floordata.UpdatedBy = UserId;
                    floordata.UpdatedDate = DateTime.Now;
                    _db.SaveChanges();

                    MRMLog _MRMLog = new MRMLog();
                    _MRMLog.UserId = Convert.ToInt32(UserId);
                    _MRMLog.Action = "Active";
                    _MRMLog.Module = "Floor Active Operation";
                    _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has Active Floor having id=  " + floordata.FloorId;
                    _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                    _MRMLog.CraetedDate = DateTime.Now;
                    _db.MRMLogs.Add(_MRMLog);
                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

    }
    #endregion
}