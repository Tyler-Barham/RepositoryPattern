using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BooksApi.Models;

namespace BooksApi.Repositories
{
    /// <summary>
    /// Deals with entities in memory.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <typeparam name="TKey">The type used for the entity's Id.</typeparam>
    public class RAMRepository<T, TKey> : IRepository<T, TKey>
        where T : class, IEntity<TKey>
    {
        /// <summary>
        /// A collection to store all entities.
        /// </summary>
        protected internal EntityList<T, TKey> collection;

        public RAMRepository()
        {
            // Setup list
            collection = new EntityList<T, TKey>();
        }

        public virtual List<T> Get(Expression<Func<T, bool>> filter)
        {
            return this.collection.AsQueryable<T>().Where(filter).ToList();
        }

        public virtual T GetById(TKey id)
        {
            return this.collection.FirstOrDefault(doc => doc.Id.Equals(id));
        }

        public virtual T Add(T entity)
        {
            if (this.GetById(entity.Id) == null)
            {
                this.collection.Add(entity);
                return entity;
            }
            else
            {
                return default;
            }
        }

        public virtual void Add(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
                this.Add(entity);
        }

        public virtual T Update(T entity)
        {
            return this.Update(entity.Id, entity);
        }

        public virtual T Update(TKey id, T entity)
        {
            var isUpdated = this.collection.UpdateEntity(id, entity);

            if (isUpdated)
                return entity;
            else
                return default;
        }

        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
                this.Update(entity);
        }

        public virtual bool Delete(TKey id)
        {
            return this.collection.RemoveEntity(id);
        }

        public virtual bool Delete(T entity)
        {
            return this.Delete(entity.Id);
        }

        public virtual bool Delete(Expression<Func<T, bool>> predicate)
        {
            var entsToDel = this.Get(predicate);
            bool anyDeleted = false;
            foreach (var entity in entsToDel)
            {
                anyDeleted &= this.Delete(entity);
            }

            return anyDeleted;
        }

        public virtual bool DeleteAll()
        {
            this.collection.Clear();

            return (this.collection.Count == 0);
        }

        public virtual long Count()
        {
            return this.collection.Count;
        }

        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return this.collection.AsQueryable<T>().Any(predicate);
        }

        public virtual IDisposable RequestStart()
        {
            throw new NotImplementedException();
        }

        public virtual void RequestDone()
        {
            throw new NotImplementedException();
        }

        #region IQueryable<T>
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator&lt;T&gt; object that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.collection.AsQueryable<T>().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.collection.AsQueryable<T>().GetEnumerator();
        }

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of IQueryable is executed.
        /// </summary>
        public virtual Type ElementType
        {
            get { return this.collection.AsQueryable<T>().ElementType; }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of IQueryable.
        /// </summary>
        public virtual Expression Expression
        {
            get { return this.collection.AsQueryable<T>().Expression; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        public virtual IQueryProvider Provider
        {
            get { return this.collection.AsQueryable<T>().Provider; }
        }
        #endregion
    }

    /// <summary>
    /// Deals with entities in memory.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <remarks>Entities are assumed to use strings for Id's.</remarks>
    public class RAMRepository<T> : RAMRepository<T, Guid>, IRepository<T>
        where T : class, IEntity<Guid>
    {
        public RAMRepository()
            : base() { }
    }
}

