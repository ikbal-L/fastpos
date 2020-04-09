using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceInterface.Model
{
    public class Product
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public String Unit { get; set; }


        public int CategorieId { get; set; }
        public Categorie Categorie { get; set; }

        public String Type { get; set; }
        public String Color { get; set; }
        
        public string PictureFileName { get; set; }
        public string PictureUri { get; set; }

        public int AvailableStock { get; set; }


    }
}
