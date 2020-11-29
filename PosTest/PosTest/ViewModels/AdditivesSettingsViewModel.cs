using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PosTest.Helpers;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using ServiceLib.Service;

namespace PosTest.ViewModels
{
    public class AdditivesSettingsViewModel : Screen
    {
        private BindableCollection<Additive> _additives;
        private Additive _selectedAdditive;
        private IAdditiveService _additiveService;
        private List<Additive> _allAdditives;
        private int _additivePageSize;
        private bool _isEditing;
        private Additive _clipBoardAdditive;
        private Additive _additiveToMove;

        public AdditivesSettingsViewModel(IAdditiveService additiveService, int additivePageSize)
        {
            _additiveService = additiveService;
            _additivePageSize = additivePageSize;
            int additiveStatusCode = 0;
            // _allAdditives = additiveService.GetAllAdditives(ref additiveStatusCode).ToList();
            _allAdditives = GenericRest.GetAll<Additive>(UrlConfig.AdditiveUrl.GetAllAdditives, out int status).ToList();
            if (status!=200)
            {
                
            }
            _isEditing = false;
            PopulateAdditivesPage();
        }

        public BindableCollection<Additive> Additives
        {
            get => _additives;
            set { Set(ref _additives, value); }
        }

        public Additive SelectedAdditive
        {
            get => _selectedAdditive;
            set { Set(ref _selectedAdditive, value); }
        }

        public Additive ClipBoardAdditive
        {
            get => _clipBoardAdditive;
            set => Set(ref _clipBoardAdditive, value);
        }

        public Additive AdditiveToMove
        {
            get => _additiveToMove;
            set => Set(ref _additiveToMove, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => Set(ref _isEditing, value);
        }

        private void PopulateAdditivesPage()
        {
            var comparer = new Comparer<Additive>();
            var additives = new List<Additive>(_allAdditives.Where(a => a.Rank != null));
            additives.Sort(comparer);
            Additives = new BindableCollection<Additive>();
            var maxRank = (int) additives.Max(a => a.Rank);
            int numberOfPages = (maxRank / _additivePageSize) + (maxRank % _additivePageSize == 0 ? 0 : 1);
            numberOfPages = numberOfPages == 0 ? 1 : numberOfPages;
            var size = numberOfPages * _additivePageSize;

            RankedItemsCollectionHelper.LoadPagesFilled(source: additives, target: Additives, size: size);
        }


        public void AdditivesList_MouseMove(object sender, MouseEventArgs e)
        {
            var key = "Additive";

            CheckoutSettingsViewModel.MouseMoveEventHandler<Additive>(sender, e, key, nameof(Additive.Description));
        }

        public void AdditivesList_TouchDown(object sender, TouchEventArgs e)
        {
            var key = "Additive";

            CheckoutSettingsViewModel.ListTouchDownEventHandler<Additive>(sender, e, key);
        }


        public void AdditivesList_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("Additive"))
            {
                ListBox listView = sender as ListBox;
                ListBoxItem target =
                    CheckoutSettingsViewModel.FindAncestor<ListBoxItem>((DependencyObject) e.OriginalSource);

                if (target == null)
                {
                    return;
                }

                // Find the data behind the ListViewItem
                Additive targetAdditive = (Additive) listView.ItemContainerGenerator.ItemFromContainer(target);

                Additive receivedAdditive = e.Data.GetData("Additive") as Additive;

                if (receivedAdditive == null || receivedAdditive.Description == null) return;

                Console.WriteLine(targetAdditive.GetHashCode());
                Console.WriteLine(receivedAdditive.GetHashCode());

                SelectedAdditive = targetAdditive;
                //TODO Fix locating the index of empty additive
                var indexOfReceived = Additives.IndexOf(receivedAdditive);
                var indexOfTarget = Additives.IndexOf(targetAdditive);
                if (SelectedAdditive.Equals(receivedAdditive))
                {
                    return;
                }

                PutAdditiveInCellOf(targetAdditive, receivedAdditive);
                SelectedAdditive = targetAdditive;
            }
        }

