using HMS.Data.Models;
using System;
using System.Collections.Generic;

namespace HMS.Data.ViewModels
{
    public class ServiceViewModel
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string CalculationUnit { get; set; }
        public bool? Status { get; set; }
        public string HouseId { get; set; }
        public double Price { get; set; }

        public string ServiceType { get; set; }

    }
}
