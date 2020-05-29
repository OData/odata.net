using ProductService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Container cc = new Container(new Uri("https://localhost:44342/"));
            cc.Timeout = 650;

            var dd =  await cc.ProductDetails.ExecuteAsync();

            Console.WriteLine("dd");
        }
    }
}
