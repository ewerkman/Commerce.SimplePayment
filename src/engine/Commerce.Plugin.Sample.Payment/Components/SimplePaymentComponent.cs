namespace Commerce.Plugin.Sample.Payment.Components
{
    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.Payments;

    /// <summary>
    /// The SimplePaymentComponent. It's really simple...
    /// </summary>
    public class SimplePaymentComponent : PaymentComponent
    {
        public SimplePaymentComponent(Money amount) : base(amount)
        {
            this.BillingParty = new Party();
        }

        public Party BillingParty { get; set; }
    }
}

