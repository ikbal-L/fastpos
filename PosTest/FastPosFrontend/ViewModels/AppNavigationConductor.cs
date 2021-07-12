using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;

namespace FastPosFrontend.ViewModels
{
    public class AppNavigationConductor<T> : Conductor<T>, IAppNavigationConductor where T : class
    {
        private BindableCollection<AppNavigationLookupItem> _appNavigationItems;
        private AppNavigationLookupItem _selectedNavigationItem;
        private AppScreen _activeScreen;

        public AppNavigationConductor()
        {
            KeepAliveScreens = new Dictionary<Type, T>();
        }

        public AppNavigationLookupItem SelectedNavigationItem
        {
            get => _selectedNavigationItem;
            set
            {
                Set(ref _selectedNavigationItem, value);
                NavigateToItem(value);
            }
        }

        public BindableCollection<AppNavigationLookupItem> AppNavigationItems
        {
            get => _appNavigationItems;
            set => Set(ref _appNavigationItems, value);
        }

        public AppScreen ActiveScreen
        {
            get => _activeScreen;
            set => Set(ref _activeScreen, value);
        }

        public Dictionary<Type, T> KeepAliveScreens { get; protected set; }

        public virtual void LoadNavigationItems()
        {
            var items = LoadSingleItems();
            var groupItems = LoadGroupItems();
            items.AddRange(groupItems);
            var result = items.AssignIndexes();
            AppNavigationItems = new BindableCollection<AppNavigationLookupItem>(result);
        }

        /// <summary>
        /// Load Single <see cref="AppNavigationLookupItem"/> Instances from Classes decorated by <see cref="NavigationItemConfigurationAttribute"/>, and  that have no Sub NavigationItems
        /// </summary>
        /// <returns></returns>
        protected virtual List<AppNavigationLookupItem> LoadSingleItems()
        {
            var items = GetAppliedAttributes<NavigationItemConfigurationAttribute>(
                    type => ! IsParentItem(type))
                .Where(attribute => attribute.LoadingStrategy == NavigationItemLoadingStrategy.Lazy &&
                                    string.IsNullOrEmpty(attribute.GroupName)
                                    && attribute.ParentNavigationItem == null)
                .Select(attribute => (AppNavigationLookupItem) attribute).ToList();
            return items;
        }

        /// <summary>
        /// Load Group <see cref="AppNavigationLookupItem"/> Instances from Classes decorated by <see cref="NavigationItemConfigurationAttribute"/>, By Grouping them using <see cref="NavigationItemConfigurationAttribute.GroupName"/>
        /// </summary>
        /// <returns></returns>
        private protected virtual List<AppNavigationLookupItem> LoadGroupItems()
        {
            var groupItems = 
                GetAppliedAttributes<NavigationItemConfigurationAttribute>( )
                .Where(attribute => attribute.LoadingStrategy == NavigationItemLoadingStrategy.Lazy &&
                                    !string.IsNullOrEmpty(attribute.GroupName
                                    ))
                .GroupBy(attribute => attribute.GroupName).Select((grouping) =>
                    new AppNavigationLookupItem(grouping.Key, isGroupingItem: true)
                    {
                        SubItems = new BindableCollection<AppNavigationLookupItem>(
                            grouping.Select(Selector))
                    }).ToList();
            return groupItems;
        }

        private AppNavigationLookupItem Selector(NavigationItemConfigurationAttribute attribute)
        {
            var lookupItem = (AppNavigationLookupItem) attribute;
            if (IsParentItem(attribute.Target))
            {
                lookupItem
                    .SubItems = new BindableCollection<AppNavigationLookupItem>(GetChildren(attribute.Target));
            }
            return lookupItem;
        }


        private IEnumerable<TAttribute> GetAppliedAttributes<TAttribute>(Func<Type,bool> filter = null) where TAttribute : Attribute
        {
            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => Attribute.IsDefined(x, typeof(NavigationItemConfigurationAttribute)));
            if (filter!= null)
            {
                types = types.Where(filter).ToArray();
            }

            return types
                .Select(type =>
                    (TAttribute) type.GetCustomAttribute(
                        typeof(TAttribute)));
        }

        private bool IsParentItem(Type target)
        {
            var result = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => Attribute.IsDefined(x, typeof(NavigationItemConfigurationAttribute)))
                .Select(type =>
                    (NavigationItemConfigurationAttribute) type.GetCustomAttribute(
                        typeof(NavigationItemConfigurationAttribute)))
                .Any(attribute => attribute.ParentNavigationItem == target);
            return result;
        }

        private IEnumerable<AppNavigationLookupItem> GetChildren(Type type)
        {
             return GetAppliedAttributes<NavigationItemConfigurationAttribute>()
                .Where(attribute => attribute.LoadingStrategy == NavigationItemLoadingStrategy.Lazy &&
                                    string.IsNullOrEmpty(attribute.GroupName)
                                    && attribute.ParentNavigationItem == type).Select(attribute =>(AppNavigationLookupItem)attribute );
        }

        private protected virtual AppNavigationLookupItem LoadDefaultItem()
        {
            var item = (AppNavigationLookupItem) Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => Attribute.IsDefined(x, typeof(NavigationItemConfigurationAttribute)))
                .Select(type =>
                    (NavigationItemConfigurationAttribute) type.GetCustomAttribute(
                        typeof(NavigationItemConfigurationAttribute)))
                .FirstOrDefault(attribute => attribute.IsDefault);
            return item;
        }

        public virtual bool NavigateToItem(AppNavigationLookupItem navigationItem)
        {
            if (navigationItem.Target == null || !navigationItem.Target.IsSubclassOf(typeof(Screen))) return false;
            if (navigationItem.KeepAlive)
            {
                if (KeepAliveScreens.ContainsKey(navigationItem.Target))
                {
                    var screen = KeepAliveScreens[navigationItem.Target];
                    return ActivateScreenByType(screen);
                }

                var keepAliveScreenInstance = (T) Activator.CreateInstance(navigationItem.Target);
                KeepAliveScreens.Add(navigationItem.Target, keepAliveScreenInstance);
                ActivateScreenByType(keepAliveScreenInstance);
                return ActivateScreenByType(keepAliveScreenInstance);
            }

            var screenInstance = (T) Activator.CreateInstance(navigationItem.Target);
            SetSettingsListener(screenInstance);
            return ActivateScreenByType(screenInstance);
        }

        private bool ActivateScreenByType(T screenInstance)
        {
            if (screenInstance is AppScreen screen)
            {
                screen.Parent = this;
                if (ActiveScreen != null && !ActiveScreen.CanNavigate(screen.GetType())) return false;
                
                ActiveScreen?.BeforeNavigateAway();
                ActiveScreen = screen;
                if (screenInstance is LazyScreen lazyScreen)
                {
                    lazyScreen.ActivateLoadingScreen();
                }
                else
                {
                    ActivateItem(screenInstance);
                }

                return true;
            }

            return false;
        }

        public virtual void OnLogin()
        {
            var defaultItem = LoadDefaultItem();
            SelectedNavigationItem = defaultItem;
            NavigateToItem(defaultItem);
        }

        public void  SetSettingsListener(object obj)
        {
            if (obj is ISettingsController settingsController)
            {
                foreach (var (_,instance) in KeepAliveScreens.Select(x => (x.Key, x.Value)))
                {
                    if (instance is ISettingsListener listener && listener.SettingsControllers.Contains(settingsController.GetType()) )
                    {
                        settingsController.SettingsUpdated += listener.OnSettingsUpdated;
                    }
                }
            }
        }

        
    }
}