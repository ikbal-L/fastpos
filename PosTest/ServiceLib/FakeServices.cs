using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLib
{
    public class FakeServices
    {
        static int currentcategorieId = 1;

        static List<Categorie> category = new List<Categorie>()
        {
            //SELECT `chp_des`,`prix_carte` FROM `corres_des` WHERE `chp_des` like '%BOISSONS%'
            new Categorie { Id = currentcategorieId++, Name = "PIZZA"},
            new Categorie { Id = currentcategorieId++, Name = "TACO"},
            new Categorie { Id = currentcategorieId++, Name = "BURGER"},
            new Categorie { Id = currentcategorieId++, Name = "KEBAB"},
            new Categorie { Id = currentcategorieId++, Name = "BOISSONS"},
        };
        

        public static List<Categorie> Category
        {
            get
            {
                return new List<Categorie>(category);
            }
        }

        //------------------------------------------------------------------------------------------------------
        static int currentProductId = 1;

        static List<Product> products = new List<Product>()
        {
            new Product { Id = currentProductId++, Name = "Pizza maison", Price = 15, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "PIZZA VIANDES", Price = 29, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "PIZZA POULET", Price = 26, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "PIZZA BOEUF CHILI", Price = 29, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "PIZZA BUFFALO", Price = 26, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "PIZZA OEUF PIMENT VERT", Price = 24, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "PIZZA JARDINIER", Price = 27, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "PIZZA HAWAII", Price = 26, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "PIZZA BACON", Price = 30, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "PIZZA PTIT DEJ", Price = 32, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "PIZZA POULET BACON", Price = 35, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "PIZZA PEPERONNI", Price = 35, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "PIZZA TACO", Price = 26, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "PIZZA MARGHERITA", Price = 29, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "PIZZA CHAMPI", Price = 23, CategorieId = 1},
            new Product { Id = currentProductId++, Name = "TACOS FISH", Price = 15, CategorieId = 2},
            new Product { Id = currentProductId++, Name = "BEEF SOFT TACO", Price = 15, CategorieId = 2},
            new Product { Id = currentProductId++, Name = "FRESCO SOFT TACO", Price = 12, CategorieId = 2},
            new Product { Id = currentProductId++, Name = "FRESH TACO", Price = 18, CategorieId = 2},
            new Product { Id = currentProductId++, Name = "FRESCO STEAK TACO", Price = 18, CategorieId = 2},
            new Product { Id = currentProductId++, Name = "TACO CRUNCH WRAP", Price = 15, CategorieId = 2},
            new Product { Id = currentProductId++, Name = "TACO CHICKEN", Price = 18, CategorieId = 2},
            new Product { Id = currentProductId++, Name = "Bitburger", Price = 7.5M, CategorieId = 3},
            new Product { Id = currentProductId++, Name = "Hamburger", Price = 7, CategorieId = 3},
            new Product { Id = currentProductId++, Name = "Burger", Price = 10, CategorieId = 3},
            new Product { Id = currentProductId++, Name = "BLUES BURGER", Price = 12, CategorieId = 3},
            new Product { Id = currentProductId++, Name = "ESCAL BURGER", Price = 11, CategorieId = 3},
            new Product { Id = currentProductId++, Name = "SANDY BURGER", Price = 9, CategorieId = 3},
            new Product { Id = currentProductId++, Name = "BURGER 120", Price = 8, CategorieId = 3},
            new Product { Id = currentProductId++, Name = "CHEESE BURGER", Price = 16, CategorieId = 3},
            new Product { Id = currentProductId++, Name = "CHICKEN BURGER", Price = 14, CategorieId = 3},
            new Product { Id = currentProductId++, Name = "CROOSTI BURGER", Price = 15, CategorieId = 3},
            new Product { Id = currentProductId++, Name = "FISH BURGER", Price = 12, CategorieId = 3},
            new Product { Id = currentProductId++, Name = "KEBAB VEAU", Price = 15, CategorieId = 4},
            new Product { Id = currentProductId++, Name = "KEBAB MERGEZ", Price = 15, CategorieId = 4},
            new Product { Id = currentProductId++, Name = "ASSIETTE TACO KEBAB", Price = 26, CategorieId = 4},
            new Product { Id = currentProductId++, Name = "KEBAB TRADITION", Price = 18, CategorieId = 4},
            new Product { Id = currentProductId++, Name = "TWISTER KEBAB", Price = 18, CategorieId = 4},
            new Product { Id = currentProductId++, Name = "ORANGINA", Price = 9, CategorieId = 5},
            new Product { Id = currentProductId++, Name = "COCA COLA", Price = 10, CategorieId = 5},
            new Product { Id = currentProductId++, Name = "COCA-ZERO", Price = 11, CategorieId = 5},
            new Product { Id = currentProductId++, Name = "COCA-LIGHT", Price = 14, CategorieId = 5},
        };

        public static List<Product> Products
        {
            get
            {
                return new List<Product>(products);
            }
        }



    }
}
