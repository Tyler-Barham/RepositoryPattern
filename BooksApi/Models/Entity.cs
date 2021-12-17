using System;
using System.Runtime.Serialization;

namespace BooksApi.Models
{
    /// <summary>
    /// Abstract Entity for all the BusinessEntities.
    /// </summary>
    [DataContract]
    [Serializable]
    public abstract class Entity : IEntity<Guid>
    {
        [DataMember]
        public Guid Id { get; set; }
    }
}
