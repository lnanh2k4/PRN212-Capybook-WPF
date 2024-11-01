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
}
