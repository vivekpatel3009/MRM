using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MRM.Web.ViewModel
{
    [Table("RoomPictureMapping")]
    public class ViewModelRoomPictureMapping
    {
        [Key]
        public Guid RoomPictureMappingId { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int DId { get; set; }
        public int count { get; set; }
        public Guid RoomId { get; set; }
        public string FileName { get; set; }
        public string ActualFileName { get; set; }
        public string Extension { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public virtual ViewModelRoom Room { get; set; }
        //public virtual ViewModelCalendar Calendar { get; set; }
    }
}