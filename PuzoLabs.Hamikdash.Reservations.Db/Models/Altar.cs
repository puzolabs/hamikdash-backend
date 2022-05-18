using System;

namespace PuzoLabs.Hamikdash.Reservations.Db.Models
{
    public class Altar
    {
        public int Id { get; set; }
        public bool IsAvailable { get; set; } // nitma or broken or dirty or available
    }
}
