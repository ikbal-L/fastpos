using Caliburn.Micro;
using ServiceInterface.Model;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastPosFrontend.Helpers
{
    public static class OrderItemEx
    {
        public static bool AddAdditive(this OrderItem orderItem, Additive additive)
        {
            if (orderItem.Additives == null)
            {
                orderItem.Additives = new BindableCollection<Additive>();
            }
            if (orderItem.Additives.Any() && orderItem.Additives.Any(a => a.Equals(additive))) return false;


            var additiveToAdd = additive.Clone();
            additiveToAdd.ParentOrderItem = orderItem;
            orderItem.Additives.Add(additiveToAdd);
            if (additiveToAdd.Id != null) orderItem.OrderItemAdditives.Add(new OrderItemAdditive() { AdditiveId = (long)additiveToAdd.Id, OrderItemId = orderItem.Id, State = AdditiveState.Added });
            return true;
        }

        public static void RemoveAdditive(this OrderItem orderItem, Additive additive)
        {


            if (orderItem.Order.Id == null)
            {

                orderItem.OrderItemAdditives.RemoveAll(orderItemAdditive => orderItemAdditive.AdditiveId == additive.Id);

            }
            else
            {
                orderItem.OrderItemAdditives.Find(oia => oia.AdditiveId == additive.Id).State = AdditiveState.Removed;
            }
            orderItem.Additives.Remove(additive);
        }
    }
}
