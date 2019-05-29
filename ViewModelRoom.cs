using MRM.DbEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MRM.Web.ViewModel
{
    [Table("Room")]
    public class ViewModelRoom
    {
        [Key]
        public Guid RoomId { get; set; }
    
         [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
          public int DId { get; set; }
        [Required(ErrorMessage = "Floor is required")]
        public Guid FloorId { get; set; }
        [Required(ErrorMessage = "RoomType is required")]
        public Guid RoomTypeId { get; set; }
        
        public string Floor { get; set; }
        public string Roomtype { get; set; }
        [Required(ErrorMessage = "RoomName is required")]
        public string RoomName { get; set; }
        public int RoomNumber { get; set; }
        [Required(ErrorMessage = "Capacity is required")]
        public int Capacity { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "MinCapacity is required")]
        public int MinCapacity { get; set; }
        [Required(ErrorMessage = "MaxCapacity is required")]
        public int MaxCapacity { get; set; }
        public bool IsActive { get; set; }
        public int Status { get; set; }
        public int Count { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsExist { get; set; }
        //public virtual ViewModelRoom Room { get; set; }
        // public int IsEdit { get; set; }
        public ICollection<RoomtypeModelClass> RoomtypeModelList { get; set; }
        public ICollection<FloorModelClass> FloorModelList { get; set; }
        public virtual ICollection<ViewModelRoomPictureMapping> RoomPictureMappings { get; set; }
        //public virtual ICollection<ViewModelRoom> RoomDetailList { get; set; }
         public IList<ViewModelRoomPictureMapping> RoomPictureMappingsList { get; set; }
        public ViewModelRoom()
        {
            RoomPictureMappingsList = new List<ViewModelRoomPictureMapping>();
        }
    }
    public class RoomtypeModelClass
    {
        [Key]
        public string Value { get; set; }
        public string Text { get; set; }
    }
    public class FloorModelClass
    {
        [Key]
        public string Value { get; set; }
        public string Text { get; set; }
    } 
   
}