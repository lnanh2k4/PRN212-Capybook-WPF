using System;
using System.Collections.Generic;

namespace Capybook.Models;

public partial class Supplier
{
    public int SupId { get; set; }

    public string? SupName { get; set; }

    public string? SupEmail { get; set; }

    public string? SupPhone { get; set; }

    public string? SupAddress { get; set; }

    public int? SupStatus { get; set; }
}
