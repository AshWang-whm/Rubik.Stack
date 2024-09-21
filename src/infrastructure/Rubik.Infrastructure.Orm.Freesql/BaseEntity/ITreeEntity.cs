using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Infrastructure.Entity.BaseEntity
{
    public interface ITreeEntity<T>
        where T : class
    {
        public int ID { get; set; }
        public int? ParentID { get; set; }

        public T? Parent { get; set; }
        public List<T> Children { get; set; }
    }
}
