using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using BooksApi.Models;

namespace BooksApi.Repository
{
    /// <summary>
    /// Deals with entities in MongoDb.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <typeparam name="TKey">The type used for the entity's Id.</typeparam>
    public class MongoRepository<T, TKey> : IRepository<T, TKey>
        where T : IEntity<TKey>
    {
        /// <summary>
        /// MongoCollection field.
        /// </summary>
        protected internal IMongoCollection<T> collection;
        protected internal IClientSession session;

        public MongoRepository(MongoDBSettings settings)
        {
            // Connect to db
            var client = new MongoClient(settings.getMongoClientSettings());
            var database = client.GetDatabase(settings.DatabaseName);

            // Create collection if not exists
            var collectionNames = database.ListCollectionNames().ToList();
            if (!collectionNames.Contains(settings.CollectionName))
                database.CreateCollection(settings.CollectionName);

            // Get the collection
            this.collection = database.GetCollection<T>(settings.CollectionName);
        }

        public virtual List<T> Get(Expression<Func<T, bool>> filter)
        {
            return this.collection.Find(filter).ToList();
        }

        public virtual T GetById(TKey id)
        {
            return this.collection.Find(doc => doc.Id.Equals(id)).FirstOrDefault();
        }

        public virtual T Add(T entity)
        {
            try
            {
                this.collection.InsertOne(entity);
            }
            catch (MongoWriteException)
            {
                return default(T);
            }
            return entity;
        }

        public virtual void Add(IEnumerable<T> entities)
        {
            this.collection.InsertMany(entities);
        }

        public virtual T Update(T entity)
        {
            this.collection.UpdateOne(doc => doc.Id.Equals(entity.Id), entity.ToBsonDocument());

            return entity;
        }

        public virtual T Update(TKey id, T entity)
        {
            this.collection.ReplaceOne(doc => doc.Id.Equals(id), entity);

            return entity;
        }

        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                this.Update(entity);
            }
        }

        public virtual bool Delete(TKey id)
        {
            var result = this.collection.DeleteOne(doc => doc.Id.Equals(id));

            if (result.IsAcknowledged && result.DeletedCount > 0)
                return true;
            else
                return false;
        }

        public virtual bool Delete(T entity)
        {
            return this.Delete(entity.Id);
        }

        public virtual bool Delete(Expression<Func<T, bool>> predicate)
        {
            var result = this.collection.DeleteMany(predicate);

            if (result.IsAcknowledged && result.DeletedCount > 0)
                return true;
            else
                return false;
        }

        public virtual bool DeleteAll()
        {
            var result = this.collection.DeleteMany(doc => true);

            if (result.IsAcknowledged && result.DeletedCount > 0)
                return true;
            else
                return false;
        }

        public virtual long Count()
        {
            return this.collection.CountDocuments(doc => true);
        }

        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return this.collection.AsQueryable<T>().Any(predicate);
        }

        public virtual IDisposable RequestStart()
        {
            this.session = this.collection.Database.Client.StartSession();
            this.session.StartTransaction();
            return this.session;
        }

        public virtual void RequestDone()
        {
            this.session.CommitTransaction();
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

