using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace BooksApi.Models
{
    public class Book : Entity, IEquatable<Book>
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

        public bool Equals(Book other)
        {
            return (
                this.Id.Equals(other.Id) &&
                this.Author.Equals(other.Author) &&
                this.BookName.Equals(other.BookName) &&
                this.Category.Equals(other.Category) &&
                this.Price.Equals(other.Price)
            );
        }
    }
}
