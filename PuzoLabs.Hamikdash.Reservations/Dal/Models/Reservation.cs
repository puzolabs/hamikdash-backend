using System;

namespace PuzoLabs.Hamikdash.Reservations.Dal.Models
{
    public enum ReservationStatus
    {
        Pending,
        InWork,
        Done
    }

    public class Reservation
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int AltarId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public Guid UserId { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
