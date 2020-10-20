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
using System.Windows.Media;
using Caliburn.Micro;
using PosTest.Helpers;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Service;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace PosTest.ViewModels.SubViewModel
{
    [Export("EPVM", typeof(EditProductViewModel))]
    public class EditProductViewModel : PropertyChangedBase, INotifyDataErrorInfo
    {
        private Product _product;
        private IProductService _productService;
        private Product _source;

        private readonly Dictionary<string, ICollection<string>> _validationErrors =
            new Dictionary<string, ICollection<string>>();

        private bool _isSaveEnabled;
        private string _name;
        private decimal _price;
        private string _unit;
        private IAdditiveService _additiveService;
        private int _additivesPageSize;

        public EditProductViewModel(ref Product sourceProduct, IProductService productService,
            int additivesPageSize = 30)
        {
            _product = new Product();
            Clone(source: ref sourceProduct, target: ref _product);
            this._source = sourceProduct;

            this._productService = productService;

            this._additiveService = new AdditiveService();
            this._additivesPageSize = additivesPageSize;
            PopulateAdditivesPage();

            this._source = sourceProduct;
            _product = new Product();
            _product = CloneFromSource();

            IsSaveEnabled = true;
        }

        public Product CloneFromSource()
        {
            if (_source != null)
            {
                var product = new Platter()
                {
                    Name = _source.Name,
                    Description = _source.Description,
                    Background = _source.Background,
                    BackgroundColor = _source.BackgroundColor,
                    Price = _source.Price,
                    Unit = _source.Unit
                };
                if (Source is Platter)
                {
                    product.IsPlatter = Source.IsPlatter;
                    product.Additives = (Source as Platter).Additives;
                    product.IdAdditives = (Source as Platter).IdAdditives;
                }

                Set(ref _name, _source.Name);
                Set(ref _price, _source.Price);
                Set(ref _unit, _source.Unit);
                return product;
            }
            else
            {
                return _source;
            }
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

        private void PopulateAdditivesPage()
        {
            var comparer = new Comparer<Additive>();
            int StatusCode = 0;
            var allAdditives = _additiveService.GetAllAdditives(ref StatusCode).ToList();
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
            if (Product.Id != null)
            {
                _productService.UpdateProduct(this._source);
            }
            else
            {
                long id = -1;
                _productService.SaveProduct(this._source, ref id);
                Product.Id = id;
                this._source.Id = id;
            }
        }

        public void CheckAdditive(object sender, RoutedEventArgs e, object add)
        {
            ListBox listView = sender as ListBox;
            ListBoxItem listBoxItem =
                FindAncestor<ListBoxItem>((DependencyObject) e.OriginalSource);

            if (listBoxItem == null)
            {
                return;
            }

            Additive source = (Additive) listView.ItemContainerGenerator.ItemFromContainer(listBoxItem);


            ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(listBoxItem);

            // Finding textBlock from the DataTemplate that is set on that ContentPresenter
            DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
            ToggleButton toggleButton = (ToggleButton) myDataTemplate.FindName("ToggleButton", myContentPresenter);


            if ((Product as Platter).Additives.Contains(source) && !(bool) toggleButton?.IsChecked)
            {
                ((Platter) Product).IdAdditives.Remove(source.Id);
                ((Platter) Product).Additives.Remove(source);
                return;
            }

            if (!(Product as Platter).Additives.Contains(source) && (bool) toggleButton?.IsChecked)
            {
                ((Platter) Product).IdAdditives.Add(source.Id);
                ((Platter) Product).Additives.Add(source);
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
            this.Product = new Product();
            this.Name = null;
            this.Price = 0;
            this.Unit = null;
            NotifyOfPropertyChange(() => this.Product);
            NotifyOfPropertyChange(() => this.Name);
            NotifyOfPropertyChange(() => this.Price);
            NotifyOfPropertyChange(() => this.Unit);
        }

        private static void Clone(ref Product source, ref Product target)
        {
            if (source == null) return;
            target.Name = source.Name;
            target.Price = source.Price;
            target.Background = source.Background;
            target.Unit = source.Unit;
            if (target is Platter)
            {
                (target as Platter).IdAdditives = (source as Platter).IdAdditives;
                (target as Platter).Additives = (source as Platter).Additives;
            }
        }

        public Product Source
        {
            get { return _source; }
            set
            {
                Set(ref _source, value);
                Product = CloneFromSource();
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

        public bool HasErrors
        {
            get { return _validationErrors.Count > 0; }
        }

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