using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

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

        public virtual string ToJson()
            => JsonConvert.SerializeObject(this);
    }
}
