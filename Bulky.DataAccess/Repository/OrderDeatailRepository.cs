﻿using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository
{
    public class OrderDeatailRepository : Repository<OrderDetail>,IOrderDetailRepository
    {
        private ApplicationDbContext _db;

        public OrderDeatailRepository(ApplicationDbContext db ) : base( db )
        {
            _db = db;
        }

       

        public void Update(OrderDetail obj)
        {
            _db.orderDetails.Update(obj);
        }
    }
}
