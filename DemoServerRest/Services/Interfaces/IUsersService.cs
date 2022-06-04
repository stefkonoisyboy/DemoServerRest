using DemoServerRest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoServerRest.Services.Interfaces
{
    public interface IUsersService
    {
        ApplicationUser Create(ApplicationUser user);

        ApplicationUser GetByEmail(string email);

        ApplicationUser GetById(int id);

        bool Exists(string email);
    }
}
