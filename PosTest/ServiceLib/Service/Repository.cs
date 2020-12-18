using Newtonsoft.Json;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;

namespace ServiceLib.Service
{
    public class Repository <TState, TIdentifier> : IRepository<TState, TIdentifier> where TState : IState<TIdentifier> where TIdentifier : struct
    {

        public virtual (int status, TState) Get(TIdentifier id)
        {
            return GenericRest.GetThing<TState>(RestApis.Resource<TState>.Get(id));
        }

        public virtual (int status, IEnumerable<TState>) Get()
        {
            return GenericRest.GetAll<TState>(RestApis.Resource<TState>.GetAll());
        }


        public virtual (int status, IEnumerable<TState>) Get(IEnumerable<TIdentifier> ids)
        {
            return GenericRest.GetManyThings<TState>((IEnumerable<long>)ids, RestApis.Resource<TState>.GetMany());
        }

        public virtual int Save(TState state, out IEnumerable<string> errors)
        {
            return GenericRest.SaveThing<TState>(state, RestApis.Resource<TState>.Save(),out errors).status;
        }

        public virtual int Delete(TIdentifier id)
        {
            return GenericRest.Delete<TState>(RestApis.Resource<TState>.Delete(id));
        }


        public virtual int Update(TState state, out IEnumerable<string> errors) 
        {
            return GenericRest.UpdateThing<TState>(state,RestApis.Resource<TState>.Put<TIdentifier>((TIdentifier) state.Id),out errors).status;
        }

        public virtual int Update(IEnumerable<TState> state)
        {
            return GenericRest.UpdateThing<IEnumerable<TState>>(state, RestApis.Resource<TState>.UpdateMany(),out _).status;
        }

        public virtual (int status, IEnumerable<TState>) Get(object param)
        {
            throw new NotImplementedException();
        }
    }




    [Export(typeof(IAdditiveRepository))]
    public class AdditiveRepository : Repository<Additive, long>, IAdditiveRepository
    {

    }

    [Export(typeof(IProductRepository))]
    public class ProductRepository : Repository<Product, long>, IProductRepository
    {
        public override (int status, IEnumerable<Product>) Get()
        {
            var result = GenericRest.GetAll<Platter>(RestApis.Resource<Product>.GetAll());
            var products = result.Item2.Cast<Product>().ToList();
            return (result.Item1,products ) ;
        }
    }

    [Export(typeof(ICategoryRepository))]
    public class CategoryRepository : Repository<Category, long>, ICategoryRepository
    {

    }

    [Export(typeof(IOrderRepository))]
    public class OrderRepository : Repository<Order, long>, IOrderRepository
    {
        public override (int status, IEnumerable<Order>) Get(object unprocessed)
        {
            var suffix = "";
            if (unprocessed is bool b && b)
            {
                suffix = nameof(unprocessed);
            }
            return GenericRest.GetAll<Order>(RestApis.Resource<Order>.GetAll() + suffix);
        }

        public override int Save(Order state, out IEnumerable<string> errors)
        {
            var response = GenericRest.SaveThing(state, RestApis.Resource<Order>.Save());
            PatchOrder(state, response,out errors);
            return (int)response.StatusCode;
        }

        public override int Update(Order state, out IEnumerable<string> errors)
        {
            var response = GenericRest.UpdateThing(state, RestApis.Resource<Order>.Save());
            PatchOrder(state, response, out errors);
            return (int)response.StatusCode;
        }

        private static void PatchOrder(Order state, RestSharp.IRestResponse response, out IEnumerable<string> errors)
        {
            errors = null;

            if (response.StatusCode != HttpStatusCode.Created || response.StatusCode != HttpStatusCode.OK)
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

                foreach (var newOrderItem in deserializedState.OrderItems)
                {
                    var oldOrderItem = state.OrderItems.FirstOrDefault(oi => oi.ProductId == newOrderItem.ProductId);
                    newOrderItem.Additives = oldOrderItem.Additives;
                }
                state.OrderItems = deserializedState.OrderItems;
                deserializedState = null;
            }


        }

    }

    [Export(typeof(ITableRepository))]
    public class TableRepository : Repository<Table, long>,ITableRepository
    {

    }

    [Export(typeof(ICustomerRepository))]
    public class CustomerRepository : Repository<Customer, long>, ICustomerRepository
    {

    }

    [Export(typeof(IWaiterRepository))]
    public class WaiterRepository : Repository<Waiter, long>, IWaiterRepository
    {

    }

    [Export(typeof(IDeliverymanRepository))]
    public class DeliverymanRepository : Repository<Deliveryman, long>, IDeliverymanRepository
    {

    }

}