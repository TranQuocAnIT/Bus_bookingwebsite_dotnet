﻿namespace DOANCOSO26.Models
{
    public class GPLXImage
    {
        public int Id { get; set; }
        public string Url { get; set; } 
        public int DriverregisId { get; set; }
        public Driverregis? Driverregis { get; set; }
    }
}
