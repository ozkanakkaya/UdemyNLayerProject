using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UdemyNLayerProject.Core.Models;

namespace UdemyNLayerProject.Core.Repositories
{
    public interface IProductRepository:IRepository<Category>
    {
        Task<Product> GetWithCategoryByIdAsync(int productId);
    }
}
