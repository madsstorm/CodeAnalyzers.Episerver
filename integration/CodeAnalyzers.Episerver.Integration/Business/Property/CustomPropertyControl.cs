using EPiServer.Web.PropertyControls;

namespace CodeAnalyzers.Episerver.Integration.Business.Property
{
    /// <summary>
    /// PropertyControl implementation used for rendering CustomProperty data.
    /// </summary>
    public class CustomPropertyControl : PropertyStringControl
    {
        /*
        Override CreateXXXControls to control the appearance of the property data in different rendering conditions.

        public override void CreateDefaultControls()        - Used when rendering the view mode.
        public override void CreateEditControls()           - Used when rendering the property in edit mode.
        public override void CreateOnPageEditControls()     - used when rendering the property for "On Page Edit".

        */

        /// <summary>
        /// Gets the CustomProperty instance for this IPropertyControl.
        /// </summary>
        /// <value>The property that is to be displayed or edited.</value>
        public CustomProperty CustomProperty
        {
            get
            {
                return PropertyData as CustomProperty;
            }
        }
    }
}
