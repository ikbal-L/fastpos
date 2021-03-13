﻿using Newtonsoft.Json;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using EndPoint = ServiceInterface.StaticValues.EndPoint;

namespace ServiceLib.Service
{
    public class Repository <TState, TIdentifier> : IRepository<TState, TIdentifier> where TState : IState<TIdentifier> where TIdentifier : struct
    {

        public virtual (int status, TState) Get(TIdentifier id)
        {
            return GenericRest.GetThing<TState>(RestApis.Resource<TState>.Action(EndPoint.Get,arg:id));
        }

        public virtual (int status, IEnumerable<TState>) Get()
        {
            return GenericRest.GetAll<TState>(RestApis.Resource<TState>.Action(EndPoint.GetAll));
        }

        public (int status, IEnumerable<TState>) Get(string subPath)
        {
            return GenericRest.GetAll<TState>(RestApis.Resource<TState>.Action(EndPoint.GetAll,subPath:subPath));
        }


        public virtual (int status, IEnumerable<TState>) Get(IEnumerable<TIdentifier> ids)
        {
            return GenericRest.GetManyThings<TState>((IEnumerable<long>)ids, RestApis.Resource<TState>.Action(EndPoint.GetMany));
        }

        public virtual int Save(TState state, out IEnumerable<string> errors)
        {
            return GenericRest.SaveThing<TState>(state, RestApis.Resource<TState>.Action(EndPoint.Save),out errors).status;
        }

        public virtual int Save(IEnumerable<TState> state)
        {
            return GenericRest.SaveThing<IEnumerable<TState>>(state, RestApis.Resource<TState>.Action(EndPoint.SaveMany),out _).status;
        }

        public virtual int Delete(TIdentifier id)
        {
            return GenericRest.Delete<TState>(RestApis.Resource<TState>.Action(EndPoint.Delete,id));
        }


        public virtual int Update(TState state, out IEnumerable<string> errors) 
        {
            
            return GenericRest.UpdateThing<TState>(state, RestApis.Resource<TState>.Action(EndPoint.Put, arg: state.Id), out errors).status;
        }

        public virtual int Update(IEnumerable<TState> state)
        {
            return GenericRest.UpdateThing<IEnumerable<TState>>(state, RestApis.Resource<TState>.Action(EndPoint.PutMany), out _).status;
        }

        public virtual (int status, IEnumerable<TState>) Get(object param)
        {
            throw new NotImplementedException();
        }

        public (bool, TReturn) Save<TReturn>(TState state)
        {
            return GenericRest.SaveThing<TState, TReturn>(state, RestApis.Resource<TState>.Action(EndPoint.Save));
        }

        public (bool, TReturn) Update<TReturn>(TState state)
        {
            return GenericRest.UpdateThing<TState, TReturn>(state, RestApis.Resource<TState>.Action(EndPoint.Put,(TIdentifier)state.Id));
        }

        public (bool, TReturn) Delete<TReturn>(TIdentifier id)
        {
            return GenericRest.DeleteThing<TReturn>(RestApis.Resource<TState>.Action(EndPoint.Delete,id));
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
            var result = GenericRest.GetAll<Product>(RestApis.Resource<Product>.Action(EndPoint.GetAll));
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
        

        public override int Save(Order state, out IEnumerable<string> errors)
        {
            var response = GenericRest.SaveThing(state, RestApis.Resource<Order>.Action(EndPoint.Save));
            PatchOrder(state, response,out errors);
            return (int)response.StatusCode;
        }

        public override int Update(Order state, out IEnumerable<string> errors)
        {
            var response = GenericRest.UpdateThing(state, RestApis.Resource<Order>.Action(EndPoint.Put,arg: state.Id));
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

                foreach (var newOrderItem in deserializedState.OrderItems)
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
    [Export(typeof(IPaymentRepository))]
    public class PaymentRepository : Repository<Payment, long>, IPaymentRepository
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
    public class UserRepository : Repository<User, long>, IUserRepository
    {

    }

}