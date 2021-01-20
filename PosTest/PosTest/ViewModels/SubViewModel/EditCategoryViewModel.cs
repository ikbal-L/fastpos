using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceLib.Service;
using ServiceInterface.Model;

namespace PosTest.ViewModels.SubViewModel
{
    public class EditCategoryViewModel : PropertyChangedBase, INotifyDataErrorInfo
    {
        private Category _category;
        private Category _source;

        private readonly Dictionary<string, ICollection<string>> _validationErrors =
            new Dictionary<string, ICollection<string>>();

        private string _name;
        private string _description;
        private bool _isSaveEnabled;

        public EditCategoryViewModel(ref Category sourceCategory)
        {
            
            
            this._source = sourceCategory;
            _category = new Category();
            CloneFromSource();
           
            IsSaveEnabled = !HasErrors;
        }


        public string Name
        {
            get => _name;
            set
            {
                Set(ref _name, value);
                Category.Name = _name;
                ValidateModelProperty(Category,Category.Name,nameof(Category.Name));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                Set(ref _description, value);
                Category.Description = _description;
                ValidateModelProperty(Category, Category.Description, nameof(Category.Description));
            }
        }

        public Category Category
        {
            get => _category;
            set
            {
                Set(ref _category, value);
                ValidateModelProperty(Category, Category.Name, nameof(Category.Name));
                ValidateModelProperty(Category, Category.Description, nameof(Category.Description));
            }
        }


        public Category Source
        {
            private get { return _source; }
            set
            {
                Set(ref _source, value);
                CloneFromSource();
                NotifyOfPropertyChange(() => this.Category);
            }
        }


        public void SaveCategory()
        {
            Clone(source: ref _category, target: ref this._source);
            NotifyOfPropertyChange(() => Source);
            StateManager.Save<Category>(this._source);
        }

        

        private static void Clone(ref Category source, ref Category target)
        {
            if (source == null) return;
            target.Name = source.Name;
            target.Description = source.Description;
            target.Background = source.Background;
            target.BackgroundColor = source.BackgroundColor;
            if (target.Id == null)
            {
                target.ProductIds = source.ProductIds;
                target.Products = source.Products;
            }
        }

        public void CloneFromSource()
        {
            if (_source!=null)
            {
                var category = new Category
                {
                    Name = _source.Name,
                    Description = _source.Description,
                    Background = _source.Background,
                    BackgroundColor = _source.BackgroundColor
                };
                if (_source.Id == null)
                {
                    category.ProductIds = new List<long>();
                    category.Products = new List<Product>();
                }
                Category = category;
                Name = _source.Name;
                Description = _source.Description;
                
                
            }
            else
            {
                Category = new Category();
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
            get
            {
                return _validationErrors.Count > 0;
            }
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