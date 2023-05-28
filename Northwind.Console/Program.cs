using Northwind.Database.Tables;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Northwind.ConsoleApp
{
    internal class Program
    {
        static HttpClient _client = new HttpClient();

        static async Task Main(string[] args)
        {
            // Update port # in the following line.
            _client.BaseAddress = new Uri("https://localhost:44346");
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            // Get categories
            var categories = await GetCollectionAsync<Category>("/api/Collection/Categories?maxAmount=10");
            ShowCollection(categories, "Categories:");

            // Get categories
            var products = await GetCollectionAsync<Product>("/api/Collection/Products?maxAmount=10");
            ShowCollection(products, "Products");

            Console.ReadKey();
        }

        static async Task<IList<T>> GetCollectionAsync<T>(string path)
        {
            List<T> result = null;
            HttpResponseMessage response = await _client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                result = await response.Content.ReadAsAsync<List<T>>();
            }
            return result;
        }

        private static void ShowCollection<T>(IList<T> collection, string title)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(title);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("--------------------------------------------");

            var t = typeof(T);
            var columns = t.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (T item in collection)
            {
                foreach (var prop in columns)
                {
                    if(prop.PropertyType.Name.Equals("Int32") || prop.PropertyType.Name.Equals("String"))
                        Console.WriteLine(prop.GetValue(item));
                }
                Console.WriteLine("--------------------------------------------");
            }
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
