using Microsoft.EntityFrameworkCore;
using MovieTickets.Domain.Identity;
using MovieTickets.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieTickets.Repository.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private DbSet<MovieTicketsApplicationUser> entities;
        string errorMessage = string.Empty;

        public UserRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<MovieTicketsApplicationUser>();
        }
        public IEnumerable<MovieTicketsApplicationUser> GetAll()
        {
            return entities.AsEnumerable();
        }

        public MovieTicketsApplicationUser Get(string id)
        {
            return entities
               .Include(z => z.UserCart)
               .Include("UserCart.TicketInShoppingCarts")
               .Include("UserCart.TicketInShoppingCarts.Ticket")
               .SingleOrDefault(s => s.Id == id);
        }
        public void Insert(MovieTicketsApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Add(entity);
            context.SaveChanges();
        }

        public void Update(MovieTicketsApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Update(entity);
            context.SaveChanges();
        }

        public void Delete(MovieTicketsApplicationUser entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entities.Remove(entity);
            context.SaveChanges();
        }
    }
}
