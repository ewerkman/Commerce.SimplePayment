using Commerce.Plugin.Sample.Payment.Policies;
using Sitecore.Commerce.Engine.Connect.Pipelines.Arguments;
using Sitecore.Commerce.Pipelines;
using Sitecore.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Web.SimplePayment.Entities;

namespace Web.SimplePayment.Pipelines
{
    public class TranslatePartyToEntity
    {
        public void Process(ServicePipelineArgs args)
        {
            var request = args.Request as TranslatePartyToEntityRequest;
            var result = args.Result as TranslatePartyToEntityResult;

            if (request != null)
            {
                // Check if there is a SalutationPolicy on the Party.Policies 
                var salutationPolicy = request.TranslateSource.Policies.FirstOrDefault(p => p is SalutationPolicy) as SalutationPolicy;
                if (salutationPolicy != null)
                {
                    var translatedEntity = result.TranslatedEntity as ExtendedCommerceParty;
                    if (translatedEntity != null)
                    {   // Translate the Salutation
                        translatedEntity.Salutation = salutationPolicy.Salutation;
                    }
                }
            }
        }
    }
}