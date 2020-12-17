using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;

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


        public virtual int Update(TState state) 
        {
            return GenericRest.UpdateThing<TState>(state,RestApis.Resource<TState>.Put<TIdentifier>((TIdentifier) state.Id)).status;
        }

        public virtual int Update(IEnumerable<TState> state)
        {
            return GenericRest.UpdateThing<IEnumerable<TState>>(state, RestApis.Resource<TState>.UpdateMany()).status;
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
            return GenericRest.GetAll<Order>(RestApis.Resource<Order>.GetAll()+suffix);
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