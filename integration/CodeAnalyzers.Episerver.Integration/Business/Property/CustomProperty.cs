using System;
using EPiServer.Core;
using EPiServer.PlugIn;

namespace CodeAnalyzers.Episerver.Integration.Business.Property
{
    /// <summary>
    /// Custom PropertyData implementation
    /// </summary>
	[Serializable]
    [PropertyDefinitionTypePlugIn]
    public class CustomProperty : PropertyLongString
    {
    }
}
