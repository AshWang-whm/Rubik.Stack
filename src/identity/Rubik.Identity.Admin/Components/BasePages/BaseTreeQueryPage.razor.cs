using AntDesign.TableModels;
using Rubik.Infrastructure.Entity.BaseEntity;
using System.Linq.Expressions;

namespace Rubik.Identity.Admin.Components.BasePages
{
    public partial class BaseTreeQueryPage<T>:BasePage<T>
        where T : BaseTreeEntity<T>, IFullEntity, new()
    {
        protected virtual Func<T, IEnumerable<T>> TreeChildren { get; set; } = item => item.Children;

        protected virtual Expression<Func<T, bool>>? ExtraWhereExpression { get; set; }

        public override async Task Query(QueryModel<T> query)
        {
            var exp = query.GetFilterExpressionOrNull();

            // 顶级的数据作为total数据分页统计
            var source = await FreeSql.Select<T>()
                .WhereIf(exp != null, exp)
                .WhereIf(exp == null, a => a.ParentID == null)
                .WhereIf(ExtraWhereExpression != null, ExtraWhereExpression)
                .Where(a => a.IsDelete == false)
                .Count(out var total)
                .ToListAsync();

            var ids = source.Select(a => a.ID).ToList();
            DataSource = ids.Count == 0 ? source : await FreeSql.Select<T>()
                    .Where(a => a.IsDelete == false)
                    .Where(a => ids.Contains(a.ID))
                    .AsTreeCte()
                    .OrderBy(a => a.Sort)
                    .ToTreeListAsync();

            Total = (int)total;
        }
    }
}
