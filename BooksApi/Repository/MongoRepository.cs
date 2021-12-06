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

        /// <summary>
        /// Gets the Mongo collection (to perform advanced operations).
        /// </summary>
        /// <remarks>
        /// One can argue that exposing this property (and with that, access to it's Database property for instance
        /// (which is a "parent")) is not the responsibility of this class. Use of this property is highly discouraged;
        /// for most purposes you can use the MongoRepositoryManager&lt;T&gt;
        /// </remarks>
        /// <value>The Mongo collection (to perform advanced operations).</value>
        public IMongoCollection<T> Collection
        {
            get { return this.collection; }
        }

        /// <summary>
        /// Gets the name of the collection
        /// </summary>
        public string CollectionName
        {
            get { return this.collection.CollectionNamespace.ToString(); }
        }

        /// <summary>
        /// Returns a list of T that matches a LINQ filter.
        /// </summary>
        /// <param name="filter">The filter that matching entities must match.</param>
        /// <returns>A list of Entity T.</returns>
        public virtual List<T> Get(Expression<Func<T, bool>> filter)
        {
            return this.collection.Find(filter).ToList();
        }

        /// <summary>
        /// Returns the T by its given id.
        /// </summary>
        /// <param name="id">The Id of the entity to retrieve.</param>
        /// <returns>The Entity T.</returns>
        public virtual T GetById(TKey id)
        {
            return this.collection.Find(doc => doc.Id.Equals(id)).FirstOrDefault();
        }

        /// <summary>
        /// Adds the new entity in the repository.
        /// </summary>
        /// <param name="entity">The entity T.</param>
        /// <returns>The added entity including its new ObjectId.</returns>
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

        /// <summary>
        /// Adds the new entities in the repository.
        /// </summary>
        /// <param name="entities">The entities of type T.</param>
        public virtual void Add(IEnumerable<T> entities)
        {
            this.collection.InsertMany(entities);
        }

        /// <summary>
        /// Upserts an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        public virtual T Update(T entity)
        {
            this.collection.UpdateOne(doc => doc.Id.Equals(entity.Id), entity.ToBsonDocument());

            return entity;
        }

        /// <summary>
        /// Upserts an entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>The updated entity.</returns>
        public virtual T Update(TKey id, T entity)
        {
            this.collection.ReplaceOne(doc => doc.Id.Equals(id), entity);

            return entity;
        }

        /// <summary>
        /// Upserts the entities.
        /// </summary>
        /// <param name="entities">The entities to update.</param>
        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                this.Update(entity);
            }
        }

        /// <summary>
        /// Deletes an entity from the repository by its id.
        /// </summary>
        /// <param name="id">The entity's id.</param>
        public virtual bool Delete(TKey id)
        {
            var result = this.collection.DeleteOne(doc => doc.Id.Equals(id));

            if (result.IsAcknowledged && result.DeletedCount > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Deletes the given entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        public virtual bool Delete(T entity)
        {
            return this.Delete(entity.Id);
        }

        /// <summary>
        /// Deletes the entities matching the predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        public virtual bool Delete(Expression<Func<T, bool>> predicate)
        {
            var result = this.collection.DeleteMany(predicate);

            if (result.IsAcknowledged && result.DeletedCount > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Deletes all entities in the repository.
        /// </summary>
        public virtual bool DeleteAll()
        {
            var result = this.collection.DeleteMany(doc => true);

            if (result.IsAcknowledged && result.DeletedCount > 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Counts the total entities in the repository.
        /// </summary>
        /// <returns>Count of entities in the collection.</returns>
        public virtual long Count()
        {
            return this.collection.CountDocuments(doc => true);
        }

        /// <summary>
        /// Checks if the entity exists for given predicate.
        /// </summary>
        /// <param name="predicate">The expression.</param>
        /// <returns>True when an entity matching the predicate exists, false otherwise.</returns>
        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return this.collection.AsQueryable<T>().Any(predicate);
        }

        /// <summary>
        /// Lets the server know that this thread is about to begin a series of related operations that must all occur
        /// on the same connection. The return value of this method implements IDisposable and can be placed in a using
        /// statement (in which case RequestDone will be called automatically when leaving the using statement). 
        /// </summary>
        /// <returns>A helper object that implements IDisposable and calls RequestDone() from the Dispose method.</returns>
        public virtual IDisposable RequestStart()
        {
            this.session = this.collection.Database.Client.StartSession();
            this.session.StartTransaction();
            return this.session;
        }

        /// <summary>
        /// Lets the server know that this thread is done with a series of related operations.
        /// </summary>
        /// <remarks>
        /// Instead of calling this method it is better to put the return value of RequestStart in a using statement.
        /// </remarks>
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
    public class MongoRepository<T> : MongoRepository<T, string>, IRepository<T>
        where T : IEntity<string>
    {
        public MongoRepository(MongoDBSettings settings)
            : base(settings) { }
    }
}

