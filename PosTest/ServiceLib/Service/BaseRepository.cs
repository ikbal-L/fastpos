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
    public class BaseRepository <T, Id> : IRepository<T, Id> where T : IState<Id> where Id : struct
    {
        
        protected string Resource = null;
 
        public BaseRepository()
        {

        }

        public IRestApi Api { get; set; } = new RestApi();
        public string BaseUrl
        {
            get => Api.BaseUrl; 
            set => Api.BaseUrl = value; 
        }

        public virtual (int status, T) GetById(Id id)
        {
            return GenericRest.GetThing<T>(Api.Resource<T>(EndPoint.GET,arg:id));
        }

        public virtual (int status, IEnumerable<T> state) GetAll()
        {
            return GenericRest.GetAll<T>(Api.Resource<T>(EndPoint.GET_ALL, resource:Resource));
        }

        public (int status, IEnumerable<T>) Get(string subPath)
        {
            return GenericRest.GetAll<T>(Api.Resource<T>(EndPoint.GET_ALL,subPath:subPath));
        }


        public virtual (int status, IEnumerable<T>) GetByIds(IEnumerable<Id> ids)
        {
            return GenericRest.GetManyThings<T>((IEnumerable<long>)ids, Api.Resource<T>(EndPoint.GET_MANY));
        }

        public virtual (int status, IEnumerable<string> errors) Save(T state)
        {
             var res = GenericRest.SaveThing(state, Api.Resource<T>(EndPoint.SAVE), out var errors);
            return (res.status, errors);
        }

        public virtual (int status, IEnumerable<T> state, IEnumerable<string> errors) SaveAll(IEnumerable<T> state)
        {
            var result = GenericRest.SaveThing(state, Api.Resource<T>(EndPoint.SAVE_MANY));
            
            if (result.StatusCode == HttpStatusCode.OK|| result.StatusCode == HttpStatusCode.Created)
            {
                var content = JsonConvert.DeserializeObject<IEnumerable<T>>(result.Content);
                return ((int) result.StatusCode, content,null);
            }
            var errors = JsonConvert.DeserializeObject<IEnumerable<string>>(result.Content);
            return ((int) result.StatusCode, null,errors);

        }

        public virtual (int status, IEnumerable<string> errors) DeleteById(Id id)
        {
            return GenericRest.Delete<T>(Api.Resource<T>(EndPoint.DELETE,id));
        }


        public virtual (int status, IEnumerable<string> errors) Update(T state) 
        {
            var res =  GenericRest.UpdateThing<T>(state, Api.Resource<T>(EndPoint.PUT, arg: state.Id), out var errors);
            return (res.status, errors);
        }

        

        public virtual (int status, IEnumerable<T>) Get(object param)
        {
            throw new NotImplementedException();
        }

        public (int status, IEnumerable<string> errors) Update(IEnumerable<T> state)
        {
            var result = GenericRest.UpdateThing(state, Api.Resource<T>(EndPoint.PUT_MANY));
            if (result.StatusCode == HttpStatusCode.OK)
            {
                return ((int)result.StatusCode, Array.Empty<string>());
            }

            var errors = JsonConvert.DeserializeObject<IEnumerable<string>>(result.Content);
            return ((int)result.StatusCode, errors);
        }

        public (bool, TReturn) Save<TReturn>(T state)
        {
            return GenericRest.SaveThing<T, TReturn>(state, Api.Resource<T>(EndPoint.SAVE));
        }

        public (bool,TReturn) Update<TReturn>(T state)
        {
            return GenericRest.UpdateThing<T, TReturn>(state, Api.Resource<T>(EndPoint.PUT,(Id)state.Id));
        }



        public int Delete(IEnumerable<Id> ids)
        {
            return GenericRest.DeleteThing(Api.Resource<T>(EndPoint.DELETE_MANY),ids);
        }

        public async Task<(int, IEnumerable<T>)> GetAsync()
        {
             return await GenericRest.GetThingAsync<IEnumerable<T>>(Api.Resource<T>(EndPoint.GET_ALL,resource:Resource));
        }

        public async Task<(int status, IEnumerable<T>)> GetAsync(string subPath)
        {
            return await GenericRest.GetThingAsync<IEnumerable<T>>(Api.Resource<T>(EndPoint.GET_ALL, subPath: subPath));
        }

        public List<T> GetByCriterias(object criterias)
        {

            var result = GenericRest.RestPost(criterias, Api.Resource<T>(EndPoint.GET_ALL_BY_CRITERIAS));
            if (result.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<List<T>>(result.Content);
            };
            return new List<T>();
        }

        public async Task<List<T>> GetByCriteriasAsync(object criterias)
        {

            var result =await GenericRest.RestPostAsync(criterias, Api.Resource<T>(EndPoint.GET_ALL_BY_CRITERIAS));
            if (result.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<List<T>>(result.Content);
            };
            return new List<T>();
        }

        public async Task<(int status, T resource)> GetByIdAsync(Id id)
        {
            return await RestService.Instance.GetResourceAsync<T>(Api.Resource<T>(EndPoint.GET, arg: id));
        }
    }


    [Export(typeof(IAdditiveRepository))]
    public class AdditiveBaseRepository : BaseRepository<Additive, long>, IAdditiveRepository
    {

    }

    [Export(typeof(IProductRepository))]
    public class ProductRepository : BaseRepository<Product, long>, IProductRepository
    {
        public override (int status, IEnumerable<Product> state) GetAll()
        {
            var result = GenericRest.GetThing<List<Product>>(Api.Resource<Product>(EndPoint.GET_ALL));
            var products = result.result;
            return (result.Item1,products ) ;
        }
    }

    [Export(typeof(ICategoryRepository))]
    public class CategoryBaseRepository : BaseRepository<Category, long>, ICategoryRepository
    {

    }

    [Export(typeof(IOrderRepository))]
    public class OrderRepository : BaseRepository<Order, long>, IOrderRepository
    {
        

        public override (int status, IEnumerable<string> errors) Save(Order state)
        {
            var response = GenericRest.SaveThing(state, Api.Resource<Order>(EndPoint.SAVE));
            PatchOrderFromResponse(state, response,out var errors);
            return ((int)response.StatusCode, errors);
        }

        public override (int status, IEnumerable<string> errors) Update(Order state)
        {
            var response = GenericRest.UpdateThing(state, Api.Resource<Order>(EndPoint.PUT,arg: state.Id));
            PatchOrderFromResponse(state, response, out var errors);
            return ((int)response.StatusCode, errors);
        }

        public static void PatchOrderFromResponse(Order state, RestSharp.IRestResponse response, out IEnumerable<string> errors)
        {
            errors = Array.Empty<string>();

            if (response.StatusCode != HttpStatusCode.Created && response.StatusCode != HttpStatusCode.OK)
            {
                try
                {
                    errors  = JsonConvert.DeserializeObject<IEnumerable<string>>(response.Content);
                }
                catch (Exception)
                {

                    
                }
            }

            else
            {
                var deserializedState = JsonConvert.DeserializeObject<Order>(response.Content);
                if (response.StatusCode == HttpStatusCode.Created)
                {
                    state.Id = deserializedState.Id;
                }

                PatchOrder(state, deserializedState);
            }


        }

        public static void PatchOrder(Order state, Order result)
        {
            state.OrderNumber = result.OrderNumber;
            state.OrderCode = result.OrderCode;
            state.NotifyOfPropertyChange(nameof(Order.OrderNumber));
            state.NotifyOfPropertyChange(nameof(Order.OrderCode));
            foreach (var newOrderItem in result.OrderItems.ToList())
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
            Api.Prefix = "config";
        }
    }
    [Export(typeof(IRoleRepository))]
    public class RoleRepository : BaseRepository<Role, long>, IRoleRepository
    {
        public RoleRepository()
        {
            Api.Prefix = "config";
        }
    }

    [Export(typeof(IPermissionRepository))]
    public class PermissionRepository : BaseRepository<Privilege, long>, IPermissionRepository
    {
        public PermissionRepository()
        {
            Api.Prefix = "config";
            Resource = "Privilege";
        }
    }

    [Export(typeof(IDailyEarningsReportRepository))]
    public class DailyExpenseReportRepository : BaseRepository<DailyEarningsReport, long>, IDailyEarningsReportRepository
    {
        public DailyExpenseReportRepository()
        {
            Resource = "daily-earnings-report";
        }
    }

    [Export(typeof(ICashRegisterExpenseRepository))]
    public class CashRegisterExpenseRepository : BaseRepository<CashRegisterExpense, long>, ICashRegisterExpenseRepository
    {
        
        public  List<string> GetEmployees()
        {
            var result = GenericRest.GetThing<List<string>>(Api.Resource("cashregisterexpense", "employees", EndPoint.GET_ALL.ToLower()));
            if (result.status == 200)
            {
                return result.Item2;
            }
            return new List<string>();
        }
    }

    [Export(typeof(IExpenseDescriptionRepository))]
    public class ExpenseDescriptionRepository : BaseRepository<ExpenseDescription, long>, IExpenseDescriptionRepository
    {

    }

}