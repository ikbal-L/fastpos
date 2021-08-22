﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;

namespace FastPosFrontend.ViewModels
{
    public class AppNavigationConductor<T> : Conductor<T>, IAppNavigationConductor where T : class
    {
        private BindableCollection<NavigationLookupItem> _appNavigationItems;
        private NavigationLookupItem _selectedNavigationItem;
        private AppScreen _activeScreen;

        public AppNavigationConductor()
        {
            KeepAliveScreens = new Dictionary<Type, T>();
        }

        public NavigationLookupItem SelectedNavigationItem
        {
            get => _selectedNavigationItem;
            set
            {
                Set(ref _selectedNavigationItem, value);
                NavigateToItem(value);
            }
        }

        public BindableCollection<NavigationLookupItem> AppNavigationItems
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
            AppNavigationItems = new BindableCollection<NavigationLookupItem>(result);
        }

        /// <summary>
        /// Load Single <see cref="NavigationLookupItem"/> Instances from Classes decorated by <see cref="NavigationItemAttribute"/>, and  that have no Sub NavigationItems
        /// </summary>
        /// <returns></returns>
        protected virtual List<NavigationLookupItem> LoadSingleItems()
        {
            var items = GetAppliedAttributes<NavigationItemAttribute>(
                    type => ! IsParentItem(type))
                .Where(attribute => attribute.LoadingStrategy == NavigationItemLoadingStrategy.Lazy &&
                                    string.IsNullOrEmpty(attribute.GroupName)
                                    && attribute.ParentNavigationItem == null)
                .Select(attribute => (NavigationLookupItem) attribute).ToList();
            return items;
        }

        /// <summary>
        /// Load Group <see cref="NavigationLookupItem"/> Instances from Classes decorated by <see cref="NavigationItemAttribute"/>, By Grouping them using <see cref="NavigationItemAttribute.GroupName"/>
        /// </summary>
        /// <returns></returns>
        private protected virtual List<NavigationLookupItem> LoadGroupItems()
        {
            var groupItems = 
                GetAppliedAttributes<NavigationItemAttribute>( )
                .Where(attribute => attribute.LoadingStrategy == NavigationItemLoadingStrategy.Lazy &&
                                    !string.IsNullOrEmpty(attribute.GroupName
                                    ))
                .GroupBy(attribute => attribute.GroupName).Select((grouping) =>
                    new NavigationLookupItem(grouping.Key, isGroupingItem: true)
                    {
                        SubItems = new BindableCollection<NavigationLookupItem>(
                            grouping.Select(Selector))
                    }).ToList();
            return groupItems;
        }

        private NavigationLookupItem Selector(NavigationItemAttribute attribute)
        {
            var lookupItem = (NavigationLookupItem) attribute;
            if (IsParentItem(attribute.Target))
            {
                lookupItem
                    .SubItems = new BindableCollection<NavigationLookupItem>(GetChildren(attribute.Target));
            }
            return lookupItem;
        }


        private IEnumerable<TAttribute> GetAppliedAttributes<TAttribute>(Func<Type,bool> filter = null) where TAttribute : Attribute
        {
            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => Attribute.IsDefined(x, typeof(NavigationItemAttribute)));
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
                .Where(x => Attribute.IsDefined(x, typeof(NavigationItemAttribute)))
                .Select(type =>
                    (NavigationItemAttribute) type.GetCustomAttribute(
                        typeof(NavigationItemAttribute)))
                .Any(attribute => attribute.ParentNavigationItem == target);
            return result;
        }

        private IEnumerable<NavigationLookupItem> GetChildren(Type type)
        {
             return GetAppliedAttributes<NavigationItemAttribute>()
                .Where(attribute => attribute.LoadingStrategy == NavigationItemLoadingStrategy.Lazy &&
                                    string.IsNullOrEmpty(attribute.GroupName)
                                    && attribute.ParentNavigationItem == type).Select(attribute =>(NavigationLookupItem)attribute );
        }

        private protected virtual NavigationLookupItem LoadDefaultItem()
        {
            var item = (NavigationLookupItem) Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => Attribute.IsDefined(x, typeof(NavigationItemAttribute)))
                .Select(type =>
                    (NavigationItemAttribute) type.GetCustomAttribute(
                        typeof(NavigationItemAttribute)))
                .FirstOrDefault(attribute => attribute.IsDefault);
            return item;
        }

        public virtual bool NavigateToItem(NavigationLookupItem navigationItem)
        {
            if (navigationItem == null) return false;
            
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