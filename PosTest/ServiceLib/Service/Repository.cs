using System;
using System.Collections.Generic;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;

namespace ServiceLib.Service
{
    public interface IRepository<TState, in TIdentifier> where TState : IState<TIdentifier> where TIdentifier : struct
    {
        (int status, TState) Get(TIdentifier id);
        (int status, IEnumerable<TState>) Get();
        (int status, IEnumerable<TState>) Get(IEnumerable<TIdentifier> ids);
        int Save(TState state, out IEnumerable<string> errors);
        int Delete(TIdentifier id);
        int Update(TState state);
        int Update(IEnumerable<TState> state);
    }

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
    }

    public class TestModel : IState<Guid>
    {
        public Guid? Id { get; set; }
    }

    public interface ITestRepository : IRepository<TestModel, Guid>
    {

    }

    public class TestRepository : Repository<TestModel, Guid>, ITestRepository
    {

    }

    public interface IAdditiveRepository : IRepository<Additive, long>
    {

    }

    public class AdditiveRepository : Repository<Additive, long>, IAdditiveRepository
    {

    }
}