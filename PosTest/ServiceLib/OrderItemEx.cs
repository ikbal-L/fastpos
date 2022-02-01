using Caliburn.Micro;
using ServiceInterface.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastPosFrontend.Helpers
{
    public static class OrderItemEx
    {
        public static bool AddAdditive(this OrderItem orderItem, Additive additive,string? modifier= null)
        {

            if (orderItem.OrderItemAdditives == null) return false;


            var oia = orderItem.OrderItemAdditives.FirstOrDefault(e =>e.Additive.Equals(additive));
            if (oia != null)
            {
                if (oia.Modifier != modifier)
                {
                    oia.Modifier = modifier;
                }
            }
            else
            {
                orderItem.OrderItemAdditives.Add(new OrderItemAdditive()
                {
                    AdditiveId = (long)additive.Id,
                    OrderItemId = orderItem.Id,
                    State = AdditiveState.Added,
                    Modifier = modifier,
                    Additive = additive,
                    OrderItem = orderItem
                });
            }


        
            return true;
        }

        public static void RemoveAdditive(this OrderItem orderItem, OrderItemAdditive orderiItemAdditive)
        {


            if (orderItem.Order.Id == null)
            {
                orderItem.OrderItemAdditives.Remove(orderiItemAdditive);
            }
            else
            {
                orderiItemAdditive.State = AdditiveState.Removed;
                orderiItemAdditive.OrderItem.OrderItemAdditivesView.Refresh();
            }

        }

        
    }
}
