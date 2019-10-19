using System.Web.Mvc;
using CodeAnalyzers.Episerver.Integration.Models.Pages;
using EPiServer.Web.Mvc;

namespace CodeAnalyzers.Episerver.Integration.Controllers
{
    public class DefaultPageController : PageController<DefaultPage>
    {
        public ActionResult Index(DefaultPage currentPage)
        {
            /* Implementation of action. You can create your own view model class that you pass to the view or
             * you can pass the page type for simpler templates */

            return View(currentPage);
        }
    }
}