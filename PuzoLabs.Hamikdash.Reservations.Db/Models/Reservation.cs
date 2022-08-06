using System;

namespace PuzoLabs.Hamikdash.Reservations.Db.Models
{
    public enum KorbanTypes
    {
        Ola,
        Minha,
        Shlamim,
        AshamTaluy,
        AshamVaday,
        Bicurim,
    }
    public enum ReservationStatus
    {
        Pending,
        InWork,
        Done
    }

    public record Reservation
    {
        public Guid Id { get; set; }
        public KorbanTypes Type { get; set; }
        public int AltarId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid UserId { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
