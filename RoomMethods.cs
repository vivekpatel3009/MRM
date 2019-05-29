using MRM.DbEntity;
using MRM.Helper.Helpers;
using MRM.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MRM.Web.DBMethods
{

    #region Methods
    public class RoomMethods
    {
        RoomMeetingManagementDbEntities dc = new RoomMeetingManagementDbEntities();
        public List<ViewModelRoom> GetAllRoomList()
        {
            List<ViewModelRoom> Model = new List<ViewModelRoom>();
            RoomMeetingManagementDbEntities dc = new RoomMeetingManagementDbEntities();
            var data = dc.Rooms.Where(x => x.IsActive == true).OrderByDescending(x => x.DId).ToList();
       
            if (data.Count > 0)
            {
                var count = 0;
                foreach (var item in data)
                {
                    ViewModelRoom model = new ViewModelRoom();
                    count++;
                    model.Count = count;
                    model.DId = item.DId;
                    model.RoomId = item.RoomId;
                    model.FloorId = item.RoomId;
                    model.RoomTypeId = item.RoomTypeId;
                    model.RoomName = item.RoomName;
                    model.Capacity = item.Capacity;
                    var floordetails = getfloor_ListValueById((Guid)item.FloorId);
                    var Roomtypedetails = getRoomtype_ListValueById((Guid)item.RoomTypeId);
                    if (item.MinCapacity != null)
                    {
                        model.MinCapacity = (int)item.MinCapacity;
                    }
                    if (item.MaxCapacity != null)
                    {
                        model.MaxCapacity = (int)item.MaxCapacity;
                    }
                    if (floordetails != null)
                    {
                        model.Floor = floordetails.FloorName;
                    }
                    if (Roomtypedetails != null)
                    {
                        model.Roomtype = Roomtypedetails.RoomtypeName;
                    }
                    model.Status = (int)item.Status;
                    model.Description = item.Description;
                    model.CreatedBy = (int)item.CreatedBy;
                    model.UpdatedBy = (int)item.UpdatedBy;
                    model.CreatedDate = (DateTime)item.CreatedDate;
                    model.UpdatedDate = (DateTime)item.UpdatedDate;
                    Model.Add(model);
                }
            }
            return Model;
        }
        public Floor getfloor_ListValueById(Guid Id)
        {
            return dc.Floors.Where(x => x.FloorId == Id).FirstOrDefault();
        }
        public RoomType getRoomtype_ListValueById(Guid Id)
        {
            return dc.RoomTypes.Where(x => x.RoomTypeId == Id).FirstOrDefault();
        }
        public int SaveRoomInfo(ViewModelRoom Model)
        {
            RoomMeetingManagementDbEntities _db = new RoomMeetingManagementDbEntities();
            var userDetail = SessionHelper.GetUserDetailFromSession();
            if (userDetail == null)
            {
                return 3;
            }
            var UserId = Convert.ToInt32(userDetail.user.id);
            if (userDetail != null)
            {
                if (1 == 1)
                {
                    var data = _db.Rooms.Where(x => x.RoomName == Model.RoomName || x.RoomId == Model.RoomId).FirstOrDefault();
                    if (data == null)
                    {
                        return 0;
                    }
                    else
                    {
                        var datas = _db.Rooms.Where(x => x.RoomId == Model.RoomId).FirstOrDefault();
                        if (datas != null)
                        {
                            datas.RoomName = Model.RoomName;
                            datas.Description = Model.Description;
                            datas.Status = Model.Status;
                            datas.Capacity = Model.Capacity;
                            datas.MinCapacity = (int)Model.MinCapacity;
                            datas.MaxCapacity = (int)Model.MaxCapacity;
                            datas.CreatedBy = Model.CreatedBy;
                            datas.UpdatedBy = Convert.ToInt32(UserId);
                            datas.UpdatedDate = DateTime.Now;
                            _db.SaveChanges();//update

                            MRMLog _MRMLog = new MRMLog();
                            _MRMLog.UserId = Convert.ToInt32(UserId);
                            _MRMLog.Action = "Update";
                            _MRMLog.Module = "Room Update Operation";
                            _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has Updated Room having id= " + datas.RoomId;
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
                    var data = _db.Rooms.Where(x => x.RoomName == Model.RoomName && x.IsActive == true).FirstOrDefault();
                    if (data != null)
                    {
                        return 0;
                    }
                    else
                    {
                        Room _Roomdata = new Room();
                        Guid g = Guid.NewGuid();
                        _Roomdata.RoomId = g;
                        _Roomdata.RoomName = Model.RoomName;
                        _Roomdata.Description = Model.Description;
                        _Roomdata.Status = Model.Status;
                        _Roomdata.IsActive = true;
                        _Roomdata.Capacity = Model.Capacity;
                        _Roomdata.MinCapacity = (int)Model.MinCapacity;
                        _Roomdata.MaxCapacity = (int)Model.MaxCapacity;
                        _Roomdata.CreatedBy = Convert.ToInt32(UserId);
                        _Roomdata.UpdatedBy = Convert.ToInt32(UserId);
                        _Roomdata.CreatedDate = DateTime.Now;
                        _Roomdata.UpdatedDate = DateTime.Now;
                        _db.Rooms.Add(_Roomdata);
                        _db.SaveChanges();
                        return 1;
                    }
                }
            }
            else { return 2; }
        }
        public ViewModelRoom GetRoomDetailsById(Guid Id)
        {
            ViewModelRoom model = new ViewModelRoom();
            RoomMeetingManagementDbEntities _db = new RoomMeetingManagementDbEntities();
            List<ViewModelRoomPictureMapping> ListPicture = new List<ViewModelRoomPictureMapping>();

            var picturedata = _db.RoomPictureMappings.Where(x => x.RoomId == Id ).ToList();
            var data = _db.Rooms.Where(x => x.RoomId == Id && x.IsActive == true).FirstOrDefault();
            if (data != null)
            {
                model.DId = data.DId;
                model.RoomId = data.RoomId;
                model.FloorId = data.FloorId;
                model.RoomTypeId = data.RoomTypeId;
                model.RoomName = data.RoomName;
                model.RoomNumber = (int)data.RoomNumber;
                model.Description = data.Description;
                model.Capacity = data.Capacity;
                if (data.MinCapacity != null)
                {
                    model.MinCapacity = (int)data.MinCapacity;
                }
                if (data.MaxCapacity != null)
                {
                    model.MaxCapacity = (int)data.MaxCapacity;
                }
                model.Status = (int)data.Status;
                if (picturedata.Count > 0)
                {
                    var comCount = 0;
                    foreach (var item in picturedata)
                    {
                        ViewModelRoomPictureMapping cm = new ViewModelRoomPictureMapping();
                        comCount++;
                        cm.RoomPictureMappingId = item.RoomPictureMappingId;
                        cm.ActualFileName = item.ActualFileName;
                        cm.DId = item.DId;
                        cm.count = comCount;
                        cm.RoomId = item.RoomId;
                        cm.FileName = item.FileName;
                        cm.Extension = item.Extension;
                        ListPicture.Add(cm);
                    }
                }
                model.RoomPictureMappingsList = ListPicture;
            }
            return model;
        }
        public bool DeleteRoomInfo(Guid Id)
        {
            RoomMeetingManagementDbEntities _db = new RoomMeetingManagementDbEntities();
            var data = _db.Rooms.Where(x => x.RoomId == Id).FirstOrDefault();
            if (data != null)
            {
                data.IsActive = false;
                data.UpdatedDate = DateTime.Now;
                _db.SaveChanges();

                var userDetail = SessionHelper.GetUserDetailFromSession();
                if (userDetail == null)
                {
                    return false;
                }
                else
                {
                    var UserId = Convert.ToInt32(userDetail.user.id);
                    MRMLog _MRMLog = new MRMLog();
                    _MRMLog.UserId = Convert.ToInt32(UserId);
                    _MRMLog.Action = "Delete";
                    _MRMLog.Module = "Floor Delete Operation";
                    _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has deleted Room having id= " + data.RoomId;
                    _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                    _MRMLog.CraetedDate = DateTime.Now;
                    _db.MRMLogs.Add(_MRMLog);
                    _db.SaveChanges();
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
        public bool checkRoomReferenceID(Guid Id)
        {
            RoomMeetingManagementDbEntities _db = new RoomMeetingManagementDbEntities();
            var data = _db.BookRooms.Where(x => x.RoomId == Id && x.IsActive == true).ToList();
            if (data.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool DeleteRoomReferenceID(Guid Id)
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
                var Roomdata = _db.Rooms.Where(x => x.RoomId == Id && x.IsActive == true).ToList();
                var BookRoomdata = _db.BookRooms.Where(x => x.RoomId == Id && x.IsActive == true).ToList();
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
                        _MRMLog.Module = "Room Delete Operation";
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
                        item.UpdatedBy = UserId;
                        item.UpdatedDate = DateTime.Now;
                        _db.SaveChanges();

                        MRMLog _MRMLog = new MRMLog();
                        _MRMLog.UserId = Convert.ToInt32(UserId);
                        _MRMLog.Action = "Delete";
                        _MRMLog.Module = "Room Delete Operation";
                        _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has deleted BookRoom having id= " + item.BookRoomId;
                        _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                        _MRMLog.CraetedDate = DateTime.Now;
                        _db.MRMLogs.Add(_MRMLog);
                        _db.SaveChanges();
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool DeActivateRoomReferenceID(Guid Id)
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
                var Roomdata = _db.Rooms.Where(x => x.RoomId == Id && x.IsActive == true).ToList();
                var BookRoomdata = _db.BookRooms.Where(x => x.RoomId == Id && x.IsActive == true).ToList();
                if (Roomdata != null)
                {
                    foreach (var item in Roomdata)
                    {
                        if (item.Status == 1)
                        {
                            item.Status = 0;
                            item.UpdatedBy = UserId;
                            item.UpdatedDate = DateTime.Now;
                            _db.SaveChanges();
                            MRMLog _MRMLog = new MRMLog();
                            _MRMLog.UserId = Convert.ToInt32(UserId);
                            _MRMLog.Action = "DeActive";
                            _MRMLog.Module = "Room DeActive Operation";
                            _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has DeActive Room having id= " + item.RoomId;
                            _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                            _MRMLog.CraetedDate = DateTime.Now;
                            _db.MRMLogs.Add(_MRMLog);
                            _db.SaveChanges();
                        }
                        else
                        {
                            item.Status = 1;
                            item.UpdatedBy = UserId;
                            item.UpdatedDate = DateTime.Now;
                            _db.SaveChanges();

                            MRMLog _MRMLog = new MRMLog();
                            _MRMLog.UserId = Convert.ToInt32(UserId);
                            _MRMLog.Action = "Active";
                            _MRMLog.Module = "Room Active Operation";
                            _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has Active Room having id= " + item.RoomId;
                            _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                            _MRMLog.CraetedDate = DateTime.Now;
                            _db.MRMLogs.Add(_MRMLog);
                            _db.SaveChanges();
                        }
                        //item.UpdatedBy = UserId;
                        //item.UpdatedDate = DateTime.Now;
                        //_db.SaveChanges();            
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
                        _MRMLog.Action = "DeActive";
                        _MRMLog.Module = "Room DeActive Operation";
                        _MRMLog.Description = userDetail.user.first_name + "_" + userDetail.user.last_name + " has DeActive BookRoom having id= " + item.BookRoomId;
                        _MRMLog.CreatedBy = Convert.ToInt32(UserId);
                        _MRMLog.CraetedDate = DateTime.Now;
                        _db.MRMLogs.Add(_MRMLog);
                        _db.SaveChanges();
                    }
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