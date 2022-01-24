using System;
using System.Collections.Generic;
using System.Linq;
using FastPosFrontend.Configurations;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using ServiceInterface.ExtentionsMethod;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels.Settings
{

    [NavigationItem(title: "General Settings", typeof(GeneralSettingsViewModel), "", groupName: "Settings", isQuickNavigationEnabled: true)]
    [PreAuthorize("Modify_Local_Settings")]
    public class GeneralSettingsViewModel : AppScreen, ISettingsController
    {
        private GeneralSettings settings;
        private readonly GeneralSettings _source;

        public GeneralSettings Settings { get => settings; set => Set(ref settings , value); }


        public GeneralSettingsViewModel()
        {
            _source = ConfigurationManager.Get<PosConfig>().General;
            CheckTableCount();
            Settings = _source.Clone();
        }

        private void CheckTableCount()
        {
            var tables = StateManager.GetAll<Table>();
            if (!tables.Any())
            {
                _source.TableCount = 0;
            }
            else
            {
                if (tables.Count != _source.TableCount)
                {
                    _source.TableCount = tables.Count;
                }
            }
        }

        public void Save()
        {
            ConfigurationManager.Get<PosConfig>().General = Settings;
            Settings.RequestSave();
            ManageTables(_source.TableCount, settings.TableCount);
        }

        public void Cancel()
        {
            Settings = _source.Clone();
        }


        public void ManageTables(int oldValue,int value)
        {
            if (value<0)
            {
                ToastNotification.Notify("Enter a valid number"); return;
            }

            var d = value - oldValue;

            if (d < 0) DeleteTables(Math.Abs(d));
           
            if (d > 0) CreateTables(Math.Abs(d));
            
        }


        public bool DeleteTables(int limit)
        {
            var ids = StateManager.GetAll<Table>().Where(t => t.Id != null).OrderByDescending(table => table.Number).Take(limit).Select((table, i) => table.Id.Value);
            return StateManager.Delete<Table, long>(ids);
        }

        public bool CreateTables(int limit)
        {
            var tables = StateManager.GetAll<Table>();
            var baseNumber = tables.Any() ? tables.Max(table => table.Number) : 0;
            var  newTables = new List<Table>(limit);
            for (int i = 1; i <= limit; i++)
            {

                newTables.Add(new Table() { Number = baseNumber + i });
            }

            var result = StateManager.SaveAll(newTables);
            return result;
        }


        public event EventHandler<SettingsUpdatedEventArgs> SettingsUpdated;
        public override void BeforeNavigateAway()
        {
            SettingsUpdated?.Invoke(this, new SettingsUpdatedEventArgs(Settings.CategoryPageSize));
            ProductLayout.Instance.OnSettingsUpdated(this, new SettingsUpdatedEventArgs());
        }
    }
}
