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
    public class UnitOfWork : IUnitOfWork
    {
        public ICategoryRepository Category { get; private set; }
        public IProductRepsitory Product {  get; private set; }

        public ICompanyRepository Company { get; private set; }
        public IShoppingCartRepository shoppingCart { get; private set; }

        public IApplicationUser applicationUser { get; private set; }

       

        public IOrderDetailRepository OrderDetail { get; private set; }

        public IOrderHeaderRepository OrderHeader { get; private set; }

        private ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            Category = new CategoryRepository(_db);
            Product = new ProductRepsitory(_db);
            Company = new CompanyRepository(_db);
            shoppingCart = new ShoppingCartRepository(_db);
            applicationUser = new ApplicationUser(_db);
            OrderDetail = new OrderDeatailRepository(_db);

            OrderHeader = new OrderHeaderRepository(_db);
        }  

       

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
