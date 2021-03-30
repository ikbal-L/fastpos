using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using PosTest.Helpers;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceInterface.StaticValues;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace PosTest.ViewModels.SubViewModel
{
    
    public class EditProductViewModel : PropertyChangedBase, INotifyDataErrorInfo
    {
        private Product _product;
        private Product _source;

        private readonly Dictionary<string, ICollection<string>> _validationErrors =
            new Dictionary<string, ICollection<string>>();

        private bool _isSaveEnabled;
        private string _name;
        private decimal _price;
        private string _unit;
        private IAdditiveService _additiveService;
        private int _additivesPageSize;
        private bool _isPlatter;

        public EditProductViewModel(ref Product sourceProduct,
            int additivesPageSize = 30)
        {
            
            this._source = sourceProduct;
            IsPlatter = sourceProduct.IsPlatter;


            this._additiveService = new AdditiveService();
            this._additivesPageSize = additivesPageSize;
            PopulateAdditivesPage();

            this._source = sourceProduct;
            
            CloneFromSource();

            
            CheckAdditiveCommand = new DelegateCommandBase(CheckAdditive, CanCheckAdditive);
            IsSaveEnabled = !HasErrors;

        }
        public void CloneFromSource()
        {
            if (_source != null)
            {
                var product = new Product()
                {
                    Name = _source.Name,
                    Description = _source.Description,
                    Background = _source.Background,
                    BackgroundColor = _source.BackgroundColor,
                    Price = _source.Price,
                    Unit = _source.Unit,
                    IsPlatter = _source.IsPlatter
                };

                

                if (Source.Id != null)
                {
                    if (Source.IsPlatter)
                    {
                        product.IsPlatter = Source.IsPlatter;
                        product.IdAdditives = Source.IdAdditives;
                        product.Additives = Source.Additives;
                    }
                }
                else
                {
                    product.IsPlatter = true;
                    product.IdAdditives = new List<long>();
                    product.Additives = new BindableCollection<Additive>();
                }
                IsPlatter = product.IsPlatter;

                //Set(ref _name, _source.Name);
                //Set(ref _price, _source.Price);
                //Set(ref _unit, _source.Unit);
                Product = product;
                Name = _source.Name;
                Price = _source.Price;
                Unit = _source.Unit;
                //NotifyOfPropertyChange(() => this.Product);
                //NotifyOfPropertyChange(() => this.Name);
                //NotifyOfPropertyChange(() => this.Price);
                //NotifyOfPropertyChange(() => this.Unit);

                return ;
            }

            Product = new Product();
        }

        public string Name
        {
            get => _name;
            set
            {
                Set(ref _name, value);
                Product.Name = _name;
                ValidateModelProperty(Product, Product.Name, nameof(Product.Name));
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                Set(ref _price, value);
                Product.Price = _price;
                ValidateModelProperty(Product, Product.Price, nameof(Product.Price));
            }
        }

        public string Unit
        {
            get => _unit;
            set
            {
                Set(ref _unit, value);
                Product.Unit = _unit;
                ValidateModelProperty(Product, Product.Unit, nameof(Product.Unit));
            }
        }

        public BindableCollection<Additive> Additives { get; set; }

        public bool IsPlatter
        {
            get => _isPlatter;
            set
            {
                Set(ref _isPlatter, value);
                
                if (Product!=null)
                {
                    Product.IsPlatter = value;
                }

                (CheckAdditiveCommand as DelegateCommandBase)?.RaiseCanExecuteChanged();
            }
        }

        public Product Product
        {
            get => _product;
            set
            {
                _product = value;
                Set(ref _product, value);
                ValidateModelProperty(Product, Product.Name, nameof(Product.Name));
                ValidateModelProperty(Product, Product.Price, nameof(Product.Price));
                ValidateModelProperty(Product, Product.Unit, nameof(Product.Unit));
            }
        }

        public ICommand CheckAdditiveCommand { get; set; }
        private void PopulateAdditivesPage()
        {
            var comparer = new Comparer<Additive>();
            int StatusCode = 0;
            // var allAdditives = _additiveService.GetAllAdditives(ref StatusCode).ToList();
            var (status,allAdditives) = GenericRest.GetAll<Additive>(UrlConfig.AdditiveUrl.GetAllAdditives);
            if (status!=200)
            {
                
            }
            var filteredAdditives = new List<Additive>(allAdditives.Where(a => a.Rank != null));
            filteredAdditives.Sort(comparer);
            Additives = new BindableCollection<Additive>();
            var maxRank = (int) filteredAdditives.Max(a => a.Rank);
            int numberOfPages = (maxRank / _additivesPageSize) + (maxRank % _additivesPageSize == 0 ? 0 : 1);
            numberOfPages = numberOfPages == 0 ? 1 : numberOfPages;
            var size = numberOfPages * _additivesPageSize;

            RankedItemsCollectionHelper.LoadPagesNotFilled(source: filteredAdditives, target: Additives, size: _additivesPageSize);
        }

        public void SaveProduct()
        {
            Clone(source: ref _product, target: ref this._source);
            StateManager.Save<Product>(this._source);
        }

        private bool CanCheckAdditive(object arg)
        {
            return _product.IsPlatter;
        }
        public void CheckAdditive(object sender)
        {
            ToggleButton toggleButton = sender as ToggleButton;
            Additive source = toggleButton.DataContext as Additive;
            if (source==null)
            {
                toggleButton.IsChecked = false;
                return;
            }

            if (Product.Additives.Contains(source) && !(bool) toggleButton?.IsChecked)
            {
                Product.IdAdditives.Remove((long)source.Id);
                Product.Additives.Remove(source);
                return;
            }

            if (!Product.Additives.Contains(source) && (bool) toggleButton?.IsChecked)
            {
                Product.IdAdditives.Add((long)source.Id);
                Product.Additives.Add(source);
                return;
            }
        }


        private childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                {
                    return (childItem) child;
                }
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }


        public static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            do
            {
                if (current is T)
                {
                    return (T) current;
                }

                current = VisualTreeHelper.GetParent(current);
            } while (current != null);

            return null;
        }

        public void Cancel()
        {
            //this.Product = new Product();
            //this.Name = null;
            //this.Price = 0;
            //this.Unit = null;
            //Product = CloneFromSource();
            

        }

        private static void Clone(ref Product source, ref Product target)
        {
            if (source == null) return;
            target.Name = source.Name;
            target.Price = source.Price;
            target.Background = source.Background;
            target.Unit = source.Unit;
            target.IsPlatter = source.IsPlatter;
            if (source.IsPlatter)
            {
                target.IdAdditives = source.IdAdditives;
                target.Additives = source.Additives; 
            }

        }

        public Product Source
        {
            get { return _source; }
            set
            {
                Set(ref _source, value);
                CloneFromSource();
                NotifyOfPropertyChange(() => this.Product);
            }
        }


        private void RaiseErrorsChanged(string propertyName)
        {
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName)
                || !_validationErrors.ContainsKey(propertyName))
                return null;

            return _validationErrors[propertyName];
        }

        public bool HasErrors => _validationErrors.Any();

        public bool IsSaveEnabled
        {
            get => _isSaveEnabled;
            set => Set(ref _isSaveEnabled, value);
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void ValidateModelProperty(object instance, object value, string propertyName)
        {
            if (_validationErrors.ContainsKey(propertyName))
                _validationErrors.Remove(propertyName);

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext =
                new ValidationContext(instance, null, null) {MemberName = propertyName};
            if (!Validator.TryValidateProperty(value, validationContext, validationResults))
            {
                _validationErrors.Add(propertyName, new List<string>());
                foreach (ValidationResult validationResult in validationResults)
                {
                    _validationErrors[propertyName].Add(validationResult.ErrorMessage);
                }
            }

            IsSaveEnabled = _validationErrors.Count == 0;
            NotifyOfPropertyChange(() => IsSaveEnabled);
            RaiseErrorsChanged(propertyName);
        }
    }
}