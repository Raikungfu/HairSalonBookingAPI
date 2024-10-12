﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BusinessObject.Models
{
    public partial class Booking
    {
        public Booking()
        {
            BookingDetails = new HashSet<BookingDetail>();
            Payments = new HashSet<Payment>();
        }

        public int BookingId { get; set; }
        public double? TotalPrice { get; set; }
        public int? VoucherId { get; set; }
        public int? ManagerId { get; set; }
        public int? CustomerId { get; set; }
        public int? StaffId { get; set; }
        public int? ReportId { get; set; }
        public int? ScheduleId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? CreateBy { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string? UpdateBy { get; set; }

        public virtual User Customer { get; set; } = null!;
        public virtual User Manager { get; set; } = null!;
        public virtual Report? Report { get; set; }
        public virtual Schedule? Schedule { get; set; }
        public virtual User Staff { get; set; } = null!;
        public virtual Voucher? Voucher { get; set; }
        [JsonIgnore]
        public virtual ICollection<BookingDetail> BookingDetails { get; set; }
        [JsonIgnore]
        public virtual ICollection<Payment> Payments { get; set; }
    }
}