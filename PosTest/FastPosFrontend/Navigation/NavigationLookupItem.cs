using System;
using Caliburn.Micro;

namespace FastPosFrontend.Navigation
{
    public class NavigationLookupItem : PropertyChangedBase,INavigationItem
    {
        private string _title;
        private Type _target;
        private IObservableCollection<NavigationLookupItem> _subItems;
        private int _index;


        public NavigationLookupItem(string title, Type target = null, bool keepAlive = false, bool isDefault = false,
            bool isGroupingItem = false)
        {
            _title = title;
            _target = target;
            KeepAlive = keepAlive;
            IsDefault = isDefault;
            IsGroupingItem = isGroupingItem;
        }

        public string Title
        {
            get => _title;
            set => Set(ref _title, value);
        }

        public Type Target
        {
            get => _target;
            set => Set(ref _target, value);
        }

        public bool KeepAlive { get; set; }
        public bool IsDefault { get; set; }

        public bool IsGroupingItem { get; set; }

        public IObservableCollection<NavigationLookupItem> SubItems
        {
            get => _subItems;
            set => Set(ref _subItems, value);
        }

        public int Index
        {
            get => _index;
            set => Set(ref _index, value);
        }

        public string GroupName => throw new NotImplementedException();

        public NavigationItemLoadingStrategy LoadingStrategy => throw new NotImplementedException();

        public Type ParentNavigationItem => throw new NotImplementedException();

        public static explicit operator NavigationLookupItem(NavigationItemAttribute configuration) =>
            new NavigationLookupItem(configuration.Title, configuration.Target, configuration.KeepAlive,
                isDefault: configuration.IsDefault);
    }
}