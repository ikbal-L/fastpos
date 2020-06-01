﻿using Caliburn.Micro;
using ServiceInterface.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ServiceInterface.Model;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel;
using System.Windows.Data;

namespace PosTest.ViewModels
{
    public class LoginViewModel : Screen
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [Import(typeof(IProductService))]
        private IProductService productService = null;

        [Import(typeof(ICategoryService))]
        private ICategoryService categorieService = null;

        [Import(typeof(IAuthentification))]
        private IAuthentification authService = null;


        //public String Username { get; set; }
        //public String Password { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            this.Compose(); 
        }
        public bool CanLogin()
        {
            //logger.Info("User: " + username + "  Pass: " + password);
            return true;// !String.IsNullOrEmpty(username)/* && !String.IsNullOrEmpty(password)*/;
        }

        public void Login()
        {
            authService.Authenticate("mbeggas", "mmmm1111", new Annex { Id = 1 }, new Terminal { Id = 1 });
            CheckoutViewModel checkoutViewModel = 
                new CheckoutViewModel(30, 
                productService, 
                categorieService);

            checkoutViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(checkoutViewModel);
        }
        
        public void Settings()
        {
            SettingsViewModel settingsViewModel = new SettingsViewModel(30, productService, categorieService);
            settingsViewModel.Parent = this.Parent;
            (this.Parent as Conductor<object>).ActivateItem(settingsViewModel);
        }

        //This method load th DLL file containing the implemetation of IProductService 
        // and satisfay the import in this class
        private void Compose()
        {
            //AssemblyCatalog catalog1 = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
            AssemblyCatalog catalog2 = new AssemblyCatalog("ServiceLib.dll");
            CompositionContainer container = new CompositionContainer(catalog2);
            container.SatisfyImportsOnce(this);
        }
    }
}
