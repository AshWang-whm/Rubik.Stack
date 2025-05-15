using AntDesign;
using AntDesign.TableModels;
using Microsoft.AspNetCore.Components;
using Rubik.Identity.Admin.Components.BasePages;
using Rubik.Identity.Share.Entity;

namespace Rubik.Identity.Admin.Components.AdminPages
{
    public partial class Organization : BaseTreeEditorPage<TbOrganization>
    {
        void OnShowOrganizePositionModal(TbOrganization org)
        {
            void buildModal(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
            {
                builder.OpenComponent<OrganizationJob>(0);
                builder.AddAttribute(1, "OrganizationID", org.ID);
                builder.CloseComponent();
            }

            var @ref = ModalService!.CreateModal(new ModalOptions
            {
                Content= buildModal,
                Title=$"查看 [{org.Name}] 岗位",
                Keyboard=true,
                Visible=true,
                Centered=true,
                MaskClosable=true,
                Maximizable=true,
                Width="75vw;"
            });

            @ref.OnClose = async () =>
            {
                System.Diagnostics.Debug.WriteLine("close");
            };
        }
    }
}
