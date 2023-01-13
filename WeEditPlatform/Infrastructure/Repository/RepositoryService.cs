using Domain;
using Infrastructure.Persistences;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repository
{
    public class RepositoryService : IRepositoryService
    {
        public IDatabaseService Database { get; set; }

        public RepositoryService(IDatabaseService database)
        {
            Database = database;
        }

        public T Add<T>(T entity) where T : EntityBase
        {
            entity.DateCreated = DateTime.UtcNow;
            Database.GetDbSet<T>().Add(entity);
            return entity;
        }

        public List<T> Add<T>(params T[] entities) where T : EntityBase
        {
            var dbset = Database.GetDbSet<T>();
            foreach (var entity in entities)
            {
                entity.DateCreated = DateTime.UtcNow;
                dbset.Add(entity);
            }

            return entities.ToList();
        }

        public T Find<T>(int id) where T : EntityBase
        {
            return Database.GetDbSet<T>().FirstOrDefault(x => x.Id.Equals(id));
        }

        public T Find<T>(int id, SpecificationBase<T> specification) where T : EntityBase
        {
            var query = Database.GetDbSet<T>().AsQueryable();

            if (specification.IsInclude)
            {
                query = AsQueryInClude(specification);
            }
            else
            {
                query = query.Where(specification.Criteria);
            }

            return query.FirstOrDefault(w => w.Id.Equals(id));
        }


        public T Find<T>(Expression<Func<T, bool>> @where) where T : EntityBase
        {
            return Database.GetDbSet<T>().FirstOrDefault(@where);
        }

        public List<T> List<T>(int[] Ids) where T : EntityBase
        {
            return Database.GetDbSet<T>().Where(x => Ids.Contains(x.Id)).ToList();
        }

        public List<T> List<T>(int[] Ids, SpecificationBase<T> specification) where T : EntityBase
        {
            var query = Database.GetDbSet<T>().AsQueryable();

            if (specification.IsInclude)
            {
                query = AsQueryInClude(specification);
            }
            else
            {
                query = query.Where(specification.Criteria);
            }

            return query.Where(w => Ids.Contains(w.Id)).ToList();
        }

        public List<T> List<T>(int[] Ids, out int[] invalidIds) where T : EntityBase
        {
            var result = Database.GetDbSet<T>().Where(x => Ids.Contains(x.Id)).ToList();
            invalidIds = Ids.Except(result.Select(i => i.Id).ToArray()).ToArray();

            return result;
        }

        public List<T> List<T>(Expression<Func<T, bool>> @where) where T : EntityBase
        {
            return Database.GetDbSet<T>().Where(@where).ToList();
        }

        public IQueryable<T> ListAsQueryable<T>(Expression<Func<T, bool>> @where) where T : EntityBase
        {
            var dbset = Database.GetDbSet<T>();
            if (@where is null)
                return dbset;

            return dbset.Where(@where);
        }

        public IQueryable<T> ListAsQueryable<T>(int[] Ids) where T : EntityBase
        {
            return Database.GetDbSet<T>().Where(x => Ids.Contains(x.Id));
        }

        public List<T> List<T>(int pageIndex, int pageSize, out int totalPage) where T : EntityBase
        {
            var dbset = Database.GetDbSet<T>();
            totalPage = dbset.Count();

            return dbset.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<T> List<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> @where) where T : EntityBase
        {
            return Database.GetDbSet<T>().Where(@where).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public List<T> List<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> @where, out int totalPage) where T : EntityBase
        {
            var query = Database.GetDbSet<T>().Where(@where);
            totalPage = query.Count();

            return query.OrderByDescending(x => x.DateCreated).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public IQueryable<T> ListAsQueryable<T>(int pageIndex, int pageSize, Expression<Func<T, bool>> @where) where T : EntityBase
        {
            return Database.GetDbSet<T>().Where(@where).Skip((pageIndex - 1) * pageSize).Take(pageSize);
        }

        // TODO: Can mapping item vs entity modified fields?
        //public List<T> Update<T>(params T[] entities) where T : EntityBase
        //{
        //    var dbset = Database.GetDbSet<T>();
        //    foreach (var entity in entities)
        //    {
        //        var item = dbset.FirstOrDefault(x => x.Id == entity.Id);
        //        if (item is not null)
        //        {

        //            item.DateModified = DateTime.Now.ToUniversalTime();
        //            dbset.Update(item);
        //        }
        //    }

        //    return entities.ToList();
        //}

        public List<T> Update<T>(params T[] entities) where T : EntityBase
        {
            entities.ToList().ForEach(item =>
            {
                item.DateModified = DateTime.Now.ToUniversalTime();
            });

            Database.GetDbSet<T>().UpdateRange(entities);

            return entities.ToList();
        }

        public T Update<T>(T entity) where T : EntityBase
        {
            entity.DateModified = DateTime.Now.ToUniversalTime();
            Database.GetDbSet<T>().Update(entity);
            return entity;
        }

        public bool Delete<T>(T entity) where T : EntityBase
        {
            var dbset = Database.GetDbSet<T>();
            var item = dbset.FirstOrDefault(x => x.Id == entity.Id);
            if (item is not null)
            {
                dbset.Remove(item);
            }
            return true;
        }

        public bool Delete<T>(T[] entities) where T : EntityBase
        {
            var dbset = Database.GetDbSet<T>();
            foreach (var entity in entities)
            {
                dbset.Remove(entity);
            }
            return true;
        }

        public bool Delete<T>(int id) where T : EntityBase
        {
            var dbset = Database.GetDbSet<T>();
            var item = dbset.FirstOrDefault(x => x.Id == id);
            if (item is not null)
            {
                dbset.Remove(item);
            }
            return true;
        }

        public bool Delete<T>(Expression<Func<T, bool>> @where) where T : EntityBase
        {
            var dbset = Database.GetDbSet<T>();
            var items = dbset.Where(@where);
            if (items is not null && items.Count() > 0)
            {
                dbset.RemoveRange(items);
            }
            return true;
        }

        public int CountWhere<T>(Expression<Func<T, bool>> @where) where T : EntityBase
        {
            return Database.GetDbSet<T>().Count(@where);
        }

        public int CountAll<T>() where T : EntityBase
        {
            return Database.GetDbSet<T>().Count();
        }

        public IEnumerable<T> Find<T>(SpecificationBase<T> specification) where T : EntityBase
        {
            var query = Database.GetDbSet<T>().AsQueryable();

            if (specification.IsInclude)
            {
                query = AsQueryInClude(specification);
            }
            else
            {
                query = query.Where(specification.Criteria);
            }

            return query.AsEnumerable();
        }

        public IEnumerable<T> Find<T>(int pageIndex, int pageSize, SpecificationBase<T> specification, out int totalPage) where T : EntityBase
        {
            var query = Database.GetDbSet<T>().AsQueryable();

            if (specification.IsInclude)
            {
                query = AsQueryInClude(specification);
            }
            else
            {
                query = query.Where(specification.Criteria);
            }

            totalPage = query.Count();

            return query.OrderByDescending(x => x.DateCreated).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsEnumerable();
        }


        // https://github.com/dotnet-architecture/eShopOnWeb
        public IQueryable<T> AsQueryInClude<T>(ISpecification<T> specification) where T : EntityBase
        {
            // fetch a Queryable that includes all expression-based includes
            var queryableResultWithIncludes = specification.Includes
                .Aggregate(Database.GetDbSet<T>().AsQueryable(),
                    (current, include) => current.Include(include));

            // modify the IQueryable to include any string-based include statements
            var secondaryResult = specification.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            // return the result of the query using the specification's criteria expression
            return secondaryResult
                            .Where(specification.Criteria)
                            .AsQueryable();
        }

        public bool SaveChanges()
        {
            var result = Task.FromResult(Database.SaveChanges());
            return result.Result.IsCompletedSuccessfully;
        }

        public List<T> All<T>() where T : EntityBase
        {
            return Database.GetDbSet<T>().ToList();
        }
        public List<T> All<T>(out int totalRecords) where T : EntityBase
        {
            var all = Database.GetDbSet<T>().ToList();
            totalRecords = all.Count;
            return all;
        }

        public T Find<T>(string rawSql) where T : EntityBase
        {
            return Database.GetDbSet<T>().FromSqlRaw(rawSql).FirstOrDefault();
        }

        public List<T> List<T>(string rawSql) where T : EntityBase
        {
            return Database.GetDbSet<T>().FromSqlRaw(rawSql).ToList();
        }
    }
}
