﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RentBrandOutfit.Models;

public partial class Location
{
    public int LocationId { get; set; }

    public string LocationDescription { get; set; }

    public virtual ICollection<Client> Client { get; set; } = new List<Client>();
}