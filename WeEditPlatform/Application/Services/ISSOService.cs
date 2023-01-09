using Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

namespace Application.Services
{
    public interface ISSOService
    {
        [Post("/api/account/roles")]
        public Task UpdateRoles(AddRolesVM addRolesVM);
    }
}
