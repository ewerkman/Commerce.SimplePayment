// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettleSimplePaymentBlock.cs" company="Sitecore Corporation">
//   Copyright (c) Sitecore Corporation 1999-2019
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Commerce.Plugin.Sample.Payment.Pipelines.Blocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Commerce.Plugin.Sample.Payment.Components;
    using Microsoft.Extensions.Logging;

    using Sitecore.Commerce.Core;
    using Sitecore.Commerce.Plugin.ManagedLists;
    using Sitecore.Commerce.Plugin.Orders;
    using Sitecore.Framework.Conditions;
    using Sitecore.Framework.Pipelines;

    [PipelineDisplayName(nameof(SettleSimplePaymentBlock)]
    public class SettleSimplePaymentBlock : PipelineBlock<Order, Order, CommercePipelineExecutionContext>
    {
        /// <summary>
        /// Gets or sets the commander.
        /// </summary>
        /// <value>
        /// The commander.
        /// </value>
        protected CommerceCommander Commander { get; set; }

        /// <inheritdoc />
        /// <summary>Initializes a new instance of the <see cref="T:Sitecore.Framework.Pipelines.PipelineBlock" /> class.</summary>
        /// <param name="commander">The commerce commander.</param>
        public SettleSimplePaymentBlock(CommerceCommander commander)
            : base(null)
        {

            this.Commander = commander;

        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="order">
        /// The pipeline argument.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="PipelineArgument"/>.
        /// </returns>
        public override async Task<Order> Run(Order arg, CommercePipelineExecutionContext context)
        {
            Condition.Requires(arg).IsNotNull($"{this.Name}: The order cannot be null.");

            var order = arg;
            var knownOrderStatuses = context.GetPolicy<KnownOrderStatusPolicy>();
            if (!order.Status.Equals(knownOrderStatuses.Released, StringComparison.OrdinalIgnoreCase)
                || !order.HasComponent<SimplePaymentComponent>())
            {
                return order;
            }

            var salesActivities = context.CommerceContext.GetEntities<SalesActivity>().Where(sa => sa.HasComponent<SimplePaymentComponent>()).ToList();
            if (!salesActivities.Any())
            {
                return order;
            }

            var knownSalesActivityStatuses = context.GetPolicy<KnownSalesActivityStatusesPolicy>();
            foreach (var salesActivity in salesActivities)
            {
                var simplePayment = salesActivity.GetComponent<SimplePaymentComponent>();
                salesActivity.PaymentStatus = knownSalesActivityStatuses.Settled;
                salesActivity.GetComponent<TransientListMembershipsComponent>().Memberships.Add(context.GetPolicy<KnownOrderListsPolicy>().SettledSalesActivities);

                await Commander.PersistEntity(context.CommerceContext, salesActivity).ConfigureAwait(false);
                await Commander.Pipeline<IRemoveListEntitiesPipeline>().Run(new ListEntitiesArgument(new List<string> { salesActivity.Id }, context.GetPolicy<KnownOrderListsPolicy>().SettleSalesActivities), context).ConfigureAwait(false);
            }

            return order;
        }
    }
}