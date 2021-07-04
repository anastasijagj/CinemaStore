using MovieTickets.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTickets.Domain.DomainModels
{
    public class Order : BaseEntity
    {
        public string UserId { get; set; }
        public MovieTicketsApplicationUser User { get; set; }
        public virtual ICollection<TicketInOrder> Tickets { get; set; }
    }
}
