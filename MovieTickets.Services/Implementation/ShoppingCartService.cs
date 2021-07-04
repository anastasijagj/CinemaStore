using MovieTickets.Domain.DomainModels;
using MovieTickets.Domain.DTO;
using MovieTickets.Repository.Interface;
using MovieTickets.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieTickets.Services.Implementation
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IRepository<ShoppingCart> _shoppingCartRepositorty;
        private readonly IRepository<Order> _orderRepositorty;
        private readonly IRepository<TicketInOrder> _ticketInOrderRepositorty;
        private readonly IUserRepository _userRepository;
        private readonly IRepository<EmailMessage> _mailRepository;
        public ShoppingCartService(IRepository<EmailMessage> mailRepository, IRepository<ShoppingCart> shoppingCartRepository, IRepository<TicketInOrder> ticketInOrderRepositorty, IRepository<Order> orderRepositorty, IUserRepository userRepository)
        {
            _shoppingCartRepositorty = shoppingCartRepository;
            _userRepository = userRepository;
            _orderRepositorty = orderRepositorty;
            _ticketInOrderRepositorty = ticketInOrderRepositorty;
            _mailRepository = mailRepository;
        }
        public bool deleteTicketFromShoppingCart(string userId, Guid ticketId)
        {
            if (!string.IsNullOrEmpty(userId) && ticketId != null)
            {
                var user = this._userRepository.Get(userId);

                var userShoppingCart = user.UserCart;

                var ticketForRemoval = userShoppingCart.TicketInShoppingCarts.Where(z => z.Ticket.Id == ticketId).FirstOrDefault();

                userShoppingCart.TicketInShoppingCarts.Remove(ticketForRemoval);

                this._shoppingCartRepositorty.Update(userShoppingCart);

                return true;
            }
            return false;
        }

        public ShoppingCartDto getShoppingCartInfo(string userId)
        {
            var user = this._userRepository.Get(userId);

            var userShoppingCart = user.UserCart;

            var ticketList = userShoppingCart.TicketInShoppingCarts.Select(z => new
            {
                TicketPrice = z.Ticket.Price,
                Quantity = z.Quantity
            }).ToList();

            var totalPrice = 0;
            foreach (var item in ticketList)
            {
                totalPrice += item.Quantity * item.TicketPrice;
            }

            //go polnime Dto so site dodadeni tiketi na korisnikot vo ShoppingCart i totalnata suma od niv
            ShoppingCartDto model = new ShoppingCartDto
            {
                TotalPrice = totalPrice,
                Tickets = userShoppingCart.TicketInShoppingCarts.ToList()
            };

            return model;
        }

        public bool orderNow(string userId)
        {
            if(!string.IsNullOrEmpty(userId))
            {
                var user = _userRepository.Get(userId);

                var userShoppingCart = user.UserCart;

                EmailMessage mail = new EmailMessage();
                mail.MailTo = user.Email;
                mail.Subject = "Successfully created order";
                mail.Status = false;

                //kreirame nov order 
                Order newOrder = new Order
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    User = user,
                };

                this._orderRepositorty.Insert(newOrder);

                List<TicketInOrder> ticketsInOrders = new List<TicketInOrder>();

                var result = ticketsInOrders = userShoppingCart.TicketInShoppingCarts.Select(z => new TicketInOrder
                {
                    Id = Guid.NewGuid(),
                    OrderId = newOrder.Id,
                    TicketId = z.TicketId,
                    SelectedTicket = z.Ticket,
                    UserOrder = newOrder,
                    Quantity=z.Quantity
                }).ToList();

                StringBuilder sb = new StringBuilder();

                var totalPrice = 0;

                sb.AppendLine("Your order is completed. The order conains: ");

                for (int i = 1; i <= result.Count(); i++)
                {
                    var item = result[i - 1];

                    totalPrice += item.Quantity * item.SelectedTicket.Price;

                    sb.AppendLine(i.ToString() + ". " + item.SelectedTicket.MovieName + " with price of: " + item.SelectedTicket.Price + " and quantity of: " + item.Quantity);
                }

                sb.AppendLine("Total price: " + totalPrice.ToString());


                mail.Content = sb.ToString();

                foreach (var item in ticketsInOrders)
                {
                    this._ticketInOrderRepositorty.Insert(item);
                }

                //ja praznime ShoppingCart posle Order
                userShoppingCart.TicketInShoppingCarts.Clear();

                this._userRepository.Update(user);
                this._mailRepository.Insert(mail);

                return true;
            }
            return false;
        }
    }
}
