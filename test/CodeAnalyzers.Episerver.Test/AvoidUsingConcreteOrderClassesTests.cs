using Verify = CodeAnalyzers.Episerver.Test.CSharpVerifier<CodeAnalyzers.Episerver.DiagnosticAnalyzers.CSharp.BannedApiAnalyzer>;
using System.Threading.Tasks;
using Xunit;

namespace CodeAnalyzers.Episerver.Test
{
    public class AvoidUsingConcreteOrderClassesTests
    {
        [Fact]
        public async Task IgnoreInterfaceProperties()
        {
            var test = @"
                using EPiServer.Commerce.Order;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test(ICart cart,
                                         IOrderGroup orderGroup,
                                         ILineItem lineItem,
                                         IOrderAddress orderAddress,
                                         IOrderForm orderForm,
                                         IOrderNote orderNote,
                                         IPaymentPlan paymentPlan,
                                         IPurchaseOrder purchaseOrder,
                                         IShipment shipment,
                                         ITaxValue taxValue,
                                         IPayment payment)
                        {
                            var currency = cart.Currency;
                            var created = orderGroup.Created;
                            var quantity = lineItem.Quantity;
                            var firstName = orderAddress.FirstName;
                            var handlingTotal = orderForm.HandlingTotal;
                            var title = orderNote.Title;
                            var startDate = paymentPlan.StartDate;
                            var orderNumber = purchaseOrder.OrderNumber;
                            var pickListId = shipment.PickListId;
                            var taxType = taxValue.TaxType;
                            var paymentId = payment.PaymentId;
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test);
        }

        [Fact]
        public async Task DetectOrderClassCreation()
        {
            var test = @"
                using System;
                using Mediachase.Commerce.Orders;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test()
                        {
                            Cart cart = new Cart("""", Guid.NewGuid());
                            LineItem lineItem = new LineItem();
                            OrderAddress orderAddress = new OrderAddress();
                            OrderForm orderForm = new OrderForm();
                            OrderNote orderNote = new OrderNote();
                            PaymentPlan paymentPlan = new PaymentPlan("""", Guid.NewGuid());
                            PurchaseOrder purchaseOrder = new PurchaseOrder(Guid.NewGuid());
                            Shipment shipment = new Shipment();
                            TaxValue taxValue = new TaxValue();
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(11, 41).WithArguments("Mediachase.Commerce.Orders.Cart"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(12, 49).WithArguments("Mediachase.Commerce.Orders.LineItem"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(13, 57).WithArguments("Mediachase.Commerce.Orders.OrderAddress"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(14, 51).WithArguments("Mediachase.Commerce.Orders.OrderForm"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(15, 51).WithArguments("Mediachase.Commerce.Orders.OrderNote"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(16, 55).WithArguments("Mediachase.Commerce.Orders.PaymentPlan"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(17, 59).WithArguments("Mediachase.Commerce.Orders.PurchaseOrder"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(18, 49).WithArguments("Mediachase.Commerce.Orders.Shipment"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(19, 49).WithArguments("Mediachase.Commerce.Orders.TaxValue"));
        }

        [Fact]
        public async Task DetectOrderClassProperties()
        {
            var test = @"
                using System;
                using Mediachase.Commerce.Orders;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test(OrderGroup orderGroup,
                                         Cart cart,
                                         LineItem lineItem,
                                         OrderAddress orderAddress,
                                         OrderForm orderForm,
                                         OrderNote orderNote,
                                         PaymentPlan paymentPlan,
                                         PurchaseOrder purchaseOrder,
                                         Shipment shipment,
                                         TaxValue taxValue,
                                         Payment payment)
                        {
                            var billingCurrency = orderGroup.BillingCurrency;
                            var orderNumberMethod = cart.OrderNumberMethod;
                            var quantity = lineItem.Quantity;
                            var firstName = orderAddress.FirstName;
                            var handlingTotal = orderForm.HandlingTotal;
                            var title = orderNote.Title;
                            var startDate = paymentPlan.StartDate;
                            var trackingNumber = purchaseOrder.TrackingNumber;
                            var pickListId = shipment.PickListId;
                            var taxType = taxValue.TaxType;
                            var amount = payment.Amount;
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(21, 51).WithArguments("Mediachase.Commerce.Orders.OrderGroup"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(22, 53).WithArguments("Mediachase.Commerce.Orders.Cart"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(23, 44).WithArguments("Mediachase.Commerce.Orders.LineItem"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(24, 45).WithArguments("Mediachase.Commerce.Orders.OrderAddress"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(25, 49).WithArguments("Mediachase.Commerce.Orders.OrderForm"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(26, 41).WithArguments("Mediachase.Commerce.Orders.OrderNote"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(27, 45).WithArguments("Mediachase.Commerce.Orders.PaymentPlan"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(28, 50).WithArguments("Mediachase.Commerce.Orders.PurchaseOrder"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(29, 46).WithArguments("Mediachase.Commerce.Orders.Shipment"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(30, 43).WithArguments("Mediachase.Commerce.Orders.TaxValue"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(31, 42).WithArguments("Mediachase.Commerce.Orders.Payment"));
        }

        [Fact]
        public async Task DetectOrderClassMethods()
        {
            var test = @"
                using System;
                using Mediachase.Commerce.Orders;

                namespace Test
                {
                    public class TypeName
                    {
                        public void Test(OrderGroup orderGroup,
                                         Cart cart,
                                         LineItem lineItem,
                                         OrderAddress orderAddress,
                                         OrderForm orderForm,
                                         PaymentPlan paymentPlan,
                                         PurchaseOrder purchaseOrder,
                                         Shipment shipment,
                                         Payment payment)
                        {
                            orderGroup.AcceptChanges();
                            var orderNumber = cart.GenerateOrderNumber(cart);
                            lineItem.AcceptChanges();
                            orderAddress.AcceptChanges();
                            orderForm.AcceptChanges();
                            var order = paymentPlan.SaveAsPurchaseOrder();
                            purchaseOrder.AcceptChanges();
                            shipment.AddLineItemIndex(0);
                            payment.AcceptChanges();
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(19, 29).WithArguments("Mediachase.Commerce.Orders.OrderGroup"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(20, 47).WithArguments("Mediachase.Commerce.Orders.Cart"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(21, 29).WithArguments("Mediachase.Commerce.Orders.LineItem"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(22, 29).WithArguments("Mediachase.Commerce.Orders.OrderAddress"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(23, 29).WithArguments("Mediachase.Commerce.Orders.OrderForm"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(24, 41).WithArguments("Mediachase.Commerce.Orders.PaymentPlan"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(25, 29).WithArguments("Mediachase.Commerce.Orders.PurchaseOrder"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(26, 29).WithArguments("Mediachase.Commerce.Orders.Shipment"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(27, 29).WithArguments("Mediachase.Commerce.Orders.Payment"));
        }
    }
}
