using Commerce.Plugin.Sample.Payment.Policies;
using Sitecore.Commerce.Core;
using System;

namespace Commerce.Plugin.Sample.Payment.Converters
{
    /// <summary>
    ///     Configure Newtonsoft.Json to use this converter by 
    /// </summary>
    public class PolicyConverter : ODataCreationConverter<Policy>
    {
        protected override Policy Create(Type objectType, string type)
        {
            switch (type)
            {
                case "Commerce.Plugin.Sample.Payment.Policies.SalutationPolicy":
                    return new SalutationPolicy();

                default:
                    return new Policy();
            }
        }
    }
}
