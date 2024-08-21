using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class ApplicationUser : Repository<Applicationuser>, IApplicationUser
    {
        public string StreetAddress;
        public string City;
        public string State;
        public string PostalCode;
        public string Name;
        public string PhoneNumber;
        private ApplicationDbContext _db;

        public ApplicationUser(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
