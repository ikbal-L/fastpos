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

namespace ServiceLib.Service
{
    public class BaseRepository <TState, TIdentifier> : IRepository<TState, TIdentifier> where TState : IState<TIdentifier> where TIdentifier : struct
    {
        protected RestApis RestApi = new RestApis();
        protected string Resource = null;
        public virtual (int status, TState) Get(TIdentifier id)
        {
            return GenericRest.GetThing<TState>(RestApi.Resource<TState>(EndPoint.GET,arg:id));
        }

        public virtual (int status, IEnumerable<TState> state) Get()
        {
            return GenericRest.GetAll<TState>(RestApi.Resource<TState>(EndPoint.GET_ALL, resource:Resource));
        }

        public (int status, IEnumerable<TState>) Get(string subPath)
        {
            return GenericRest.GetAll<TState>(RestApi.Resource<TState>(EndPoint.GET_ALL,subPath:subPath));
        }


        public virtual (int status, IEnumerable<TState>) Get(IEnumerable<TIdentifier> ids)
        {
            return GenericRest.GetManyThings<TState>((IEnumerable<long>)ids, RestApi.Resource<TState>(EndPoint.GET_MANY));
        }

        public virtual int Save(TState state, out IEnumerable<string> errors)
        {
            return GenericRest.SaveThing(state, RestApi.Resource<TState>(EndPoint.SAVE),out errors).status;
        }

        public virtual (int status, IEnumerable<TState> state, IEnumerable<string> errors) Save(IEnumerable<TState> state)
        {
            var result = GenericRest.SaveThing<IEnumerable<TState>>(state, RestApi.Resource<TState>(EndPoint.SAVE_MANY));
            
            if (result.StatusCode == HttpStatusCode.OK|| result.StatusCode == HttpStatusCode.Created)
            {
                var content = JsonConvert.DeserializeObject<IEnumerable<TState>>(result.Content);
                return ((int) result.StatusCode, content,null);
            }
            var errors = JsonConvert.DeserializeObject<IEnumerable<string>>(result.Content);
            return ((int) result.StatusCode, null,errors);

        }

        public virtual (int status, IEnumerable<string> errors) Delete(TIdentifier id)
        {
            return GenericRest.Delete<TState>(RestApi.Resource<TState>(EndPoint.DELETE,id));
        }


        public virtual int Update(TState state, out IEnumerable<string> errors) 
        {
            
            return GenericRest.UpdateThing<TState>(state, RestApi.Resource<TState>(EndPoint.PUT, arg: state.Id), out errors).status;
        }

        

        public virtual (int status, IEnumerable<TState>) Get(object param)
        {
            throw new NotImplementedException();
        }

        public (int status, IEnumerable<string> errors) Update(IEnumerable<TState> state)
        {
            var result = GenericRest.UpdateThing(state, RestApi.Resource<TState>(EndPoint.PUT_MANY));
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return ((int)result.StatusCode, Array.Empty<string>());
            }

            var errors = JsonConvert.DeserializeObject<IEnumerable<string>>(result.Content);
            return ((int)result.StatusCode, errors);
        }

        public (bool, TReturn) Save<TReturn>(TState state)
        {
            return GenericRest.SaveThing<TState, TReturn>(state, RestApi.Resource<TState>(EndPoint.SAVE));
        }

        public (bool,TReturn) Update<TReturn>(TState state)
        {
            return GenericRest.UpdateThing<TState, TReturn>(state, RestApi.Resource<TState>(EndPoint.PUT,(TIdentifier)state.Id));
        }

        public (bool, TReturn) Delete<TReturn>(TIdentifier id)
        {
            return GenericRest.DeleteThing<TReturn>(RestApi.Resource<TState>(EndPoint.DELETE,id));
        }

        public int Delete(IEnumerable<TIdentifier> ids)
        {
            return GenericRest.DeleteThing(RestApi.Resource<TState>(EndPoint.DELETE_MANY),ids);
        }

        public async Task<(int, IEnumerable<TState>)> GetAsync()
        {
             return await GenericRest.GetThingAsync<IEnumerable<TState>>(RestApi.Resource<TState>(EndPoint.GET_ALL,resource:Resource));
        }

        public async Task<(int status, IEnumerable<TState>)> GetAsync(string subPath)
        {
            return await GenericRest.GetThingAsync<IEnumerable<TState>>(RestApi.Resource<TState>(EndPoint.GET_ALL, subPath: subPath));
        }

        public List<TState> GetByCriterias(object criterias)
        {

            var result = GenericRest.RestPost(criterias, RestApi.Resource<TState>(EndPoint.GET_ALL_BY_CRITERIAS));
            if (result.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<List<TState>>(result.Content);
            };
            return new List<TState>();
        }

        public async Task<List<TState>> GetByCriteriasAsync(object criterias)
        {

            var result =await GenericRest.RestPostAsync(criterias, RestApi.Resource<TState>(EndPoint.GET_ALL_BY_CRITERIAS));
            if (result.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<List<TState>>(result.Content);
            };
            return new List<TState>();
        }
    }




    [Export(typeof(IAdditiveRepository))]
    public class AdditiveBaseRepository : BaseRepository<Additive, long>, IAdditiveRepository
    {

    }

    [Export(typeof(IProductRepository))]
    public class ProductRepository : BaseRepository<Product, long>, IProductRepository
    {
        public override (int status, IEnumerable<Product> state) Get()
        {
            var result = GenericRest.GetAll<Product>(RestApi.Resource<Product>(EndPoint.GET_ALL));
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
            var response = GenericRest.SaveThing(state, RestApi.Resource<Order>(EndPoint.SAVE));
            PatchOrderFromResponse(state, response,out errors);
            return (int)response.StatusCode;
        }

        public override int Update(Order state, out IEnumerable<string> errors)
        {
            var response = GenericRest.UpdateThing(state, RestApi.Resource<Order>(EndPoint.PUT,arg: state.Id));
            PatchOrderFromResponse(state, response, out errors);
            return (int)response.StatusCode;
        }

        public static void PatchOrderFromResponse(Order state, RestSharp.IRestResponse response, out IEnumerable<string> errors)
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

                PatchOrder(state, deserializedState);

                deserializedState = null;
            }


        }

        public static void PatchOrder(Order state, Order deserializedState)
        {
            foreach (var newOrderItem in deserializedState.OrderItems.ToList())
            {
                OrderItem? oldOrderItem = state.OrderItems.FirstOrDefault(oi => oi.ProductId == newOrderItem.ProductId);
                if (oldOrderItem!= null)
                {
                    oldOrderItem.Id = newOrderItem.Id;
                    oldOrderItem.OrderItemAdditives = newOrderItem.OrderItemAdditives; 
                }

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
            RestApi.Prefix = "config";
        }
    }
    [Export(typeof(IRoleRepository))]
    public class RoleRepository : BaseRepository<Role, long>, IRoleRepository
    {
        public RoleRepository()
        {
            RestApi.Prefix = "config";
        }
    }

    [Export(typeof(IPermissionRepository))]
    public class PermissionRepository : BaseRepository<Privilege, long>, IPermissionRepository
    {
        public PermissionRepository()
        {
            RestApi.Prefix = "config";
            Resource = "Privilege";
        }
    }

    [Export(typeof(IDailyExpenseReportRepository))]
    public class DailyExpenseReportRepository : BaseRepository<DailyExpenseReport, long>, IDailyExpenseReportRepository
    {

    }

}