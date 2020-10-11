using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceInterface.Model;

namespace PosTest.ViewModels.SubViewModel
{
    public class EditProductVeiwModel: PropertyChangedBase
    {
        private Product _product;
        private IProductService _productService;
        private Product _source;

        public EditProductVeiwModel(ref Product sourceProduct, IProductService productService)
        {
            _product = new Product();
            Clone(source: ref sourceProduct, target: ref _product);
            this._source = sourceProduct;

            this._productService = productService;
        }

        public Product Product
        {
            get => _product;
            set
            {
                _product = value;
                Set(ref _product, value);
            }
        }

        public void SaveCategory()
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

        public void Cancel()
        {
            this.Product = new Product();
            NotifyOfPropertyChange(() => this.Product);
        }

        private static void Clone(ref Product source, ref Product target)
        {
            if (source == null) return;
            target.Id = source.Id;
            target.Name = source.Name;
            target.Price = source.Price;
            target.Background = source.Background;
            target.Unit = source.Unit;
        }

        public Product Source
        {
            private get { return _source; }
            set
            {
                Set(ref _source, value);
                Clone(source: ref _source, target: ref _product);
                NotifyOfPropertyChange(() => this.Product);
            }
        }
    }
}