using ServiceInterface.Model;

namespace FastPosFrontend.ViewModels
{
    public static class OrderFilterCriteria
    {
        public static readonly string TIME = "Time";
        public static readonly string PRICE = "Price";
        public static readonly string ITEM_COUNT = "Item Count";
        public static readonly string[] CRITERIAS = { PRICE,TIME,ITEM_COUNT};
        public static readonly string[] SECONDARY_CRITERIA_ORDER_STATE = 
         { 
            Secondary.OrderStateCriteria.ALL, 
            Secondary.OrderStateCriteria.DELIVERED,
            Secondary.OrderStateCriteria.DELIVERED_PAID, 
            Secondary.OrderStateCriteria.DELIVERED_RETURNED 
        };
        public static class Secondary
        {
            public static class OrderStateCriteria
            {
                public static readonly string ALL = "All";
                public static readonly string DELIVERED = "Delivered";
                public static readonly string DELIVERED_PAID = "Delivered & Paid";
                public static readonly string DELIVERED_RETURNED = "Delivered & Returned";
                
            }

            
        }
    }
}
