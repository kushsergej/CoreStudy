using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreStudy.Models
{
    //class uses for Identity
    public class ApplicationIdentityContext: IdentityDbContext<IdentityUser>
    {
        #region Database initialisation
        public ApplicationIdentityContext(DbContextOptions<ApplicationIdentityContext> options): base(options)
        {
            Database.EnsureCreated();
        }
        #endregion
    }
}
