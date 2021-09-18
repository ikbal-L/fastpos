namespace FastPosFrontend.ViewModels
{
    public class DeliverymanFilterCriteria
    {
        public static readonly string NAME = "Name";
        public static readonly string MOBILE = "Mobile";
        public static readonly string[] CRITERIAS = { NAME, MOBILE};
    }

    public class DeliveryCheckoutFilter 
    {
        public static readonly string DELIVERY_MAN = "Deliveryman";
        public static readonly string ORDER = "Order";
        public static readonly string PAYMENT = "Payment";
        public static readonly string[] OPTIONS = { DELIVERY_MAN, ORDER,PAYMENT };
    }
}
