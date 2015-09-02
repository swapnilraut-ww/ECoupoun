using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECoupoun.Data
{
    [MetadataType(typeof(CategoryModel))]
    public partial class Category
    {
        
    }

    public class CategoryModel
    {
        [Display(Name = "Parent Category")]        
        public int CategoryParentId { get; set; }

        [Display(Name = "Active")]
        [Required]
        public bool IsActive { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Description")]
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Display(Name = "Mapping Name")]
        [Required]
        public string MappingName { get; set; }
    }
}