        private void PutAdditiveInCellOf(Additive targetAdditive, Additive receivedAdditive)
        {
            var targetRank = targetAdditive.Rank;
            var receivedRank = receivedAdditive.Rank;
            var targetIndex = Additives.IndexOf(targetAdditive);
            var receivedIndex = Additives.IndexOf(receivedAdditive);
            targetAdditive.Rank = receivedRank;
            receivedAdditive.Rank = targetRank;
            
            Additives[targetIndex] = receivedAdditive;
            Additives[receivedIndex] = targetAdditive;
            try
            {
                if (receivedAdditive.Id != null)
                {
                    int receivedAdditiveStatusCode=_additiveService.UpdateAdditive(receivedAdditive);
                    if (receivedAdditiveStatusCode!=200)
                    {
                        var message = "Failed To update Additive {Additive}  {ERRORCODE}, attempting to resave after {0} milliseconds ";
                        var args = new object[] { receivedAdditive.Description, receivedAdditiveStatusCode, 300 };
                        ServiceHelper.HandleStatusCodeErrors(receivedAdditiveStatusCode, message, args);
                    }
                }
                if (targetAdditive.Id != null)
                {
                    int targetAdditiveStatusCode=_additiveService.UpdateAdditive(targetAdditive);
                    if (targetAdditiveStatusCode!=200)
                    {
                        {
                            var message = "Failed To update Additive {Additive}  {ERRORCODE}, attempting to resave after {0} milliseconds ";
                            var args = new object[] { targetAdditive.Description, targetAdditiveStatusCode, 300 };
                            ServiceHelper.HandleStatusCodeErrors(targetAdditiveStatusCode, message, args);
                        }
                    }
                }
            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Problem connecting to server");
            }
            catch (Exception e)
            {
#if DEBUG
                throw;
#endif
                NLog.LogManager.GetCurrentClassLogger().Error(e);
            }
        }

        public void SaveAdditive()
        {
            IsEditing = false;
            try
            {
                int statusCode = 0;
                if (SelectedAdditive.Id == null)
                {

                    statusCode = _additiveService.SaveAdditive(SelectedAdditive, out long id, out IEnumerable<string> errors);
                    SelectedAdditive.Id = id;
                }
                else
                {
                    statusCode = _additiveService.UpdateAdditive(SelectedAdditive);
                }

                if (statusCode != 200 && statusCode!=201)
                {
                    var message =
                        "Failed To update Additive {Additive}  {ERRORCODE}, attempting to resave after {0} milliseconds ";
                    var args = new object[] {SelectedAdditive.Description, statusCode, 300};
                    ServiceHelper.HandleStatusCodeErrors(statusCode, message, args);
                }
            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Problem connecting to server ");
            }
            catch (Exception e)
            {
#if DEBUG
                throw;
#endif
                NLog.LogManager.GetCurrentClassLogger().Error(e);
            }
        }

        public void CopyAdditive()
        {
            if (SelectedAdditive == null || SelectedAdditive.Id == null)
            {
                ToastNotification.Notify("Select a Valid Additive to copy", 1);
                return;
            }

            ClipBoardAdditive = SelectedAdditive;
        }

        public void PasteAdditive()
        {
            if (ClipBoardAdditive == null)
            {
                ToastNotification.Notify("Copy an additive first");
                return;
            }

            if (SelectedAdditive == null || SelectedAdditive.Rank == null)
            {
                ToastNotification.Notify("Select a zone to copy in first");
                return;
            }

            if (ClipBoardAdditive.Equals(SelectedAdditive))
            {
                ToastNotification.Notify("You chose the same additive");
                return;
            }

            int rank = (int) SelectedAdditive.Rank;
            var additive = new Additive(ClipBoardAdditive);
            additive.Rank = rank;
            additive.Id = null;
            try
            {
                var statusCode = _additiveService.SaveAdditive(additive, out var id, out IEnumerable<string> errors);
                if (statusCode == 200)
                {
                    additive.Id = id;
                }
                else
                {
                    var message =
                        "Failed To save Additive {Additive}  {ERRORCODE}, attempting to resave after {0} milliseconds ";
                    var args = new object[] {additive.Description, statusCode, 300};
                    ServiceHelper.HandleStatusCodeErrors(statusCode, message, args);
                }
            }
            catch (AggregateException)
            {
                ToastNotification.Notify("Problem connecting to server");
            }
            catch (Exception e)
            {
#if DEBUG
                throw;
#endif
                NLog.LogManager.GetCurrentClassLogger().Error(e);
            }

            Additives[rank - 1] = additive;
        }

        public void MoveAdditive()
        {
            if (AdditiveToMove == null)
            {
                AdditiveToMove = SelectedAdditive;
                return;
            }

            if (AdditiveToMove == SelectedAdditive)
            {
                ToastNotification.Notify("You selected the same additive");
                AdditiveToMove = null;
                SelectedAdditive = null;
                return;
            }

            PutAdditiveInCellOf(SelectedAdditive, AdditiveToMove);
            AdditiveToMove = null;
        }

        public void DeleteAdditive()
        {
            if (SelectedAdditive == null||SelectedAdditive.Id==null)
            {
                ToastNotification.Notify("Select an Additive to delete first ");
                return;
            }

            var selectedAdditiveId = (long)SelectedAdditive.Id;
            if (_additiveService.DeleteAdditive(selectedAdditiveId)==200)
            {
                Additives.Remove(SelectedAdditive);
                ToastNotification.Notify("Additive was deleted successfully",1);

            }
        }
    }
}