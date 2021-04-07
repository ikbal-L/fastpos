using Newtonsoft.Json;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EndPoint = ServiceInterface.StaticValues.EndPoint;

namespace ServiceLib.Service
{
    public class BaseRepository <TState, TIdentifier> : IRepository<TState, TIdentifier> where TState : IState<TIdentifier> where TIdentifier : struct
    {
        protected RestApis restApi = new RestApis();
        public virtual (int status, TState) Get(TIdentifier id)
        {
            return GenericRest.GetThing<TState>(restApi.Action<TState>(EndPoint.Get,arg:id));
        }

        public virtual (int status, IEnumerable<TState>) Get()
        {
            return GenericRest.GetAll<TState>(restApi.Action<TState>(EndPoint.GetAll));
        }

        public (int status, IEnumerable<TState>) Get(string subPath)
        {
            return GenericRest.GetAll<TState>(restApi.Action<TState>(EndPoint.GetAll,subPath:subPath));
        }


        public virtual (int status, IEnumerable<TState>) Get(IEnumerable<TIdentifier> ids)
        {
            return GenericRest.GetManyThings<TState>((IEnumerable<long>)ids, restApi.Action<TState>(EndPoint.GetMany));
        }

        public virtual int Save(TState state, out IEnumerable<string> errors)
        {
            return GenericRest.SaveThing<TState>(state, restApi.Action<TState>(EndPoint.Save),out errors).status;
        }

        public virtual (int status, IEnumerable<TState> state, IEnumerable<string> errors) Save(IEnumerable<TState> state)
        {
            var result = GenericRest.SaveThing<IEnumerable<TState>>(state, restApi.Action<TState>(EndPoint.SaveMany));
            
            if (result.StatusCode == HttpStatusCode.OK|| result.StatusCode == HttpStatusCode.Created)
            {
                var content = JsonConvert.DeserializeObject<IEnumerable<TState>>(result.Content);
                return ((int) result.StatusCode, content,null);
            }
            var errors = JsonConvert.DeserializeObject<IEnumerable<string>>(result.Content);
            return ((int) result.StatusCode, null,errors);

        }

        public virtual int Delete(TIdentifier id)
        {
            return GenericRest.Delete<TState>(restApi.Action<TState>(EndPoint.Delete,id));
        }


        public virtual int Update(TState state, out IEnumerable<string> errors) 
        {
            
            return GenericRest.UpdateThing<TState>(state, restApi.Action<TState>(EndPoint.Put, arg: state.Id), out errors).status;
        }

        public virtual int Update(IEnumerable<TState> state)
        {
            return GenericRest.UpdateThing<IEnumerable<TState>>(state, restApi.Action<TState>(EndPoint.PutMany), out _).status;
        }

        public virtual (int status, IEnumerable<TState>) Get(object param)
        {
            throw new NotImplementedException();
        }

        public (bool, TReturn) Save<TReturn>(TState state)
        {
            return GenericRest.SaveThing<TState, TReturn>(state, restApi.Action<TState>(EndPoint.Save));
        }

        public (bool,TReturn) Update<TReturn>(TState state)
        {
            return GenericRest.UpdateThing<TState, TReturn>(state, restApi.Action<TState>(EndPoint.Put,(TIdentifier)state.Id));
        }

        public (bool, TReturn) Delete<TReturn>(TIdentifier id)
        {
            return GenericRest.DeleteThing<TReturn>(restApi.Action<TState>(EndPoint.Delete,id));
        }

        public int Delete(IEnumerable<TIdentifier> ids)
        {
            return GenericRest.DeleteThing(restApi.Action<TState>(EndPoint.DeleteMany),ids);
        }

        public async Task<(int, IEnumerable<TState>)> GetAsync()
        {
             return await GenericRest.GetThingAsync<IEnumerable<TState>>(restApi.Action<TState>(EndPoint.GetAll));
        }

        public async Task<(int status, IEnumerable<TState>)> GetAsync(string subPath)
        {
            return await GenericRest.GetThingAsync<IEnumerable<TState>>(restApi.Action<TState>(EndPoint.GetAll, subPath: subPath));
        }
    }




    [Export(typeof(IAdditiveRepository))]
    public class AdditiveBaseRepository : BaseRepository<Additive, long>, IAdditiveRepository
    {

    }

    [Export(typeof(IProductRepository))]
    public class ProductRepository : BaseRepository<Product, long>, IProductRepository
    {
        public override (int status, IEnumerable<Product>) Get()
        {
            var result = GenericRest.GetAll<Product>(restApi.Action<Product>(EndPoint.GetAll));
            var products = result.Item2.Cast<Product>().ToList();
            return (result.Item1,products ) ;
        }
    }

