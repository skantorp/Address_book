using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataBase;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Address_Book
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SetupDB();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        public static void SetupDB()
        {
            var dataBase = new AddressDataBase();

            if (!dataBase.CheckDbExists())
            {
                dataBase.SetUpDb();
            }
        }
    }
}
