using System;
using System.Collections.Generic;

namespace Capybook.Models;

public partial class Account
{
    public string Username { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Password { get; set; }

    public DateOnly? Dob { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public int? Role { get; set; }

    public string? Address { get; set; }

    public int? Sex { get; set; }

    public int? AccStatus { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
