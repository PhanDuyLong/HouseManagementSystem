using HMS.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HMS.Data.ViewModels
{
    public class CreateServiceViewModel
    {
        
        [Required]
        public string HouseId { get; set; }
        public string Name { get; set; }
        public string CalculationUnit { get; set; }
        public double? Price { get; set; }

        [Required]
        public string ServiceTypeName { get; set; }
    }
}
