using Caliburn.Micro;
using PosTest.Helpers;
using PosTest.Views.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PosTest.ViewModels.Settings
{
    public class SettingsViewModel : Screen
    {
        private ObservableCollection<SettingsItemBase> _SettingItems;
        public ObservableCollection<SettingsItemBase> SettingItems
        {
            get { return _SettingItems; }
            set { _SettingItems = value; }
        }
        private SettingsItemBase _SelectedItem;

        public SettingsItemBase SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }
        public SettingsViewModel()
        {
            SettingItems = new ObservableCollection<SettingsItemBase>(Assembly.GetExecutingAssembly().GetTypes().
                Where(x => x.IsSubclassOf(typeof(SettingsItemBase))).ToList()
                .Select(t=> (SettingsItemBase)Activator.CreateInstance(t)).OrderBy(x=>x.Index).ToList());
            SelectedItem = SettingItems?.FirstOrDefault();
        }
        public void  Close() {
            LoginViewModel loginvm = new LoginViewModel();
            loginvm.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(loginvm);
        }
    }
}
