using System;

namespace Tic_tac_toe.Models
{
    public class Move
    {
        public User User { get; set; }

        public int BoxPosition { get; set; }

        public DateTime Date { get; set; }

        public Guid Id { get; set; }

        public Move(User user, int boxPosition)
        {
            User = user;
            BoxPosition = boxPosition;
            Date = DateTime.Now;
            Id = Guid.NewGuid();
        }

        public Move()
        {

        }
    }
}