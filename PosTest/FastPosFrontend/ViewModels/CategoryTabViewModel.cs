using Caliburn.Micro;
using ServiceInterface.Interface;
using ServiceInterface.Model;

namespace FastPosFrontend.ViewModels
{
    class CategoryTabViewModel : Screen
    {

        public CategoryTabViewModel(int pageSize, IProductService productService, ICategoryService categorieService)
        {

            //currentOrderitem = new BindableCollection<OrdreItem>();
            int getProductsStatusCode = 0;
            
            var AllRequestedProducts = productService.GetAllProducts(ref getProductsStatusCode);
            var (getCategoriesStatusCode,categories) = categorieService.GetAllCategories( );
            var Categories = new BindableCollection<Category>(categories);
            CategoriesTab = new BindableCollection<CategoryItem>();

            foreach (var p in AllRequestedProducts)
            {
                if (CategoriesTab.Count == 0)
                {
                    var products = new BindableCollection<Product>();
                    products.Add(p);
                    CategoriesTab.Add(new CategoryItem { Category = p.Category, Products = products });
                    continue;
                }

                bool exists = false;
                foreach(var item in CategoriesTab)
                {
                    if (item.Category.Equals(p.Category))
                    {
                        if (item.Products == null)
                            item.Products = new BindableCollection<Product>();
                        item.Products.Add(p);
                        exists = true;
                        continue;
                    }                   
                }

                if (!exists)
                {
                    var products = new BindableCollection<Product>();
                    products.Add(p);
                    CategoriesTab.Add(new CategoryItem { Category = p.Category, Products = products });
                }
            }
        }

        public BindableCollection<CategoryItem> CategoriesTab { get; set; }
    }

    public class CategoryItem : PropertyChangedBase
    {
        private Category _category;
        private BindableCollection<Product> _products;

        public Category Category 
        {
            get => _category;
            set
            {
                _category = value;
                //NotifyOfPropertyChange(() => Category);
            }
        }

        public BindableCollection<Product> Products 
        {
            get => _products;
            set
            {
                _products = value;
                //NotifyOfPropertyChange(() => Products);
            }
        }
    }
}
