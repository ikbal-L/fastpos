using System;
using System.Collections.Generic;

namespace ServiceLib.helpers
{
    public partial class AssociationManager
    {
        private partial class AssociationBuilder
        {
            private readonly AssociationManager _manager;

            public AssociationBuilder(AssociationManager manager)
            {
                _manager = manager;
            }

            public Type Target { get; private set; }
            public Type Source { get; private set; }
            public Action<IEnumerable<object>, IEnumerable<object>> Map { get; private set; }

            public IManyToOne Associate<TOne>()
            {
                Target = typeof(TOne);
                return this;
            }

            public IAssociationAction With<TMany>()
            {
                Source = typeof(TMany);
                return this;
            }

            public IAssociationBuild Using<TOne, TMany>(Action<IEnumerable<TOne>, IEnumerable<TMany>> map)
            {
                Map = (one, many) =>
                {
                    if (one is IEnumerable<TOne> TOne && many is IEnumerable<TMany> TMany)
                    {
                        map(TOne, TMany);
                    }
                    else
                    {
                        throw new InvalidOperationException(
                            $"Expected{typeof(TOne)} got type of {one.GetType()},Expected{typeof(TMany)} got type of {many.GetType()} ");
                    }
                };
                return this;
            }

            public IAssociationBuilder Build()
            {
                var mapAction = Map;
                if (_manager._associations.ContainsKey(Target))
                {
                    _manager._associations[Target].Add(Source, mapAction);
                }
                else
                {
                    var mappers = new Dictionary<Type, Action<IEnumerable<object>, IEnumerable<object>>>
                    {
                        {Source, mapAction}
                    };
                    _manager._associations.Add(Target, mappers);
                }

                return _manager;
            }
        }
    }
}