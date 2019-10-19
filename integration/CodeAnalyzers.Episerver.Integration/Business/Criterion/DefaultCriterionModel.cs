using System;
using EPiServer.Personalization.VisitorGroups;

namespace CodeAnalyzers.Episerver.Integration.Business.Criterion
{
    public class DefaultCriterionModel : CriterionModelBase
    {
        #region Editable Properties

        // TODO:
        // Add your model's editable properties here.
        // Decorate them with the DojoWidget attribute to
        // steer the control used to present the property
        // in the Visitor Group Admin UI

        #endregion

        public override ICriterionModel Copy()
        {
            // TODO:
            // If your model uses only built-in CLR types
            // then you can safely return base.ShallowCopy()
            // otherwise you need to add logic to do a deep copy
            throw new NotImplementedException();
        }
    }
}
