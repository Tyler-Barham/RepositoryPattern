using System;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace BooksApi.Models
{
    public class Book : Entity, IEquatable<Book>
    {
        [JsonProperty("Book")]
        public string BookName { get; set; }

        [DataMember]
        public decimal Price { get; set; }

        [DataMember]
        public string Category { get; set; }

        [DataMember]
        public string Author { get; set; }

        public override bool Equals(object obj)
            => Equals(obj as Book);

        public bool Equals(Book other)
        {
            // Optimization for a common success case.
            if (ReferenceEquals(this, other))
                return true;

            if (other is null)
                return false;

            return (
                this.Id.Equals(other.Id) &&
                this.Author.Equals(other.Author) &&
                this.BookName.Equals(other.BookName) &&
                this.Category.Equals(other.Category) &&
                this.Price.Equals(other.Price)
            );
        }

        public static bool operator ==(Book lhs, Book rhs)
        {
            if (ReferenceEquals(lhs, rhs))
                return true; // Same reference or both null.
            else if (lhs is null)
                return false; // Only the left side is null.

            // Equals handles the case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Book lhs, Book rhs)
            => !(lhs == rhs);

        public override int GetHashCode()
            => HashCode.Combine(this.Id, this.Author, this.BookName, this.Category, this.Price);
    }
}
