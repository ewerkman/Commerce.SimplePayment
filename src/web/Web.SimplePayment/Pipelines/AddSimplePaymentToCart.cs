using Commerce.Plugin.Sample.Payment.Components;
using Sitecore.Commerce.Core;
using Sitecore.Commerce.Engine.Connect;
using Sitecore.Commerce.Engine.Connect.Entities;
using Sitecore.Commerce.Engine.Connect.Pipelines;
using Sitecore.Commerce.Engine.Connect.Pipelines.Carts;
using Sitecore.Commerce.Pipelines;
using Sitecore.Commerce.ServiceProxy;
using Sitecore.Commerce.Services.Carts;
using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Web.SimplePayment.Entities;

namespace Web.SimplePayment.Pipelines
{
    public class AddSimplePaymentToCart : CartProcessor
    {
        public override void Process(ServicePipelineArgs args)
        {
            AddPaymentInfoRequest request;
            AddPaymentInfoResult result;

            request = args.Request as AddPaymentInfoRequest;
            result = args.Result as AddPaymentInfoResult;

            if( request.Payments.Any(p => p is SimplePaymentInfo))
            {
                var cart = request.Cart;
                var container = EngineConnectUtility.GetShopsContainer(shopName: cart.ShopName, customerId: cart.CustomerId);

                foreach (var payment in request.Payments.Where(p => p is SimplePaymentInfo).Select(p => TranslatePayment((SimplePaymentInfo) p, cart.Parties, result)).ToList())
                {
                    var command = Proxy.DoCommand(container.AddSimplePayment(cart.ExternalId, payment));
                    result.HandleCommandMessages(command);                   
                }

                // Remove the SimplePaymentInfo payments from the list of payments, so they are not evaluated by other processors
                request.Payments = request.Payments.Where(p => !(p is SimplePaymentInfo)).ToList();
            }

        }

        private SimplePaymentComponent TranslatePayment(SimplePaymentInfo paymentInfo, 
            IEnumerable<Sitecore.Commerce.Entities.Party> parties,
            AddPaymentInfoResult result)
        {
            var address = parties.FirstOrDefault(a => a.ExternalId.Equals(paymentInfo.PartyID, StringComparison.OrdinalIgnoreCase)) as CommerceParty;
            Assert.IsNotNull(address, "Billing address can not be null");

            return new SimplePaymentComponent()
            {
                Id = Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture),
                PaymentMethod = new EntityReference { EntityTarget = paymentInfo.PaymentMethodID },
                BillingParty = this.TranslateEntityToParty(address, new Party(), result),
                Amount = Money.CreateMoney(paymentInfo.Amount)
            };
        }
    }
}