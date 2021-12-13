using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using BooksApi.Models;

namespace BooksApi.Repositories
{
    /// <summary>
    /// Deals with entities in MongoDb.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <typeparam name="TKey">The type used for the entity's Id.</typeparam>
    public class MongoRepository<T, TKey> : IRepository<T, TKey>
        where T : IEntity<TKey>
    {
        protected internal IMongoCollection<T> collection;
        protected internal IClientSession session;

        public MongoRepository(MongoDBSettings settings)
        {
            // Connect to db
            var client = new MongoClient(settings.GetMongoClientSettings());
            var database = client.GetDatabase(settings.DatabaseName);

            // Create collection if not exists
            var collectionNames = database.ListCollectionNames().ToList();
            if (!collectionNames.Contains(settings.CollectionName))
                database.CreateCollection(settings.CollectionName);

            // Get the collection
            collection = database.GetCollection<T>(settings.CollectionName);
        }

        public virtual List<T> Get(Expression<Func<T, bool>> filter)
            => collection.Find(filter).ToList();

        public virtual T GetById(TKey id)
            => collection.Find(doc => doc.Id.Equals(id)).FirstOrDefault();

        public virtual T Add(T entity)
        {
            try
            {
                collection.InsertOne(entity);
            }
            catch (MongoWriteException)
            {
                return default;
            }
            return entity;
        }

        public virtual void Add(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
                Add(entity);
        }

        public virtual T Update(T entity)
            => Update(entity.Id, entity);

        public virtual T Update(TKey id, T entity)
        {
            if (!id.Equals(entity.Id))
                return default;

            var result = collection.ReplaceOne(doc => doc.Id.Equals(id), entity);

            if (result.IsAcknowledged && result.ModifiedCount > 0)
                return entity;
            else
                return default;
        }

        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
                Update(entity.Id, entity);
        }

        public virtual bool Delete(TKey id)
            => Delete(doc => doc.Id.Equals(id));

        public virtual bool Delete(T entity)
            => Delete(doc => doc.Id.Equals(entity.Id));

        public virtual bool Delete(Expression<Func<T, bool>> predicate)
        {
            var result = collection.DeleteMany(predicate);

            if (result.IsAcknowledged && result.DeletedCount > 0)
                return true;
            else
                return false;
        }

        public virtual bool DeleteAll()
            => Delete(doc => true) && Count() == 0;

        public virtual long Count()
            => collection.CountDocuments(doc => true);

        public virtual bool Exists(Expression<Func<T, bool>> predicate)
            => collection.AsQueryable().Any(predicate);

        public virtual IDisposable RequestStart()
        {
            session = collection.Database.Client.StartSession();
            session.StartTransaction();
            return session;
        }

        public virtual void RequestDone()
            => session.CommitTransaction();

        #region IQueryable<T>
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator&lt;T&gt; object that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<T> GetEnumerator()
            => collection.AsQueryable().GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => collection.AsQueryable().GetEnumerator();

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of IQueryable is executed.
        /// </summary>
        public virtual Type ElementType
        {
            get { return collection.AsQueryable().ElementType; }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of IQueryable.
        /// </summary>
        public virtual Expression Expression
        {
            get { return collection.AsQueryable().Expression; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        public virtual IQueryProvider Provider
        {
            get { return collection.AsQueryable().Provider; }
        }
        #endregion
    }

    /// <summary>
    /// Deals with entities in MongoDb.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <remarks>Entities are assumed to use strings for Id's.</remarks>
    public class MongoRepository<T> : MongoRepository<T, Guid>, IRepository<T>
        where T : IEntity<Guid>
    {
        public MongoRepository(MongoDBSettings settings)
            : base(settings) { }
    }
}

