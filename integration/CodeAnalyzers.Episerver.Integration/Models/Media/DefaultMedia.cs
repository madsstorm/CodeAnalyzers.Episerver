using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;

namespace CodeAnalyzers.Episerver.Integration.Models.Media
{
    [ContentType(DisplayName = "DefaultMedia", GUID = "65736cfc-b3ec-49d8-b159-460218e97367", Description = "")]
    [MediaDescriptor(ExtensionString = "pdf,doc,docx")]
    public class DefaultMedia : MediaData
    {
        [CultureSpecific]
        [Editable(true)]
        [Display(
            Name = "Description",
            Description = "Description field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Description { get; set; }
    }
}