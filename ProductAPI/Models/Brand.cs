﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace ProductAPI.Models;

public partial class Brand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; }

    public virtual ICollection<Product> Product { get; set; } = new List<Product>();
}