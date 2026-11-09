using System;
using System.Collections.Generic;

namespace Fuentes_Week05_Work04
{
    public class ProductInfo
    {
        public string Barcode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Origin { get; set; }
        public bool IsActive { get; set; }
        public decimal Cost { get; set; }
        public decimal Markup { get; set; }
        public decimal Price { get; set; }
    }

    // Holds starter products and products saved during the current app session.
    public class initialData
    {
        private readonly Dictionary<string, ProductInfo> products =
            new Dictionary<string, ProductInfo>(StringComparer.OrdinalIgnoreCase);

        public initialData()
        {
            SaveProduct(new ProductInfo
            {
                Barcode = "10001",
                Name = "Keyboard",
                Description = "USB keyboard",
                Category = "Electronics",
                Origin = "Local",
                IsActive = true,
                Cost = 500m,
                Markup = 20m,
                Price = 600m
            });

            SaveProduct(new ProductInfo
            {
                Barcode = "10002",
                Name = "Notebook",
                Description = "A5 ruled notebook",
                Category = "Books and Stationery",
                Origin = "Abroad",
                IsActive = true,
                Cost = 75m,
                Markup = 20m,
                Price = 90m
            });

            SaveProduct(new ProductInfo
            {
                Barcode = "10003",
                Name = "Desk Lamp",
                Description = "LED desk lamp",
                Category = "Household",
                Origin = "Local",
                IsActive = false,
                Cost = 800m,
                Markup = 15m,
                Price = 920m
            });
        }

        public ProductInfo FindByBarcode(string barcode)
        {
            if (string.IsNullOrWhiteSpace(barcode))
                return null;

            ProductInfo product;
            products.TryGetValue(barcode.Trim(), out product);
            return product;
        }

        public void SaveProduct(ProductInfo product)
        {
            if (product == null)
                throw new ArgumentNullException("product");
            if (string.IsNullOrWhiteSpace(product.Barcode))
                throw new ArgumentException("A product barcode is required.", "product");

            products[product.Barcode.Trim()] = product;
        }
    }
}
