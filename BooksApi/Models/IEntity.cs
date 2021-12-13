using System;

namespace BooksApi.Models
{
    /// <summary>
    /// Generic Entity interface.
    /// </summary>
    /// <typeparam name="TKey">The type used for the entity's Id.</typeparam>
    public interface IEntity<TKey>
    {
        /// <summary>
        /// Gets or sets the Id of the Entity.
        /// </summary>
        /// <value>Id of the Entity.</value>
        TKey Id { get; set; }

        public string ToJson();
    }

    /// <summary>
    /// "Default" Entity interface.
    /// </summary>
    /// <remarks>Entities are assumed to use Guid for Id's.</remarks>
    public interface IEntity : IEntity<Guid>
    {
    }
}