using System;
using System.Web;
using EPiServer.Personalization.VisitorGroups;

namespace CodeAnalyzers.Episerver.Integration.Business.Criterion
{
    [VisitorGroupCriterion(DisplayName = "DefaultCriterion", ScriptUrl = "Business/Criterion/DefaultCriterion.js")]
    public class DefaultCriterion : CriterionBase<DefaultCriterionModel>
    {
        public override bool IsMatch(System.Security.Principal.IPrincipal principal, HttpContextBase httpContext)
        {
            // TODO:
            // Satisfy if the current request is a match using the
            // criterion's logic amd model.
            // The model instance can be accessed via the Model property
            throw new NotImplementedException();
        }
    }
}
