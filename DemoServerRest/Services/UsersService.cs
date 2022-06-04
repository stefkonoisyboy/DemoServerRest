using DemoServerRest.Data;
using DemoServerRest.Data.Models;
using DemoServerRest.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoServerRest.Services
{
    public class UsersService : IUsersService
    {
        private readonly ApplicationDbContext dbContext;

        public UsersService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public ApplicationUser Create(ApplicationUser user)
        {
            this.dbContext.Users.Add(user);
            user.Id = this.dbContext.SaveChanges();

            return user;
        }

        public bool Exists(string email)
        {
            return this.dbContext.Users.Any(u => u.Email == email);
        }

        public ApplicationUser GetByEmail(string email)
        {
            return this.dbContext.Users.FirstOrDefault(u => u.Email == email);
        }

        public ApplicationUser GetById(int id)
        {
            return this.dbContext.Users.FirstOrDefault(u => u.Id == id);
        }
    }
}
