using System;
using System.Collections.Generic;

namespace Capybook.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? VouId { get; set; }

    public string? Username { get; set; }

    public int? StaffId { get; set; }

    public DateOnly? OrderDate { get; set; }

    public int? OrderStatus { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Account? UsernameNavigation { get; set; }

    public virtual Voucher? Vou { get; set; }
}
