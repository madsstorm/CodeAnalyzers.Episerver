using EPiServer.PlugIn;

namespace CodeAnalyzers.Episerver.Integration.Plugins
{
    [GuiPlugIn(DisplayName = "AdminPlugin", Description = "AdminPlugin", Area = PlugInArea.AdminMenu, Url = "~/Plugins/AdminPlugin.aspx")]
    public partial class AdminPlugin : System.Web.UI.Page
    {

        // TODO: Add your Plugin Control Code here.

    }
}