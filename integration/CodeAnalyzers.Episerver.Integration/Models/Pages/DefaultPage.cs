using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace CodeAnalyzers.Episerver.Integration.Models.Pages
{
    [ContentType(DisplayName = "DefaultPage", GUID = "890ee4e2-a723-4119-90ec-198616a733d4", Description = "")]
    public class DefaultPage : PageData
    {
        [CultureSpecific]
        [Display(
            Name = "Main body",
            Description = "The main body will be shown in the main content area of the page, using the XHTML-editor you can insert for example text, images and tables.",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual XhtmlString MainBody { get; set; }
    }
}