using System.ComponentModel.DataAnnotations;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;

namespace CodeAnalyzers.Episerver.Integration.Models.Blocks
{
    [ContentType(DisplayName = "DefaultBlock", GUID = "8baf5f3b-d09b-426a-9200-8806c8c51dbd", Description = "")]
    public class DefaultBlock : BlockData
    {
        [CultureSpecific]
        [Display(
            Name = "Name",
            Description = "Name field's description",
            GroupName = SystemTabNames.Content,
            Order = 1)]
        public virtual string Name { get; set; }
    }
}