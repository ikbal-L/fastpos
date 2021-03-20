using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;

namespace FastPosFrontend.ViewModels.Settings
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
        private bool _leftBarVisibilty;

        public bool LeftBarVisibilty
        {
            get { return _leftBarVisibilty; }
            set { _leftBarVisibilty = value;
                NotifyOfPropertyChange(nameof(LeftBarVisibilty));
            }
        }
        private Screen ParentVeiwModel { get; set; }
        public SettingsViewModel(Screen parentVeiwModel)
        {
            ParentVeiwModel = parentVeiwModel;
               SettingItems = new ObservableCollection<SettingsItemBase>(Assembly.GetExecutingAssembly().GetTypes().
                Where(x => x.IsSubclassOf(typeof(SettingsItemBase))).ToList()
                .Select(t=> (SettingsItemBase)Activator.CreateInstance(t)).OrderBy(x=>x.Index).ToList());
            SelectedItem = SettingItems?.FirstOrDefault();
        }
        public void  Close() {
            (this.Parent as Conductor<object>).ActivateItem(ParentVeiwModel);
        }
      public void  HideenLeftBar(SettingsItemBase obj) {
            LeftBarVisibilty = false;
            SelectedItem = obj;
        }
    }
}
