﻿using Caliburn.Micro;
using ServiceInterface.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceLib
{
    public class FakeServices
    {
        static int currentcategorieId = 1;


        public static List<Additive> Additives { get; }

        static List<Category> categories = new List<Category>()
        {
            //SELECT `chp_des`,`prix_carte` FROM `corres_des` WHERE `chp_des` like '%BOISSONS%'
            new Category { Id = currentcategorieId++, Name = "PIZZA", },
            new Category { Id = currentcategorieId++, Name = "TACO"},
            new Category { Id = currentcategorieId++, Name = "BURGER"},
            new Category { Id = currentcategorieId++, Name = "KEBAB"},
            new Category { Id = currentcategorieId++, Name = "BOISSONS"},
        };
        

        public static List<Category> Categories
        {
            get
            {
               /* foreach(var cat in categories)
                {
                    cat.Products = Products.Where(p => p.Category.Equals(cat)).ToList();
                }*/
                categories.ForEach(cat => cat.Products = Products.Where(p => p.Category.Equals(cat)).ToList());
                categories.ForEach(cat => { Console.Write("Cat= " + cat.Id); cat.Products.ForEach(p => Console.Write(" " + p.Id)); Console.WriteLine(); }) ; 
                return categories;
            }
        }

        //------------------------------------------------------------------------------------------------------
        static int currentProductId = 1;
        static List<Product> products;
        static FakeServices() 
        {
            products = new List<Product>()
            {
                new Product { Id = currentProductId++, Name = "Pizza maison", Price = 15, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1)?.FirstOrDefault() },
                new Product { Id = currentProductId++, Name = "PIZZA VIANDES", Price = 29, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1)?.FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "PIZZA POULET", Price = 26, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "PIZZA BOEUF CHILI", Price = 29, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "PIZZA BUFFALO", Price = 26, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "PIZZA OEUF PIMENT VERT", Price = 24, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "PIZZA JARDINIER", Price = 27, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "PIZZA HAWAII", Price = 26, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "PIZZA BACON", Price = 30, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "PIZZA PTIT DEJ", Price = 32, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "PIZZA POULET BACON", Price = 35, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "PIZZA PEPERONNI", Price = 35, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "PIZZA TACO", Price = 26, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "PIZZA MARGHERITA", Price = 29, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "PIZZA CHAMPI", Price = 23, CategoryId = 1, Category=categories.Where(cat => cat.Id == 1).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "TACOS FISH", Price = 15, CategoryId = 2, Category=categories.Where(cat => cat.Id == 2).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "BEEF SOFT TACO", Price = 15, CategoryId = 2, Category=categories.Where(cat => cat.Id == 2).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "FRESCO SOFT TACO", Price = 12, CategoryId = 2, Category=categories.Where(cat => cat.Id == 2).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "FRESH TACO", Price = 18, CategoryId = 2, Category=categories.Where(cat => cat.Id == 2).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "FRESCO STEAK TACO", Price = 18, CategoryId = 2, Category=categories.Where(cat => cat.Id == 2).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "TACO CRUNCH WRAP", Price = 15, CategoryId = 2, Category=categories.Where(cat => cat.Id == 2).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "TACO CHICKEN", Price = 18, CategoryId = 2, Category=categories.Where(cat => cat.Id == 2).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "Bitburger", Price = 7.5M, CategoryId = 3, Category=categories.Where(cat => cat.Id == 3).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "Hamburger", Price = 7, CategoryId = 3, Category=categories.Where(cat => cat.Id == 3).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "Burger", Price = 10, CategoryId = 3, Category=categories.Where(cat => cat.Id == 3).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "BLUES BURGER", Price = 12, CategoryId = 3, Category=categories.Where(cat => cat.Id == 3).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "ESCAL BURGER", Price = 11, CategoryId = 3, Category=categories.Where(cat => cat.Id == 3).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "SANDY BURGER", Price = 9, CategoryId = 3, Category=categories.Where(cat => cat.Id == 3).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "BURGER 120", Price = 8, CategoryId = 3, Category=categories.Where(cat => cat.Id == 3).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "CHEESE BURGER", Price = 16, CategoryId = 3, Category=categories.Where(cat => cat.Id == 3).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "CHICKEN BURGER", Price = 14, CategoryId = 3, Category=categories.Where(cat => cat.Id == 3).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "CROOSTI BURGER", Price = 15, CategoryId = 3, Category=categories.Where(cat => cat.Id == 3).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "FISH BURGER", Price = 12, CategoryId = 3, Category=categories.Where(cat => cat.Id == 3).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "KEBAB VEAU", Price = 15, CategoryId = 4, Category=categories.Where(cat => cat.Id == 4).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "KEBAB MERGEZ", Price = 15, CategoryId = 4, Category=categories.Where(cat => cat.Id == 4).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "ASSIETTE TACO KEBAB", Price = 26, CategoryId = 4, Category=categories.Where(cat => cat.Id == 4).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "KEBAB TRADITION", Price = 18, CategoryId = 4, Category=categories.Where(cat => cat.Id == 4).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "TWISTER KEBAB", Price = 18, CategoryId = 4, Category=categories.Where(cat => cat.Id == 4).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "ORANGINA", Price = 9, CategoryId = 5, Category=categories.Where(cat => cat.Id == 4).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "COCA COLA", Price = 10, CategoryId = 5, Category=categories.Where(cat => cat.Id == 5).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "COCA-ZERO", Price = 11, CategoryId = 5, Category=categories.Where(cat => cat.Id == 5).FirstOrDefault()},
                new Product { Id = currentProductId++, Name = "COCA-LIGHT", Price = 14, CategoryId = 5, Category=categories.Where(cat => cat.Id == 5).FirstOrDefault()},
            };

            int addId = 0;
            List<Additive> additives = new List<Additive>
            {
                new Additive {Id=addId++, Description="Harissa+" },
                new Additive {Id=addId++, Description="Harissa++" },
                new Additive {Id=addId++, Description="Mayonaise+" },
                new Additive {Id=addId++, Description="Sana Ognion+" },
                new Additive {Id=addId++, Description="Sana Fromage+" },
                new Additive {Id=addId++, Description="Avec Kecchap" },
                new Additive {Id=addId++, Description="Sans Olive" },
                new Additive {Id=addId++, Description="Harissa+++" },
            };
            Additives = additives;
            products.ForEach(
                p =>
                {
                    if (p.CategoryId == 1)
                        p.Additives = new BindableCollection<Additive>(additives);
                    if (p.Id % 2 == 0)
                        p.IsMuchInDemand = true;
                }
            );
           
        }

        public static List<Product> Products 
        {
            get => products;
        }
    }
}
