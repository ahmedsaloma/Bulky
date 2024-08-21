using Bulky.DataAccess.Data;
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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private ApplicationDbContext _db;

        public OrderHeaderRepository(ApplicationDbContext db ) : base( db )
        {
            _db = db;
        }

       

        public void Update(OrderHeader obj)
        {
            _db.orderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string orderSatatus, string? paymentStatus = null)
        {
            var ordeFromDb = _db.orderHeaders.FirstOrDefault(u => u.Id == id);
            if ((ordeFromDb != null))
            {
                ordeFromDb.OrderStatus = orderSatatus;
                if(!string.IsNullOrEmpty(paymentStatus))
                {
                    ordeFromDb.PaymentStatus = paymentStatus;   
                }

            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var ordeFromDb = _db.orderHeaders.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                ordeFromDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                ordeFromDb.PaymentIntentId = paymentIntentId;
                ordeFromDb.OrderDate = DateTime.Now;    
            }

        }
    }
}