    [Export(typeof(ICategoryRepository))]
    public class CategoryBaseRepository : BaseRepository<Category, long>, ICategoryRepository
    {

    }

    [Export(typeof(IOrderRepository))]
    public class OrderBaseRepository : BaseRepository<Order, long>, IOrderRepository
    {
        

        public override int Save(Order state, out IEnumerable<string> errors)
        {
            var response = GenericRest.SaveThing(state, restApi.Action<Order>(EndPoint.Save));
            PatchOrder(state, response,out errors);
            return (int)response.StatusCode;
        }

        public override int Update(Order state, out IEnumerable<string> errors)
        {
            var response = GenericRest.UpdateThing(state, restApi.Action<Order>(EndPoint.Put,arg: state.Id));
            PatchOrder(state, response, out errors);
            return (int)response.StatusCode;
        }

        private static void PatchOrder(Order state, RestSharp.IRestResponse response, out IEnumerable<string> errors)
        {
            errors = null;

            if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
            {
                errors = JsonConvert.DeserializeObject<IEnumerable<string>>(response.Content);
            }

            else
            {
                var deserializedState = JsonConvert.DeserializeObject<Order>(response.Content);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    state.Id = deserializedState.Id;
                }

                foreach (var newOrderItem in deserializedState.OrderItems.ToList())
                {
                    var oldOrderItem = state.OrderItems.FirstOrDefault(oi => oi.ProductId == newOrderItem.ProductId);
                    if (oldOrderItem?.Additives != null)
                    {
                        foreach (var additive in oldOrderItem.Additives)
                        {
                            additive.ParentOrderItem = newOrderItem;
                        } 
                    }
                    newOrderItem.Additives = oldOrderItem?.Additives;
                    newOrderItem.Order = oldOrderItem?.Order;
                    newOrderItem.Product = oldOrderItem?.Product;

                }
                state.OrderItems = deserializedState.OrderItems;
                deserializedState = null;
            }


        }

        public List<Order> GetOrderByStates(string[] states, long deliverymanId)
        {
            return GenericRest.PostThing<List<Order>>(UrlConfig.OrderUrl.GetByStatePage + $"/{deliverymanId}",states).Item2;
        }
        public PageList<Order> getAllByDeliveryManAndStatePage( int pageNumber, int pageSize, long deliverymanId, string[] states)
        {
            return GenericRest.PostThing<PageList<Order>>(UrlConfig.OrderUrl.GetAllByDeliveryManPage + $"/{pageNumber}/{pageSize}/{deliverymanId}", states).Item2;
        }
    }

    [Export(typeof(ITableRepository))]
    public class TableRepository : BaseRepository<Table, long>,ITableRepository
    {

    }

    [Export(typeof(ICustomerRepository))]
    public class CustomerRepository : BaseRepository<Customer, long>, ICustomerRepository
    {

    }

    [Export(typeof(IWaiterRepository))]
    public class WaiterRepository : BaseRepository<Waiter, long>, IWaiterRepository
    {

    }

    [Export(typeof(IDeliverymanRepository))]
    public class DeliverymanRepository : BaseRepository<Deliveryman, long>, IDeliverymanRepository
    {

    }
    [Export(typeof(IPaymentRepository))]
    public class PaymentRepository : BaseRepository<Payment, long>, IPaymentRepository
    {
        public PageList<Payment> getAllByDeliveryManPage(int pageNumber, int pageSize, long deliverymanId)
        {
            return GenericRest.GetThing<PageList<Payment>>(UrlConfig.PaymentUrl.GetAllByDeliveryManPage + $"/{pageNumber}/{pageSize}/{deliverymanId}").Item2;
        }

        public List<Payment> getByDeliveryManAndDate(long deliverymanId, DateTime date)
        {
           return GenericRest.PostThing<List<Payment>>(UrlConfig.PaymentUrl.GetByDeliveryManAndDate + $"/{deliverymanId}", date).Item2;
        }
    }

    [Export(typeof(IUserRepository))]
    public class UserRepository : BaseRepository<User, long>, IUserRepository
    {
        public UserRepository()
        {
            restApi.Prefix = "config";
        }
    }
    [Export(typeof(IRoleRepository))]
    public class RoleRepository : BaseRepository<Role, long>, IRoleRepository
    {
        public RoleRepository()
        {
            restApi.Prefix = "config";
        }
    }

}