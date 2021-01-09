using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UdemyNLayerProject.Core.Repositories
{
    public interface IRepository<TEntity> where TEntity:class // IEntity mutlaka bir class olmak zorundadır.
    {
        // id göre nesneyi getir
        Task<TEntity> GetByIdAsync(int id);

        //tüm nesneleri getir
        Task<IEnumerable<TEntity>> GetAllAsync();

        //herhangi bir paremetreye göre ilgili nesneleri bul
        Task<IEnumerable<TEntity>> Find(Expression<Func<TEntity, bool>> predicate);//TEntity alan geriye bool dönen bir method(Func ve predicate bir delegedir)

        //herhangi bir paremetreye göre örneğin product ın innerbarcode u şu olan ürünü döndür(1 ürün olacak)
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        //ekleme işlemi
        Task AddAsync(TEntity entity);

        //toplu ekleme işlemi
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        //silme işlemi
        Task Remove(TEntity entity);

        //toplu silme işlemi
        Task RemoveRange(IEnumerable<TEntity> entities);

        //güncelleme işlemi
        TEntity Update(TEntity entity);

    }
}
