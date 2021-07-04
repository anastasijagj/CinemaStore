using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieTickets.Domain.DomainModels
{
    public class Ticket : BaseEntity
    {
        public string Image { get; set; }
        [Required]
        [Display(Name = "Movie Name")]
        public string MovieName { get; set; }
        [Required]
        [Display(Name = "Movie Description")]
        public string MovieDescription { get; set; }
        [Required]
        public int Price { get; set; }
        [Required]
        public string Genre { get; set; }
        [Required]
        public DateTime Date { get; set; }
        public virtual ICollection<TicketInOrder> Orders { get; set; }
        public virtual ICollection<TicketInShoppingCart> TicketInShoppingCarts { get; set; }
    }
}
