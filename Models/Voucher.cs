using System;
using System.Collections.Generic;

namespace Capybook.Models;

public partial class Voucher
{
    public int VouId { get; set; }

    public string? VouName { get; set; }

    public string? VouCode { get; set; }

    public double? Discount { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? Quantity { get; set; }

    public int? VouStatus { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
