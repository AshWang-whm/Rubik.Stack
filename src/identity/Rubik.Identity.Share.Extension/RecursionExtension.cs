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
        /// <summary>
        /// 递归写入子集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="mapper"></param>
        /// <returns></returns>
        public static List<T> Recursion<T>(this List<T> source,IEnumerable<T> mapper) where T : class,ITreeEntity<T>
        {
            foreach (var item in source)
            {
                var childrens = mapper.Where(a => a.ParentID == item.ID).ToList();
                foreach (var child in childrens)
                {
                    item.Children.Add(child);
                    child.Parent = item;
                }

                childrens.Recursion(mapper);
            }

            return source;
        }
    }
}
