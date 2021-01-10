using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UdemyNLayerProject.Core.Repositories;
using UdemyNLayerProject.Core.UnitOfWorks;
using UdemyNLayerProject.Data.Repositories;

namespace UdemyNLayerProject.Data.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _contex;

        public UnitOfWork(AppDbContext appDbContext)
        {
            _contex = appDbContext;
        }


        private ProductRepository _productRepository;
        private CategoryRepository _categoryRepository;

        public IProductRepository Product => _productRepository = _productRepository ?? new ProductRepository(_contex);

        public ICategoryRepository Categories => _categoryRepository = _categoryRepository ?? new CategoryRepository(_contex);


        public void Commit()//alternatif isim SaveChange()
        {
            _contex.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await _contex.SaveChangesAsync();
        }
    }
}
