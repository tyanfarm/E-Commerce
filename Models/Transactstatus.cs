using System;
using System.Collections.Generic;

namespace E_Commerce.Models;

public partial class Transactstatus
{
    public int TransactStatusId { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public virtual ICollection<Order> Orders { get; } = new List<Order>();
}
