using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Cassandra;
using Cassandra.Mapping;
using Cassandra.Data.Linq;
using BooksApi.Models;
using System.IO;
using System.Reflection;

namespace BooksApi.Repositories
{
    /// <summary>
    /// Deals with entities in Cassandrab.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <typeparam name="TKey">The type used for the entity's Id.</typeparam>
    public class CassandraRepository<T, TKey> : IRepository<T, TKey>
        where T : class, IEntity<TKey>
    {

        protected internal ISession session;
        protected internal CassandraDBSettings settings;
        protected internal Table<T> table;

        public CassandraRepository(CassandraDBSettings _settings)
        {
            // TODO Seralize this
            settings = _settings;

            string filename = "temp.zip";

            if (MappingConfiguration.Global.Get<T>() == null)
            {
                // Set the Mapping Configuration
                MappingConfiguration.Global.Define(
                    new Map<T>()
                        .TableName(settings.TableName)
                        .PartitionKey(nameof(IEntity.Id))
                        .KeyspaceName(settings.Keyspace));
            }

            // Generate temporary file from manifest
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(settings.ConnectionZip))
            using (var file = new FileStream(filename, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(file);
            }

            // Build db connection
            session = Cluster.Builder()
                .WithCloudSecureConnectionBundle(filename)
                .WithCredentials(settings.User, settings.Password)
                .WithDefaultKeyspace(settings.Keyspace)
                .Build()
                .Connect();

            // Remove file now that we no longer need it
            File.Delete(filename);

            // Prepare schema
            // Needs to be run with db admin credentials
            //session.Execute(new SimpleStatement($"CREATE KEYSPACE IF NOT EXISTS {settings.Keyspace} WITH replication = {{ 'class': 'SimpleStrategy', 'replication_factor': '1' }}"));
            //session.Execute(new SimpleStatement($"USE {settings.Keyspace}"));
            //session.Execute(new SimpleStatement($"CREATE TABLE IF NOT EXISTS {settings.TableName}({settings.TableSchema}, PRIMARY KEY ({nameof(IEntity.Id)}))"));

            // Get reference to table
            table = new Table<T>(session);
        }

        private bool UseFilter(Expression<Func<T, bool>> filter)
        {
            // Cassandra 3.17.1 cannot handle the expression "row => true".
            var constExp = filter.Body as ConstantExpression;
            return (constExp == null) || (!constExp.Value.Equals(true));
        }

        public virtual List<T> Get(Expression<Func<T, bool>> filter)
        {
            if (UseFilter(filter))
                return table.Where(filter).Execute().ToList();
            else
                return table.Execute().ToList();
        }

        public virtual T GetById(TKey id)
            => Get(row => row.Id.Equals(id)).FirstOrDefault();

        public virtual T Add(T entity)
        {
            if (Exists(row => row.Id.Equals(entity.Id)))
                return default;

            table.Insert(entity).Execute();
            
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

            if (Exists(row => row.Id.Equals(id)))
                table.Insert(entity).Execute(); // Upserts

            return GetById(entity.Id);
        }

        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
                Update(entity.Id, entity);
        }

        public virtual bool Delete(TKey id)
            => Delete(row => row.Id.Equals(id));

        public virtual bool Delete(T entity)
            => Delete(row => row.Id.Equals(entity.Id));

        public virtual bool Delete(Expression<Func<T, bool>> filter)
        {
            if (UseFilter(filter))
                table.Where(filter).Delete().Execute();
            else
                throw new InvalidQueryException($"Filter is not valid for Cassandra.Linq ({filter.ToString()})");
            return !Get(filter).Any();
        }

        public virtual bool DeleteAll()
        {
            session.Execute($"TRUNCATE {settings.TableName};");
            return Count() == 0;
        }
          

        public virtual long Count()
            => table.Count().Execute();

        public virtual bool Exists(Expression<Func<T, bool>> filter)
            => Get(filter).Any();

        /// <summary>
        /// Lets the server know that this thread is about to begin a series of related operations that must all occur
        /// on the same connection. The return value of this method implements IDisposable and can be placed in a using
        /// statement (in which case RequestDone will be called automatically when leaving the using statement). 
        /// </summary>
        /// <returns>A helper object that implements IDisposable and calls RequestDone() from the Dispose method.</returns>
        public virtual IDisposable RequestStart()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Lets the server know that this thread is done with a series of related operations.
        /// </summary>
        /// <remarks>
        /// Instead of calling this method it is better to put the return value of RequestStart in a using statement.
        /// </remarks>
        public virtual void RequestDone()
        {
            throw new NotImplementedException();
        }

        // Should this region be part of the interface?
        #region IQueryable<T>
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator&lt;T&gt; object that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<T> GetEnumerator()
            => table.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An IEnumerator object that can be used to iterate through the collection.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            => table.GetEnumerator();

        /// <summary>
        /// Gets the type of the element(s) that are returned when the expression tree associated with this instance of IQueryable is executed.
        /// </summary>
        public virtual Type ElementType
        {
            get { return table.ElementType; }
        }

        /// <summary>
        /// Gets the expression tree that is associated with the instance of IQueryable.
        /// </summary>
        public virtual Expression Expression
        {
            get { return table.Expression; }
        }

        /// <summary>
        /// Gets the query provider that is associated with this data source.
        /// </summary>
        public virtual IQueryProvider Provider
        {
            get { return table.Provider; }
        }
        #endregion
    }

    /// <summary>
    /// Deals with entities in Cassandra.
    /// </summary>
    /// <typeparam name="T">The type contained in the repository.</typeparam>
    /// <remarks>Entities are assumed to use strings for Id's.</remarks>
    public class CassandraRepository<T> : CassandraRepository<T, Guid>, IRepository<T>
        where T : class, IEntity<Guid>
    {
        public CassandraRepository(CassandraDBSettings settings)
            : base(settings) { }
    }
}

