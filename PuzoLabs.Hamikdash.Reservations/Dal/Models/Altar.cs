using System;

namespace PuzoLabs.Hamikdash.Reservations.Dal.Models
{
    public class Altar
    {
        public Guid Id { get; set; }
        public bool IsAvailable { get; set; } // nitma or broken or dirty or available
    }
}
