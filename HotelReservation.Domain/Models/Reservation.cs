﻿
namespace HotelReservation.Domain.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int ReferenceceNo { get; set; }
        public bool IsFeaturedDeal { get; set; }
        public double Price { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}