using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Rubik.Identity.Share.Entity;
using Rubik.Share.Entity.BaseEntity;

namespace Rubik.Identity.Share.Extension
{
    public static class RecursionExtension
    {
        public static List<T> Recursion<T>(this List<T> source,IEnumerable<T> mapper) where T : class,ITreeEntity<T>
        {
            foreach (var item in source)
            {
                var childrens = mapper.Where(a => a.ParentID == item.ID).ToList();
                foreach (var child in childrens)
                {
                    item.Children.Add(child);
                }

                childrens.Recursion(mapper);
            }

            return source;
        }
    }
}
