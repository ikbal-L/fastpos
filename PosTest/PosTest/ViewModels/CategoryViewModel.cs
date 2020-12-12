using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PosTest.ViewModels
{
    public class CategoryViewModel : Screen
    {
        private ICategoryService _categorieService;
        public bool ListViewSwitcher { get; set; }

        private int _pageSize;

        public BindableCollection<Category> Categories { get; set; }


        private bool _IsDialogOpen;
        public bool IsDialogOpen
        {
            get => _IsDialogOpen;
            set => Set(ref _IsDialogOpen, value);
        }

        private Category _currentCategory;
        public Category CurrentCategory
        {
            get { return _currentCategory; }
            set
            {
                _currentCategory = value;
                NotifyOfPropertyChange(() => CurrentCategory);
            }
        }

        public CategoryViewModel()
        {
        }

        public CategoryViewModel(int pageSize, IProductService productsService, ICategoryService categorieService)
        {
            //_productsService = productsService;
            //Products = BindableCollection.GetDefaultView(_productsService.GetAllProducts());
            //Products = new BindableCollection<Product>( _productsService.GetAllProducts());
            _pageSize = pageSize;
            _categorieService = categorieService;
            var(getCategoriesStatusCode ,allCategories) = _categorieService.GetAllCategories();
            Categories = new BindableCollection<Category>(allCategories);
            CurrentCategory = Categories.Cast<Category>().FirstOrDefault();

        }

        public void NewCommand()
        {
            CurrentCategory = new Category();
            Categories.Add(CurrentCategory);
            IsDialogOpen = true;
        }

        public void SaveCommand()
        {
            if (CurrentCategory.Id <= 0)
            {
                var _nextId = Categories.Count == 0 ? 0 : Categories.Max(f => f.Id);
                CurrentCategory.Id = _nextId + 1 ;
                //_categorieService.SaveCategory(CurrentCategory);
            }
            else
            {
                var t= _categorieService.UpdateCategory(CurrentCategory, out IEnumerable<string> errorsOfCurrentCategory);
            }
            IsDialogOpen = false;
        }

        public void DeleteCommand()
        {
            if (_currentCategory == null)
                return;
            _categorieService.DeleteCategory((long)_currentCategory.Id);
        }     

        public void CancelCommand()
        {
            if (CurrentCategory != null)
            {
                if (CurrentCategory.Id == 0)
                    Categories.Remove(CurrentCategory);
            }
            IsDialogOpen = false;
        }
    }
}
