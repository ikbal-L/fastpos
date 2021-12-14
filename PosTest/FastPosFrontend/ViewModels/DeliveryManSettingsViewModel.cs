using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using FastPosFrontend.Events;
using FastPosFrontend.Helpers;
using FastPosFrontend.ViewModels.Settings;
using FastPosFrontend.Views;
using FastPosFrontend.Views.Settings;
using ServiceInterface.Model;
using ServiceLib.Service.StateManager;
using FastPosFrontend.Navigation;
namespace FastPosFrontend.ViewModels
{
    [NavigationItem("Deliveryman Settings", typeof(DeliveryManSettingsViewModel), "", groupName: "Settings", isQuickNavigationEnabled: true)]
    [PreAuthorize("Create_Deliveryman|Update_Deliveryman|DeleteDeliveryman")]
    public class DeliveryManSettingsViewModel : LazyScreen,ISettingsController
    {
        private ObservableCollection<Deliveryman> _Deliverymans;

        public ObservableCollection<Deliveryman> Deliverymans
        {
            get { return _Deliverymans; }
            set {
                _Deliverymans = value;
                NotifyOfPropertyChange((nameof(Deliverymans)));
            }
        }
        private Deliveryman _SelectedDeliveryMan;

        public Deliveryman SelectedDeliveryMan
        {
            get { return _SelectedDeliveryMan; }
            set {
                _SelectedDeliveryMan = value;
                NotifyOfPropertyChange(nameof(SelectedDeliveryMan));
                NotifyOfPropertyChange(nameof(DeleteVisibility));
                NotifyOfPropertyChange(nameof(EditVisibility));
            }
        }
        private UserControl _DailogContent;

        public UserControl DailogContent
        {
            get { return _DailogContent; }
            set { _DailogContent = value;
                NotifyOfPropertyChange(nameof(DailogContent));
            }
        }
        private AddEditDeliveryManViewModel _AddEditDeliveryManViewModel;
        private AddEditDeliveryManView _addEditDeliveryManView;
        public DeliveryManSettingsViewModel() : base()
        {

            //this.Title = "Delivery Mans";
            //this.Content = new DeliveryManSettingsView() { DataContext = this };

            
        }
        public bool DeleteVisibility { get => SelectedDeliveryMan != null; }
        public bool EditVisibility { get => SelectedDeliveryMan != null; }
        public void AddWiaterAction() {
            _AddEditDeliveryManViewModel.NewDeliveryMan();
            _AddEditDeliveryManViewModel.ChangeDailogSatate(true);
        }
        public void EditWiaterAction()
        {

            if (SelectedDeliveryMan == null)
            {
                return;
            }
            _AddEditDeliveryManViewModel.ChangeDeliveryMan(SelectedDeliveryMan);
            _AddEditDeliveryManViewModel.ChangeDailogSatate(true);
        }
        public void DeleteWiaterAction() {
            if (SelectedDeliveryMan == null)
            {
                ToastNotification.Notify("Selected DeliveryMan First", NotificationType.Warning);
                return;
            }

            var main = Parent as MainViewModel;
            main?.OpenDialog(
                DefaultDialog
                    .New("Are you sure you want perform this action?")
                    .Title("Delete Deliveryman")
                    .Ok(o =>
                    {
                        DeleteDeliveryman();
                        main.CloseDialog();
                    })
                    .Cancel(o =>
                    {
                        main.CloseDialog();
                    }));

            //var response = ModalDialogBox.YesNo("Are you sure you want to Delete this Deliveryman?", "Cancel Order").Show();
            //if (response)
            //{
            //    DeleteDeliveryman();
            //}
        }

        private void DeleteDeliveryman()
        {
            if (StateManager.Delete<Deliveryman>(SelectedDeliveryMan))
            {

                Deliverymans.Remove(SelectedDeliveryMan);
                SelectedDeliveryMan = null;
                DailogContent = _addEditDeliveryManView;
                ToastNotification.Notify("Delete  Success", NotificationType.Success);
            }
            else
            {
                ToastNotification.Notify("Delete  Fail");
            }
        }

        protected override void Setup()
        {
            _data = new NotifyAllTasksCompletion(StateManager.GetAsync<Deliveryman>());
        }

        public override void Initialize()
        {
            Deliverymans = new ObservableCollection<Deliveryman>(StateManager.GetAll<Deliveryman>());
            _AddEditDeliveryManViewModel = new AddEditDeliveryManViewModel(this);
            _addEditDeliveryManView = new AddEditDeliveryManView() { DataContext = _AddEditDeliveryManViewModel };
            DailogContent = _addEditDeliveryManView;
        }

        public event EventHandler<SettingsUpdatedEventArgs> SettingsUpdated;

        public void RaiseSettingsUpdated()
        {
            SettingsUpdated?.Invoke(this, new SettingsUpdatedEventArgs(Deliverymans));
        }

        public override void BeforeNavigateAway()
        {
            RaiseSettingsUpdated();
        }
    }
}
