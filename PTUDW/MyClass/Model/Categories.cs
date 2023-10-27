using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClass.Model
{
    [Table("Categories")]
    public class Categories
    {
        [Key]
        public int Id { get; set; }

        [Display(Name="Tên loại SP")]
        [Required(ErrorMessage = "Tên loại sản phẩm không để rỗng")]
        public string Name { get; set; }

        [Display(Name = "Tên rút gọn")]
        public string Slug { get; set; }

        [Display(Name = "Cấp cha")]
        public int? ParentID { get; set; }

        [Display(Name = "Sắp xếp")]
        public int? Order { get; set; }

        [Required(ErrorMessage = "Mô tả không để trống")]
        [Display(Name = "Mô tả")]
        public string MetaDesc { get; set; }

        [Required(ErrorMessage = "Từ khóa không để rỗng")]
        [Display(Name = "Từ khóa")]
        public string MetaKey { get; set; }

        [Display(Name = "Tạo bởi")]
        [Required(ErrorMessage = "Người tạo không để rỗng")]
        public int CreateBy { get; set; }

        [Display(Name = "Ngày tạo")]
        [Required(ErrorMessage = "Ngày tạo không để rỗng")]
        public DateTime CreateAt { get; set; }

        [Display(Name = "Cập nhật bởi")]
        [Required(ErrorMessage = "Người cập nhật không để rỗng")]
        public int UpdateBy { get; set; }

        [Display(Name = "Ngày cập nhật")]
        [Required(ErrorMessage = "Ngày cập nhật không để rỗng")]
        public DateTime UpdateAt { get; set; }

        [Display(Name = "Trạng thái")]
        [Required(ErrorMessage = "Trạng thái không để rỗng")]
        public int Status { get; set; }

    }
}
