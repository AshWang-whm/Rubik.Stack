using AntDesign.TableModels;
using System.Linq.Expressions;

namespace Rubik.Identity.Admin.Extensions
{
    public static class AntDesignExtension
    {
        public static Expression<Func<T,bool>>? GetFilterExpressionOrNull<T>(this QueryModel<T> queryModel)
        {
            var exp = queryModel.GetFilterExpression();
            if (exp.Body.NodeType == ExpressionType.Constant)
                return null;
            return exp;
        }
    }
}
