using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Identity.Share.Extension
{
    public static class ExpressionExtension
    {
        public static Expression<Func<T1,T2,bool>> ExpressionCombine<T1,T2>(this Expression<Func<T1,bool>> exp1, Expression<Func<T2, bool>> exp2)
        {
            var paras = exp1.Parameters.Union(exp2.Parameters);
            var combineExp = Expression.Lambda<Func<T1,T2, bool>>(Expression.AndAlso(exp1.Body, exp2.Body), paras);
            return combineExp;
        }

        public static Expression<Func<T1,T2,bool>> ExpressionConvertToMultiGenerics<T1, T2>(this Expression<Func<T1, bool>> exp1)
        {
            exp1??= Expression.Lambda<Func<T1, bool>>(Expression.Constant(true, typeof(bool)), Expression.Parameter(typeof(T1)));

            var exp2= Expression.Lambda<Func<T2, bool>>(Expression.Constant(true, typeof(bool)), Expression.Parameter(typeof(T2)));
            return ExpressionCombine(exp1, exp2);
        }
    }
}
