using Domain;
using Infrastructure.Persistences;
using System.Linq.Expressions;

namespace Infrastructure.Repository
{
    public interface IRepositoryService
    {
        IDatabaseService Database { get; set; }

        /// <summary>
        /// Reserve to create an entity. Call SaveChanges to save any creating items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Add<T>(T entity) where T : EntityBase;

        /// <summary>
        /// Reserve to create a array of entities. Call SaveChanges to save any creating items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        List<T> Add<T>(params T[] entities) where T : EntityBase;

        List<T> All<T>() where T : EntityBase;
        List<T> All<T>(out int totalRecords) where T : EntityBase;

        T Find<T>(int id) where T : EntityBase;

        T Find<T>(int id, SpecificationBase<T> specification) where T : EntityBase;

        T Find<T>(Expression<Func<T, bool>> where) where T : EntityBase;
        T Find<T>(string rawSql) where T : EntityBase;

        List<T> List<T>(int[] Ids) where T : EntityBase;
        List<T> List<T>(int[] Ids, out int[] invalidIds) where T : EntityBase;
        List<T> List<T>(int[] Ids, SpecificationBase<T> specification) where T : EntityBase;

        List<T> List<T>(Expression<Func<T, bool>> where) where T : EntityBase;
        List<T> List<T>(string rawSql) where T : EntityBase;

        IQueryable<T> ListAsQueryable<T>(Expression<Func<T, bool>> where) where T : EntityBase;

        IQueryable<T> ListAsQueryable<T>(int[] Ids) where T : EntityBase;

        List<T> List<T>(int pageIndex, int pageSize, out int totalPage) where T : EntityBase;

        List<T> List<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> where) where T : EntityBase;

        List<T> List<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> where, out int totalPage) where T : EntityBase;

        IQueryable<T> ListAsQueryable<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> where) where T : EntityBase;

        /// <summary>
        /// Reserve to update a list of entities. Call SaveChanges to save any updating items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        //List<T> Update<T>(params T[] entities) where T : EntityBase;
        List<T> Update<T>(params T[] entities) where T : EntityBase;
        T Update<T>(T entity) where T : EntityBase;

        /// <summary>
        /// Reserve to delete entity. Call SaveChanges to save any deleting items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        bool Delete<T>(T entity) where T : EntityBase;

        /// <summary>
        /// Reserve to delete list of entities. Call SaveChanges to save any deleting items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        bool Delete<T>(T[] entities) where T : EntityBase;

        /// <summary>
        /// Reserve to delete entity. Call SaveChanges to save any deleting items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Id"></param>
        /// <returns></returns>
        bool Delete<T>(int id) where T : EntityBase;

        /// <summary>
        /// Reserve to delete any entity match with condition. Call SaveChanges to save any deleting items.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <returns></returns>
        bool Delete<T>(Expression<Func<T, bool>> where) where T : EntityBase;

        int CountWhere<T>(Expression<Func<T, bool>> @where) where T : EntityBase;

        int CountAll<T>() where T : EntityBase;

        IEnumerable<T> Find<T>(SpecificationBase<T> specification) where T : EntityBase;
        IEnumerable<T> Find<T>(int pageIndex, int pageSize, SpecificationBase<T> specification, out int totalPage) where T : EntityBase;

        /// <summary>
        /// Save all changes of any reservations
        /// </summary>
        /// <returns></returns>
        bool SaveChanges();
    }
}
