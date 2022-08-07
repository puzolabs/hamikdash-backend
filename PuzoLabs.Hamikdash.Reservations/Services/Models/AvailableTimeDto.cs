using System;

namespace PuzoLabs.Hamikdash.Reservations.Services.Models
{
    public record AvailableTimeDto
    {
        public int AltarId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
