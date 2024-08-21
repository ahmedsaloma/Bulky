using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepsitory Product { get; }

        IShoppingCartRepository shoppingCart { get; }

        ICompanyRepository Company { get; }

        IApplicationUser applicationUser { get; }

        IOrderDetailRepository OrderDetail { get; } 

        IOrderHeaderRepository OrderHeader { get; }    
        

        void Save();
    }
}
