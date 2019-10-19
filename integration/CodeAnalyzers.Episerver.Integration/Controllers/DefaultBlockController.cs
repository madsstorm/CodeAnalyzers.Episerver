using System.Web.Mvc;
using CodeAnalyzers.Episerver.Integration.Models.Blocks;
using EPiServer.Web.Mvc;

namespace CodeAnalyzers.Episerver.Integration.Controllers
{
    public class DefaultBlockController : BlockController<DefaultBlock>
    {
        public override ActionResult Index(DefaultBlock currentBlock)
        {
            return PartialView(currentBlock);
        }
    }
}
