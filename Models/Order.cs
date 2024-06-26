﻿using System;
using System.Collections.Generic;

namespace E_Commerce.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime? OrderDate { get; set; }

    public DateTime? ShipDate { get; set; }

    public int? TransactStatusId { get; set; }

    public bool? Deleted { get; set; }

    public bool? Paid { get; set; }

    public DateTime? PaymentDate { get; set; }

    public int? PaymentId { get; set; }

    public string? Note { get; set; }

    public string? Address { get; set; }

    public double? TotalMoney { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<Orderdetail> Orderdetails { get; } = new List<Orderdetail>();

    public virtual Transactstatus? TransactStatus { get; set; }
}
