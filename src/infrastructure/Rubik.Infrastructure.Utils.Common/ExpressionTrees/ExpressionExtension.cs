using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Infrastructure.Utils.Common.ExpressionTrees
{
    public static class ExpressionExtension
    {
        public static Expression<Func<T1,T2,bool>> ExpressionCombine<T1,T2>(this Expression<Func<T1,bool>> exp1, Expression<Func<T2, bool>> exp2)
        {
            var paras = exp1.Parameters.Union(exp2.Parameters);
            var combineExp = Expression.Lambda<Func<T1,T2, bool>>(Expression.AndAlso(exp1.Body, exp2.Body), paras);
            return combineExp;
        }

        public static Expression<Func<T1, T2,T3, bool>> ExpressionCombine<T1, T2,T3>(this Expression<Func<T1, bool>> exp1, Expression<Func<T2, bool>> exp2
            , Expression<Func<T3, bool>> exp3)
        {
            var paras = exp1.Parameters.Union(exp2.Parameters).Union(exp3.Parameters);
            var combineExp = Expression.Lambda<Func<T1, T2,T3, bool>>(Expression.AndAlso(exp1.Body,Expression.AndAlso(exp2.Body, exp3.Body)), paras);
            return combineExp;
        }

        public static Expression<Func<T1,T2,bool>> ExpressionConvertToMultiGenerics<T1, T2>(this Expression<Func<T1, bool>> exp1)
        {
            exp1??= Expression.Lambda<Func<T1, bool>>(Expression.Constant(true, typeof(bool)), Expression.Parameter(typeof(T1)));

            var exp2= Expression.Lambda<Func<T2, bool>>(Expression.Constant(true, typeof(bool)), Expression.Parameter(typeof(T2)));
            return exp1.ExpressionCombine(exp2);
        }

        public static Expression<Func<T1, T2,T3, bool>> ExpressionConvertToMultiGenerics<T1, T2,T3>(this Expression<Func<T1, bool>> exp1)
        {
            exp1 ??= Expression.Lambda<Func<T1, bool>>(Expression.Constant(true, typeof(bool)), Expression.Parameter(typeof(T1)));

            var exp2 = Expression.Lambda<Func<T2, bool>>(Expression.Constant(true, typeof(bool)), Expression.Parameter(typeof(T2)));

            var exp3 = Expression.Lambda<Func<T3, bool>>(Expression.Constant(true, typeof(bool)), Expression.Parameter(typeof(T3)));

            return exp1.ExpressionCombine(exp2, exp3);
        }
    }
}
