using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Rubik.Share.Utils.Objects
{
    public class GenericsCloneExpression<TIn, TOut>
    {
        static Func<TIn, TOut> _cloneFunc;

        /// <summary>
        /// 为每个泛型副本 编译一份 泛型复制类
        /// </summary>
        static GenericsCloneExpression()
        {
            // 创建一个函数的参数  =>  TOut Clone(TIn TIn){ }
            var paraExpression = Expression.Parameter(typeof(TIn), "TIn");
            var inProps = typeof(TIn).GetProperties();
            List<MemberBinding> bindings = new(inProps.Length);
            foreach (var outProp in typeof(TOut).GetProperties())
            {
                if (!inProps.Any(a => a.Name == outProp.Name && a.PropertyType == outProp.PropertyType))
                    continue;
                if (!outProp.CanWrite)
                    continue;
                // 编译后的代码模拟
                // 获取原对象的Property =>  var val =  typeof(TIn).GetProperty(outProp.Name).GetValue(Tin);
                MemberExpression property = Expression.Property(paraExpression, typeof(TIn).GetProperty(outProp.Name)!);
                // 绑定到TOut的Property => TOut.Property = val as 字段的类型;
                // TOut.Property = TIn.Property;
                MemberBinding memberBinding = Expression.Bind(outProp, property);
                bindings.Add(memberBinding);
            }

            var inFields = typeof(TIn).GetFields().Select(a => a.Name);
            foreach (var outField in typeof(TOut).GetFields())
            {
                if (!inFields.Any(a => a == outField.Name))
                    continue;
                // 获取TIn的Field
                MemberExpression field = Expression.Field(paraExpression, typeof(TIn).GetField(outField.Name)!);
                // 绑定到TOut的Field
                // TIn.Field = TOut.Field
                MemberBinding filedBinding = Expression.Bind(outField, field);
                bindings.Add(filedBinding);
            }

            MemberInitExpression memberInitExpression = Expression.MemberInit(Expression.New(typeof(TOut)), bindings);
            Expression<Func<TIn, TOut>> lambda = Expression.Lambda<Func<TIn, TOut>>(memberInitExpression, paraExpression);  // Lambda 转成 匿名函数
            _cloneFunc = lambda.Compile();
        }

        public static TOut Clone(TIn obj) => _cloneFunc(obj);
    }
}
