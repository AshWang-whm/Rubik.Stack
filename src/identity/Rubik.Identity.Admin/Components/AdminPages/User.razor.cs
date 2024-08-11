using AntDesign.TableModels;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class User: BaseEditorPage<TbUser>
    {
        public override Task Query(QueryModel<TbUser> query)
        {
            throw new NotImplementedException();
        }
    }
}
