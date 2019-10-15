using Microsoft.CodeAnalysis;

namespace CodeAnalyzers.Episerver.Extensions
{
    internal static class INamedTypeSymbolExtensions
    {
        public static bool InheritsOrIs(this INamedTypeSymbol symbol, ITypeSymbol type)
        {
            if(symbol.Equals(type))
            {
                return true;
            }

            var baseType = symbol.BaseType;
            while (baseType != null)
            {
                if (type.Equals(baseType))
                    return true;

                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}