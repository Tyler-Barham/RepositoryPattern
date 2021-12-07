using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BooksApi.Models
{
    public class EntityList<T, TKey> : List<T>
        where T : IEntity<TKey>
    {
        public bool IsValidIndex(int idx)
        {
            return (idx >= 0 && idx < this.Count);
        }

        public bool UpdateEntity(TKey id, T entity)
        {
            if (!id.Equals(entity.Id))
                return false;

            var idx = this.FindIndex(e => e.Id.Equals(id));

            if (this.IsValidIndex(idx))
            {
                this[idx] = entity;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool RemoveEntity(TKey id)
        {
            var idx = this.FindIndex(e => e.Id.Equals(id));

            if (this.IsValidIndex(idx))
            {
                this.RemoveAt(idx);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
