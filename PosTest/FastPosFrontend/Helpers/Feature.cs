using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FastPosFrontend.Helpers
{
    public class Feature
    {
        public bool IsLoaded { get; private set; }

        public Dictionary<Type, FeatureContext> Contexts { get; private set; } = new();

        public void RegisterContext<TContext>(FeatureContext context) 
        {
            Contexts.Add(typeof(TContext), context);
        }
        public void Execute<TContext>([CallerMemberName] string userCase = null)
        {
            if (!IsLoaded) return;
            var contextType = typeof(TContext);
            if (Contexts.ContainsKey(contextType))
            {
                Contexts[contextType][userCase]?.Invoke();
            }
        }
    }

    public class FeatureContext
    {
        public object Context { get; set; }

        public Dictionary<string, Action> UseCases { get; set; } = new();

        public Action? this[string userCase] => UseCases.TryGetValue(userCase, out var action) ? action : default;

    }

    public class FeatureFactory
    {
        public void Register()
        {

        }
    }

    public class ObserveMutationsFeature:Feature
    {
        public ObserveMutationsFeature()
        {

        }
    }


}
