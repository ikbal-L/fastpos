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
        private ICategoryService _categoryService;
        private Category _source;

        private readonly Dictionary<string, ICollection<string>> _validationErrors =
            new Dictionary<string, ICollection<string>>();

        public EditCategoryViewModel(ref Category sourceCategory, ICategoryService categoryService)
        {
            _category = new Category();
            Clone(source: ref sourceCategory, target: ref _category);
            this._source = sourceCategory;

            this._categoryService = categoryService;
        }

        public Category Category
        {
            get => _category;
            set
            {
                _category = value;
                Set(ref _category, value);
                ValidateModelProperty(Category.Name, nameof(Category.Name));
                ValidateModelProperty(Category.Description, nameof(Category.Description));
            }
        }

        public void SaveCategory()
        {
            Clone(source: ref _category, target: ref this._source);
            NotifyOfPropertyChange(() => Source);
            if (Source.Id != null)
            {
                _categoryService.UpdateCategory(this._source);
            }
            else
            {
                long id = -1;
                _categoryService.SaveCategory(this._source, ref id);
                Category.Id = id;
                this._source.Id = id;
            }
        }

        public void Cancel()
        {
            this.Category = new Category();
            NotifyOfPropertyChange(() => this.Category);
        }

        private static void Clone(ref Category source, ref Category target)
        {
            if (source == null) return;
            target.Name = source.Name;
            target.Description = source.Description;
            target.Background = source.Background;
            target.BackgroundColor = source.BackgroundColor;
        }

        public Category Source
        {
            private get { return _source; }
            set
            {
                Set(ref _source, value);
                Clone(source: ref _source, target: ref _category);
                NotifyOfPropertyChange(() => this.Category);
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

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        protected void ValidateModelProperty(object value, string propertyName)
        {
            if (_validationErrors.ContainsKey(propertyName))
                _validationErrors.Remove(propertyName);

            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            ValidationContext validationContext =
                new ValidationContext(Category, null, null) { MemberName = propertyName };
            if (!Validator.TryValidateProperty(value, validationContext, validationResults))
            {
                _validationErrors.Add(propertyName, new List<string>());
                foreach (ValidationResult validationResult in validationResults)
                {
                    _validationErrors[propertyName].Add(validationResult.ErrorMessage);
                }
            }
            RaiseErrorsChanged(propertyName);
        }
    }
}