using Microsoft.CodeAnalysis;

namespace CodeAnalyzers.Episerver.Extensions
{
    internal static class ITypeSymbolExtensions
    {
        public static bool InheritsOrIs(this ITypeSymbol symbol, ITypeSymbol type)
        {
            if(symbol.Equals(type))
            {
                return true;
            }

            var baseType = symbol.BaseType;
            while (baseType != null)
            {
                if (type.Equals(baseType))
                {
                    return true;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}