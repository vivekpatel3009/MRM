using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MRM.Web.ViewModel
{
    [Table("Floor")]
    public class ViewModelFloor
    {       
        [Key]
        public Guid FloorId { get; set; }
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int DId { get; set; }
        [Required(ErrorMessage = "FloorName is required")]
        public string FloorName{ get; set; }
        public int FloorNumber { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public bool IsActive { get; set; }
        public int Count { get; set; }
        public int CreatedBy { get; set; }
        public int UpdatedBy{ get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        //public int IsEdit { get; set; }
        //public string Message { get; set; }
    }
}