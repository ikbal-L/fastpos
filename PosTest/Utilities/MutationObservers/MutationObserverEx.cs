using System;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Utilities.Mutation.Observers.Extentions
{
    public static class MutationObserverEx
    {
        
        public static void MethodGuard(this IMutationObserver observer, [CallerMemberName] string method = "")
        {

            if (observer == null)
            {
                throw new ArgumentNullException($"Method:{nameof(MethodGuard)}, Argument {nameof(observer)}: null, Called On By Method: {method}");
            }

            IsInitializedGuardClause(observer, method);
        }

        [Conditional("DEBUG")]
        private static void IsInitializedGuardClause(IMutationObserver observer, string method)
        {
            switch (method)
            {

                case nameof(IMutationObserver.Commit):
                case nameof(IMutationObserver.Push):
                case nameof(IMutationObserver.IsMutated):
                case nameof(ICollectionMutationObserver.GetMutationTypes):
                case nameof(ICollectionMutationObserver<object>.GetAddedItems):
                case nameof(ICollectionMutationObserver<object>.GetRemovedItems):
                    var type = observer.GetType();
                    if (!observer.IsInitialized) throw new InvalidOperationException($"Can't Call {method} On object of Type {type.FullName} if it is not initialized");
                    break;
                default:
                    break;
            }
        }
    }






}