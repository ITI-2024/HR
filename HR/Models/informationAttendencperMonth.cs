﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HR.Models
{
    public class informationAttendencperMonth
    {
        [Key]
        public DateOnly Monthofyear { get; set; }
        
        public int extraTime { get; set; }

        public int discountTime { get; set; }

        public double totalNetSalary { get; set; }
        
        public int attendofDay {  get; set; }


        [ForeignKey("Emp")]
        public string? idemp { get; set; }
        [JsonIgnore]
        public virtual Employee? Emp { get; set; }




    }
}