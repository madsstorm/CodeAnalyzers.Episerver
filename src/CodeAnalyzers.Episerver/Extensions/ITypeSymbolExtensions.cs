using Microsoft.CodeAnalysis;
using System.Linq;

namespace CodeAnalyzers.Episerver.Extensions
{
    internal static class ITypeSymbolExtensions
    {
        internal static bool IsAssignableFrom(this ITypeSymbol targetType, ITypeSymbol sourceType, bool exactMatch = false)
        {
            if (targetType != null)
            {
                while (sourceType != null)
                {
                    if (sourceType.Equals(targetType))
                    {
                        return true;
                    }

                    if (exactMatch)
                    {
                        return false;
                    }

                    if (targetType.TypeKind == TypeKind.Interface)
                    {
                        return sourceType.AllInterfaces.Any(i => i.Equals(targetType));
                    }

                    sourceType = sourceType.BaseType;
                }
            }

            return false;
        }
    }
}