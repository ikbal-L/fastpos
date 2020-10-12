using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceLib.Service;
using ServiceInterface.Model;

namespace PosTest.ViewModels.SubViewModel
{
    public class EditCategoryViewModel :  PropertyChangedBase
    {
        private Category _category;
        private ICategoryService _categoryService;
        private Category _source;

        public EditCategoryViewModel(ref Category sourceCategory,ICategoryService categoryService)
        {
            _category = new Category();
            Clone(source:ref sourceCategory,target:ref _category);
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
            }
        }

        public void SaveCategory()
        {
            Clone(source:ref _category,target:ref this._source);
            NotifyOfPropertyChange(()=>Source);
            if (Source.Id != null)
            {
                _categoryService.UpdateCategory(this._source);

            }
            else
            {
                long id = -1;
                _categoryService.SaveCategory(this._source,ref id);
                Category.Id = id;
                this._source.Id = id;
            }

        }

        public void Cancel()
        {
            this.Category = new Category();
            NotifyOfPropertyChange(()=>this.Category);
        }

        private static  void Clone(ref Category source, ref Category target)
        {
            if (source ==null) return;
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
                NotifyOfPropertyChange(()=>this.Category);
            }
        }
    }
}
