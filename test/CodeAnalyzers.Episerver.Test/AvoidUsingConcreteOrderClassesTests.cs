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
                        public void Test(ILineItem lineItem,
                                         IOrderAddress orderAddress,
                                         IOrderForm orderForm,
                                         IOrderNote orderNote,
                                         IPaymentPlan paymentPlan,
                                         IPurchaseOrder purchaseOrder,
                                         IShipment shipment,
                                         ITaxValue taxValue)
                        {
                            var quantity = lineItem.Quantity;
                            var firstName = orderAddress.FirstName;
                            var handlingTotal = orderForm.HandlingTotal;
                            var title = orderNote.Title;
                            var startDate = paymentPlan.StartDate;
                            var orderNumber = purchaseOrder.OrderNumber;
                            var pickListId = shipment.PickListId;
                            var taxType = taxValue.TaxType;
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
                        public void Test(Cart cart,
                                         LineItem lineItem,
                                         OrderAddress orderAddress,
                                         OrderForm orderForm,
                                         OrderNote orderNote,
                                         PaymentPlan paymentPlan,
                                         PurchaseOrder purchaseOrder,
                                         Shipment shipment,
                                         TaxValue taxValue)
                        {
                            var orderNumberMethod = cart.OrderNumberMethod;
                            var quantity = lineItem.Quantity;
                            var firstName = orderAddress.FirstName;
                            var handlingTotal = orderForm.HandlingTotal;
                            var title = orderNote.Title;
                            var startDate = paymentPlan.StartDate;
                            var trackingNumber = purchaseOrder.TrackingNumber;
                            var pickListId = shipment.PickListId;
                            var taxType = taxValue.TaxType;
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(19, 53).WithArguments("Mediachase.Commerce.Orders.Cart"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(20, 44).WithArguments("Mediachase.Commerce.Orders.LineItem"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(21, 45).WithArguments("Mediachase.Commerce.Orders.OrderAddress"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(22, 49).WithArguments("Mediachase.Commerce.Orders.OrderForm"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(23, 41).WithArguments("Mediachase.Commerce.Orders.OrderNote"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(24, 45).WithArguments("Mediachase.Commerce.Orders.PaymentPlan"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(25, 50).WithArguments("Mediachase.Commerce.Orders.PurchaseOrder"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(26, 46).WithArguments("Mediachase.Commerce.Orders.Shipment"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(27, 43).WithArguments("Mediachase.Commerce.Orders.TaxValue"));
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
                        public void Test(Cart cart,
                                         LineItem lineItem,
                                         OrderAddress orderAddress,
                                         OrderForm orderForm,
                                         PaymentPlan paymentPlan,
                                         PurchaseOrder purchaseOrder,
                                         Shipment shipment)
                        {
                            var orderNumber = cart.GenerateOrderNumber(cart);
                            lineItem.AcceptChanges();
                            orderAddress.AcceptChanges();
                            orderForm.AcceptChanges();
                            var order = paymentPlan.SaveAsPurchaseOrder();
                            purchaseOrder.AcceptChanges();
                            shipment.AddLineItemIndex(0);
                        }
                    }
                }";

            await Verify.VerifyAnalyzerAsync(test,
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(17, 47).WithArguments("Mediachase.Commerce.Orders.Cart"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(18, 29).WithArguments("Mediachase.Commerce.Orders.LineItem"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(19, 29).WithArguments("Mediachase.Commerce.Orders.OrderAddress"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(20, 29).WithArguments("Mediachase.Commerce.Orders.OrderForm"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(21, 41).WithArguments("Mediachase.Commerce.Orders.PaymentPlan"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(22, 29).WithArguments("Mediachase.Commerce.Orders.PurchaseOrder"),
                Verify.Diagnostic(Descriptors.Epi1007AvoidUsingConcreteOrderClasses).WithLocation(23, 29).WithArguments("Mediachase.Commerce.Orders.Shipment"));
        }
    }
}
