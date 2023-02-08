using System;
using System.Collections.Generic;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace AXM.BILLLine.VS
{
    public class BillLineTrigger : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {

            try
            {
                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                    IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                    IOrganizationService service = (IOrganizationService)serviceFactory.CreateOrganizationService(context.UserId);
                    if (context.MessageName.ToLower() == "create")
                    {
                        Entity targetEntity = context.InputParameters["Target"] as Entity;
                        //var newg = targetEntity.GetAttributeValue<EntityReference>("axm_billdn").Id;
                        //string updateEntityId = targetEntity["axm_billlineid"].ToString();
                        Entity updateBillEntity = service.Retrieve("axm_bill", targetEntity.GetAttributeValue<EntityReference>("axm_billnameid").Id, new ColumnSet("axm_price"));
                        Money price = (Money)targetEntity["axm_price"];
                        Money billPrice;
                        decimal oldPrice;
                        if (updateBillEntity.GetAttributeValue<Money>("axm_price") == null)
                        {
                            oldPrice = 0;
                        }
                        else
                        {
                            billPrice = (Money)updateBillEntity["axm_price"];
                            oldPrice = billPrice.Value;
                        }

                        decimal dPrice = price.Value;

                        Money finalPrice = new Money(dPrice + oldPrice);
                        //updateBillEntity.Id = updateEntityId;
                        updateBillEntity["axm_price"] = finalPrice;
                        service.Update(updateBillEntity);

                    }


                    if (context.MessageName.ToLower() == "update")
                    {
                        Entity preImage = context.PreEntityImages["preImage"] as Entity;// za prethodna cena
                        Entity target = context.InputParameters["Target"] as Entity;//za promeneta cena
                        if (preImage == null || target == null)
                        {
                            return;
                        }
                        if (preImage.Contains("axm_price") && target.Contains("axm_price"))
                        {
                            Entity updateBillEntity = service.Retrieve("axm_billdn", preImage.GetAttributeValue<EntityReference>("axm_billnameid").Id, new ColumnSet("axm_price"));
                            Money prePrice = preImage.GetAttributeValue<Money>("axm_price");
                            Money targetPrice = target.GetAttributeValue<Money>("axm_price");
                            Money billPrice = updateBillEntity.GetAttributeValue<Money>("axm_price");
                            decimal pp, tp, bp;
                            pp = prePrice.Value;
                            tp = targetPrice.Value;
                            bp = billPrice.Value;
                            if (pp != tp)
                            {
                                bp = bp - pp + tp;
                            }
                            Money updatedPrice = new Money(bp);
                            updateBillEntity["axm_price"] = updatedPrice;
                            service.Update(updateBillEntity);

                        }
                    }

                    else
                    {
                        throw new Exception("Something is wrong");
                    }


                }



            }
            catch (Exception ex)
            {
                new InvalidOperationException(ex.Message);
            }

        }
    }
}
        
/* if(context.MessageName == "Update")
             {
                     Entity preImage = context.PreEntityImages["PreImage"];
                     Entity entityUpd = (Entity)context.InputParameters["Target"];
                     Money preI = (Money)preImage["axm_price"];
                     Money entUpd = (Money)entityUpd["axm_price"];
                     var id1 = entityUpd.GetAttributeValue<EntityReference>("axm_billnameid").Id;
                     Entity retrieved = service.Retrieve("axm_bill", id1, new ColumnSet("axm_price"));
                     Money old = (Money)retrieved["axm_price"];

                 decimal newPrice = old.Value;
                 if (preI.Value != entUpd.Value)
                     {
                     newPrice = newPrice - preI.Value + entUpd.Value;
                     retrieved["axm_price"] = newPrice;
                     service.Update(retrieved);
                 }*/