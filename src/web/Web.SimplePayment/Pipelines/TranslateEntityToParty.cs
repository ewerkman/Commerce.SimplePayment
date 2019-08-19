using Commerce.Plugin.Sample.Payment.Policies;
using Sitecore.Commerce.Engine.Connect.Pipelines.Arguments;
using Sitecore.Commerce.Pipelines;
using Web.SimplePayment.Entities;

namespace Web.SimplePayment.Pipelines
{
    public class TranslateEntityToParty
    {
        public void Process(ServicePipelineArgs args)
        {
            var request = args.Request as TranslateEntityToPartyRequest;
            var result = args.Result as TranslateEntityToPartyResult;

            if( request.TranslateSource is ExtendedCommerceParty)
            {   // Translate the Salutation by adding a SalutationPolicy to the Party
                var source = request.TranslateSource as ExtendedCommerceParty;
                var salutationPolicy = new SalutationPolicy() { Salutation = source.Salutation };

                request.TranslateDestination.Policies.Add(salutationPolicy);
            }
        }
    }
}