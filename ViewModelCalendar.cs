using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MRM.Web.ViewModel
{
    [Table("BookRoom")]
    public class ViewModelCalendar
    {
        [Key]
        public Guid BookRoomId { get; set; }
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int DId { get; set; }
        public string Floor { get; set; }
        public string Room { get; set; }
        public int Capacity { get; set; }
        public string RoomType { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Description { get; set; }
        public Guid FloorId { get; set; }
        public Guid RoomTypeId { get; set; }
        public Guid RoomId { get; set; }
        public int RoomNumber { get; set; }
        public int Count { get; set; }
        //public int MinCapacity { get; set; }
        //public int MaxCapacity { get; set; }
        public bool IsSyncGoogleCal { get; set; }
        public bool IsSyncOutlookCal { get; set; }
        public bool IsSyncIcal { get; set; }
        //public ViewModelRoom RoomDetails { get; set; }
        public virtual ICollection<ViewModelRoomPictureMapping> RoomPictureMappings { get; set; }
       // public virtual ICollection<ViewModelRoomPictureMapping> RoomMappings { get; set; }
        public IList<ViewModelRoomPictureMapping> RoomMappings { get; set; }
        //public IList<SelectListItem> FloorList { get; set; }
        //public IList<SelectListItem> RoomTypeList { get; set; }
        //public IList<SelectListItem> RoomList { get; set; }
        //public virtual ICollection<ViewModelRoom> RoomMappings { get; set; }
        //public ICollection<RoomtypeModelClass> RoomtypeModelList { get; set; }
        //public ICollection<FloorModelClass> FloorModelList { get; set; }
        public IList<ViewModelRoom> RoomDetailList { get; set; }
        public IList<ViewModelCalendar> RoomDetailListCal { get; set; }
        public ViewModelCalendar()
        {
            RoomDetailList = new List<ViewModelRoom>();
            RoomDetailListCal = new List<ViewModelCalendar>();
            
        }
        //public ViewModelCalendar() {
        //    FloorList = new List<SelectListItem>();
        //    RoomTypeList = new List<SelectListItem>();
        //    RoomList = new List<SelectListItem>();
        //}  
    }
}