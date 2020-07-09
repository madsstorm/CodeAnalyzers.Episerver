using Microsoft.CodeAnalysis;
using System.Linq;

namespace CodeAnalyzers.Episerver.Extensions
{
    internal static class IPropertySymbolExtensions
    {
        internal static bool IsModelProperty(this IPropertySymbol propertySymbol, INamedTypeSymbol ignoreAttributeType)
        {
            if (propertySymbol is null)
            {
                return false;
            }

            if (propertySymbol.GetMethod is null || propertySymbol.SetMethod is null)
            {
                return false;
            }

            if (!propertySymbol.IsVirtual)
            {
                return false;
            }

            if (propertySymbol.DeclaredAccessibility != Accessibility.Public)
            {
                return false;
            }

            if (ignoreAttributeType != null)
            {
                var attribute = propertySymbol.GetAttributes().FirstOrDefault(attr => ignoreAttributeType.IsAssignableFrom(attr.AttributeClass));
                if (attribute != null)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
