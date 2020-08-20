using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PosTest.ViewModels
{
    public class CheckoutSettingsViewModel : Screen
    {
        private bool _IsCategoryDetailsDrawerOpen;
        private bool _IsProductDetailsDrawerOpen;

        private bool _IsDeleteCategoryDialogOpen;
        private bool _IsDeleteProductDialogOpen;

        public bool IsCategoryDetailsDrawerOpen
        {
            get => _IsCategoryDetailsDrawerOpen;
            set => Set(ref _IsCategoryDetailsDrawerOpen, value);
        }
        public bool IsProductDetailsDrawerOpen
        {
            get => _IsProductDetailsDrawerOpen;
            set => Set(ref _IsProductDetailsDrawerOpen, value);
        }
        
        public bool IsDeleteCategoryDialogOpen
        {
            get => _IsDeleteCategoryDialogOpen;
            set => Set(ref _IsDeleteCategoryDialogOpen, value);
        }

        public bool IsDeleteProductDialogOpen
        {
            get => _IsDeleteProductDialogOpen;
            set => Set(ref _IsDeleteProductDialogOpen, value);
        }
        
        public void DetailsCategory()
        {
            IsCategoryDetailsDrawerOpen = true;
        }

        public void DetailsProduct()
        {
            IsProductDetailsDrawerOpen = true;
        }     
        public void DeleteCategory()
        {
            IsDeleteCategoryDialogOpen = true;
   
        }    
        
        public void DeleteProduct()
        {
            IsDeleteProductDialogOpen = true;
   
        }


    }
}
