using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MvcCore.Data
{
    public partial class Blog
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("دسته بندی")]
        public int CategoryId { get; set; }


        [Required]
        [MaxLength(30)]
        [DisplayName("نام وبلاگ")]
        public string Name { get; set; }


        [Required]
        [DisplayName("تاریخ اعتبار")]
        public DateTime MaxDate { get; set; }


        [Required]
        [DisplayName("حداکثر پست")]
        public int MaxPostCount { get; set; }

        [DisplayName("تاریخ ثبت")]
        public DateTime InsertDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public virtual Category Category { get; set; }
    }
}