using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using FastPosFrontend.ViewModels.Settings;

namespace FastPosFrontend.ViewModels
{
    public class MainViewModel : AppNavigationConductor<object>
    {
        private const string WindowTitleDefault = "FAST POS";

        private string _windowTitle = WindowTitleDefault;
        private string _buttonStr = "Login";
        private bool _isLoggedIn = false;
        private bool _isDbServerOn = false;
        private bool _isBackendServerOn = false;
        private AppScreen _activeScreen;


        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                Set(ref _isLoggedIn, value);
                if (value)
                {
                    LoadNavigationItems();
                }
            }
        }

        public bool IsDbServerOn
        {
            get => _isDbServerOn;
            set => Set(ref _isDbServerOn, value);
        }

        public bool IsBackendServerOn
        {
            get => _isBackendServerOn;
            set => Set(ref _isBackendServerOn, value);
        }

        public AppScreen ActiveScreen
        {
            get => _activeScreen;
            set => Set(ref _activeScreen, value);
        }


        public String ButtonStr
        {
            get => _buttonStr;
            set
            {
                _buttonStr = value;
                NotifyOfPropertyChange(() => ButtonStr);
            }
        }


        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                NotifyOfPropertyChange(() => WindowTitle);
            }
        }

        protected override void OnActivate()
        {
            //var splashScreen = new SplashScreenView();
            //splashScreen.Show();
            //System.Threading.Thread.Sleep(7000);
            //Task.Factory.StartNew(() =>
            //{
            //    
            //    this.Dispatcher.Invoke(() =>
            //    {
            //        DisplayRootViewFor<MainViewModel>();
            //        splashScreen.Close();
            //    });
            //});
            LoginViewModel toActivateViewModel = new LoginViewModel {Parent = this};
            //UserSettingsViewModel userSettingsViewModel = new UserSettingsViewModel() { Parent = this };
            //ActivateItem(userSettingsViewModel);
            ActivateItem(toActivateViewModel);

            //splashScreen.Close();
        }

        public void showLogin()
        {
            ButtonStr = "Logged in";
            ActivateItem(new LoginViewModel());
        }
    }

    public class AppNavigationConductor<T> : Conductor<T>, IAppNavigationConductor where T : class
    {
        private BindableCollection<AppNavigationLookupItem> _appNavigationItems;

        public AppNavigationConductor()
        {
        }

        public BindableCollection<AppNavigationLookupItem> AppNavigationItems
        {
            get => _appNavigationItems;
            set => Set(ref _appNavigationItems, value);
        }

        public virtual void LoadNavigationItems()
        {

            var items = LoadSingleItems();
            var groupItems = LoadGroupItems();
            items.AddRange(groupItems);
            AppNavigationItems = new BindableCollection<AppNavigationLookupItem>(items);
        }

        /// <summary>
        /// Load Single <see cref="AppNavigationLookupItem"/> Instances from Classes decorated by <see cref="NavigationItemConfigurationAttribute"/>, and  that have no Sub NavigationItems
        /// </summary>
        /// <returns></returns>
        protected virtual List<AppNavigationLookupItem> LoadSingleItems()
        {
            var items = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => Attribute.IsDefined(x, typeof(NavigationItemConfigurationAttribute)))
                .Select(type =>
                    (NavigationItemConfigurationAttribute)type.GetCustomAttribute(
                        typeof(NavigationItemConfigurationAttribute)))
                .Where(attribute => attribute.LoadingStrategy == NavigationItemLoadingStrategy.Lazy && string.IsNullOrEmpty(attribute.GroupName))
                .Select(attribute => (AppNavigationLookupItem)attribute).ToList();
            return items;
        }

        /// <summary>
        /// Load Group <see cref="AppNavigationLookupItem"/> Instances from Classes decorated by <see cref="NavigationItemConfigurationAttribute"/>, By Grouping them using <see cref="NavigationItemConfigurationAttribute.GroupName"/>
        /// </summary>
        /// <returns></returns>
        private protected virtual List<AppNavigationLookupItem> LoadGroupItems()
        {
            var groupItems = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x => Attribute.IsDefined(x, typeof(NavigationItemConfigurationAttribute)))
                .Select(type =>
                    (NavigationItemConfigurationAttribute)type.GetCustomAttribute(
                        typeof(NavigationItemConfigurationAttribute)))
                .Where(attribute => attribute.LoadingStrategy == NavigationItemLoadingStrategy.Lazy &&
                                    !string.IsNullOrEmpty(attribute.GroupName))
                .GroupBy(attribute => attribute.GroupName).Select((grouping) =>
                    new AppNavigationLookupItem(grouping.Key)
                    {
                        SubItems = new BindableCollection<AppNavigationLookupItem>(
                            grouping.Select(attribute => (AppNavigationLookupItem)attribute))
                    }).ToList();
            return groupItems;
        }

        public virtual void NavigateToItem(IAppNavigationItem navigationItem)
        {
        }
    }

    public interface IAppNavigationConductor
    {
    }
}