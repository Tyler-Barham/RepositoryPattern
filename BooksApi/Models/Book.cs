using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace BooksApi.Models
{
    public class Book : Entity
    {
        public override Guid Id { get; set; }

        [JsonProperty("Name")]
        public string BookName { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string Author { get; set; }
    }
}
