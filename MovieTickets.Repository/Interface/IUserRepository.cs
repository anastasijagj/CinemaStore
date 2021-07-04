using MovieTickets.Domain.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieTickets.Repository.Interface
{
    public interface IUserRepository
    {
        IEnumerable<MovieTicketsApplicationUser> GetAll();
        MovieTicketsApplicationUser Get(string id);
        void Insert(MovieTicketsApplicationUser entity);
        void Update(MovieTicketsApplicationUser entity);
        void Delete(MovieTicketsApplicationUser entity);
    }
}
