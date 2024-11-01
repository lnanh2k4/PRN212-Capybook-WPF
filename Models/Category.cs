using System;
using System.Collections.Generic;

namespace Capybook.Models;

public partial class Category
{
    public int CatId { get; set; }

    public string? CatName { get; set; }

    public int? ParentCatId { get; set; }

    public int? CatStatus { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();

    public virtual ICollection<Category> InverseParentCat { get; set; } = new List<Category>();

    public virtual Category? ParentCat { get; set; }

    public string ParentCategoryName
    {
        get
        {
            using (var context = new Prn212ProjectCapybookContext())
            {
                var parentCategory = context.Categories.FirstOrDefault(c => c.CatId == ParentCatId);
                return parentCategory != null ? parentCategory.CatName : "N/A";
            }
        }
    }
}
