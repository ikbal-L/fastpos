﻿using Caliburn.Micro;
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
    public class ProductsViewModel : Screen
    {
        private IProductService _productsService;
        private ICategoryService _categorieService;
        private IAdditiveService _additiveService;
        private bool _IsDialogOpen;
        private BindableCollection<Additive> _availableAdditives;

        public BindableCollection<Product> Products { get; set; }
        public BindableCollection<Category> Categories { get; set; }
        public BindableCollection<Additive> Additives { get; set; }
        public BindableCollection<Product> ProductsCanCompositIngredients { get; set; }
        
        public BindableCollection<Additive> AvailableAdditives
        {
            get => _availableAdditives;
            set
            {
                _availableAdditives = value;
                NotifyOfPropertyChange(() => AvailableAdditives);
            }
        }

        private Product _currentProduct;
        
        public Product CurrentProduct
        {
            get { return _currentProduct; }
            set
            {
                _currentProduct = value;
                NotifyOfPropertyChange(() => CurrentProduct);
            }
        }

        private Additive _selectedAvailableAdditive;
        public Additive SelectedAvailableAdditive
        {
            get { return _selectedAvailableAdditive; }
            set
            {
                _selectedAvailableAdditive = value;
                NotifyOfPropertyChange(() => SelectedAvailableAdditive);

            }
        }

        private Additive _selectedAddedAdditive;
        public Additive SelectedAddedAdditive
        {
            get { return _selectedAddedAdditive; }
            set
            {
                _selectedAddedAdditive = value;
                NotifyOfPropertyChange(() => SelectedAddedAdditive);

            }
        }

        public bool ListViewSwitcher { get; set; }
        public int CurrentProductIndex { get; set; }

        public int pageSize;
        private int _pageSize;


        public bool IsDialogOpen
        {
            get => _IsDialogOpen;
            set => Set(ref _IsDialogOpen, value);
        }
        
        public Product SelectedCompositProduct { get; set; }

        public ProductsViewModel()
        {
        }

        public ProductsViewModel(int pageSize,
            IProductService productsService,
            ICategoryService categorieService,
            IAdditiveService additiveService )
        {
            _pageSize = pageSize;
            _productsService = productsService;
            _categorieService = categorieService;
            _additiveService = additiveService;

            int getProductsStatusCode = 0,
                getCategoriesStatusCode = 0,
                getAdditiveStatusCode = 0;
            Products = new BindableCollection<Product>(_productsService.GetAllProducts(ref getProductsStatusCode));
            ProductsCanCompositIngredients = new BindableCollection<Product>(Products.Where(p => p.IsPlatter == false));
            Categories = new BindableCollection<Category>(_categorieService.GetAllCategories(ref getCategoriesStatusCode));
            Additives = new BindableCollection<Additive>(_additiveService.GetAllAdditives(ref getAdditiveStatusCode));

            CurrentProduct = Products.Cast<Product>().FirstOrDefault();
        }

        public void UpdateAdditivesList()
        {
            if(CurrentProduct is Platter plat && plat.Additives != null) 
            {
                var platter = (Platter)CurrentProduct;
                var exceptList = Additives?.Where( a => !platter.Additives.Any(pa => pa.Equals(a)));
                AvailableAdditives = 
                    new BindableCollection<Additive>(exceptList?.OrderBy(f => f.Description));
            }
        }

        public void DeleteCommand()
        {
            if (_currentProduct == null)
                return;

            _productsService.DeleteProduct((long)_currentProduct.Id);
        }     

        public void NewCommand()
        {
            CurrentProduct = new Product();
            Products.Add(CurrentProduct);
            IsDialogOpen = true;
        }

        public void EditCommand()
        {
            IsDialogOpen = true;
            UpdateAdditivesList();
        }

        public void SaveCommand()
        {
            if (CurrentProduct.Id <= 0)
            {
                var _nextId = Products.Count == 0 ? 0 : Products.Max(f => f.Id);
                CurrentProduct.Id = _nextId + 1;
                // Insert Product
                //_productsService.SaveProduct(CurrentProduct);
            }
            else
            {
                _productsService.UpdateProduct(CurrentProduct);
            }
            IsDialogOpen = false;
        }

        public void CancelCommand()
        {

            if (CurrentProduct != null)
            {
                // id = 0 is new product
                if (CurrentProduct.Id == 0)
                    Products.Remove(CurrentProduct);
                if (CurrentProduct.Id != 0)
                {
                    var getProductsStatusCode = 0;
                     CurrentProduct = Products[CurrentProductIndex]
                        =_productsService.GetProduct((long)CurrentProduct.Id, ref getProductsStatusCode);
                }

            }
            IsDialogOpen = false;
        }
        
        public void AddAllAdditivesCommand()
        {
            if (AvailableAdditives == null)
                return;

            if (CurrentProduct is Platter platter)
            {
                if (platter.Additives == null)
                    platter.Additives = new BindableCollection<Additive>();
                platter.Additives.Clear();
                platter.Additives.AddRange(Additives);
                UpdateAdditivesList();
            }
        }
        public void AddAdditiveCommand()
        {
            if (SelectedAvailableAdditive == null)
                    return;

            if(CurrentProduct is Platter platter)
            {
                if (platter == null)
                    platter.Additives = new BindableCollection<Additive>();

                platter.Additives.Add(SelectedAvailableAdditive);
                UpdateAdditivesList();
            }
            
        }
        public void RemoveAdditiveCommand()
        {
            if (CurrentProduct is Platter platter)
            {
                if (SelectedAddedAdditive == null 
                    || platter.Additives.Count == 0)
                    return;
                platter.Additives.Remove(SelectedAddedAdditive);
                UpdateAdditivesList();
            }
        }
        public void RemoveAllAdditivesCommand()
        {
            if (CurrentProduct is Platter platter)
            {
                if (platter.Additives == null || platter.Additives.Count == 0)
                    return;
                platter.Additives.Clear();
                UpdateAdditivesList();
            }
        }

        public void addNewIngredientCommand(string Quantity)
        {
            int quantity = 0;
            if(int.TryParse(Quantity,out quantity))
            {
                if(CurrentProduct is Platter platter)
                {
                    if (platter.Ingredients == null)
                        platter.Ingredients = new List<Ingredient>();
                    platter.Ingredients.Add(new Ingredient { Product = SelectedCompositProduct, Quantity = quantity });
                }
            }
            
        }

        //public bool CanAddAllAdditivesCommand(string Quantity)
        //{
        //    //return AvailableAdditives == null || AvailableAdditives.Count==0;
        //}


    }
}
