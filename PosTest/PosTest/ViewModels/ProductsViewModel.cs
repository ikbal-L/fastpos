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
        public bool ListViewSwitcher { get; set; }

        public int pageSize;
        public IProductService productService;

        public BindableCollection<Product> Products { get; set; }
        public BindableCollection<Category> Categories { get; set; }

        private Product _currentProduct;
        private int _pageSize;
        private IProductService _productsService;
        private ICategorieService _categorieService;

        private bool _IsDialogOpen;
        public bool IsDialogOpen
        {
            get => _IsDialogOpen;
            set => Set(ref _IsDialogOpen, value);
        }
        public Product CurrentProduct
        {
            get { return _currentProduct; }
            set
            {
                _currentProduct = value;
                NotifyOfPropertyChange(() => CurrentProduct);

            }
        }

        public ProductsViewModel()
        {
        }

        public ProductsViewModel(int pageSize, IProductService productsService, ICategorieService categorieService)
        {
            _pageSize = pageSize;
            _productsService = productsService;
            _categorieService = categorieService;

            //Products = BindableCollection.GetDefaultView(_productsService.GetAllProducts());
            Products = new BindableCollection<Product>( _productsService.GetAllProducts());
            Categories = new BindableCollection<Category>(_categorieService.GetAllCategory());
            CurrentProduct = Products.Cast<Product>().FirstOrDefault();
        }

        public void DeleteCommand()
        {
            if (_currentProduct == null)
                return;
            _productsService.DeleteProduct(_currentProduct.Id);
        }     
        public void NewCommand()
        {
            CurrentProduct = new Product();
            Products.Add(CurrentProduct);
            IsDialogOpen = true;
        }
       public void SaveCommand()
        {
            _productsService.SaveProduct(CurrentProduct);
            IsDialogOpen = false;
        }

        public void CancelCommand()
        {
            if (CurrentProduct != null)
            {
                if (CurrentProduct.Id == 0)
                    Products.Remove(CurrentProduct);
            }
            IsDialogOpen = false;
        }
    }
}
