namespace Commerce.Plugin.Sample.Payment.Policies
{
    using Commerce.Plugin.Sample.Payment.Converters;
    using Newtonsoft.Json;
    using Sitecore.Commerce.Core;

    [JsonConverter(typeof(PolicyConverter))]
    public class SalutationPolicy : Policy
    {
        public string Salutation { get; set; }       
    }
}
