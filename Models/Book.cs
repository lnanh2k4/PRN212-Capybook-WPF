using System;
using System.Collections.Generic;

namespace Capybook.Models;

public partial class Book
{
    public int BookId { get; set; }

    public int? CatId { get; set; }

    public string? BookTitle { get; set; }

    public string? Author { get; set; }

    public string? Translator { get; set; }

    public string? Publisher { get; set; }

    public int? PublicationYear { get; set; }

    public string? Isbn { get; set; }

    public string? Image { get; set; }

    public string? BookDescription { get; set; }

    public int? Hardcover { get; set; }

    public string? Dimension { get; set; }

    public double? Weight { get; set; }

    public decimal? BookPrice { get; set; }

    public int? BookQuantity { get; set; }

    public int? BookStatus { get; set; }

    public virtual Category? Cat { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
