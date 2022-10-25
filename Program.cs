using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.IO;

namespace RedisConsoleApp
{
    class Program
    {
        private static IConfigurationRoot _config;

        private static Lazy<ConnectionMultiplexer> lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
        {
            string cacheConnection = _config["CacheConnection"];
            return ConnectionMultiplexer.Connect(cacheConnection);
        });

        public static ConnectionMultiplexer Connection
        {
            get
            {
                return lazyConnection.Value;
            }
        }

        private static void CreateConfig()
        {
            var builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            _config = builder.Build();
        }

        static void Main(string[] args)
        {
            try
            {
                CreateConfig();

                IDatabase cache = lazyConnection.Value.GetDatabase();

                string cacheCommand = "PING";
                Console.WriteLine("Cache command: " + cacheCommand);
                Console.WriteLine("Response: " + cache.Execute(cacheCommand).ToString());

                cacheCommand = "GET Message";
                Console.WriteLine("Cache command: " + cacheCommand + " or StringGet()");
                Console.WriteLine("Response: " + cache.StringGet("Message").ToString());

                cacheCommand = "SET Message \"Hello! I can get the cache working from this console app :)\"";
                Console.WriteLine("Cache command: " + cacheCommand + " or StringGet()");
                Console.WriteLine("Response: " + cache.StringSet("Message", "Hello! I can get the cache working from this console app :)").ToString());

                cacheCommand = "GET Message";
                Console.WriteLine("Cache command: " + cacheCommand + " or StringGet()");
                Console.WriteLine("Response: " + cache.StringGet("Message").ToString());

                cacheCommand = "CLIENT LIST";
                Console.WriteLine("Cache command: " + cacheCommand);
                Console.WriteLine("Response: " + cache.Execute("CLIENT", "LIST").ToString().Replace("id=", "id="));

                //ToDoItem toDoItem = new ToDoItem("1", "This is a new incoming item!", DateTime.UtcNow);
                //Console.WriteLine("Response: " + cache.StringSet("1", JsonConvert.SerializeObject(toDoItem)));

                //ToDoItem itemFromCache = JsonConvert.DeserializeObject<ToDoItem>(cache.StringGet("1"));
                //Console.WriteLine("Item Id: " + itemFromCache.Id);
                //Console.WriteLine("Item Description: " + itemFromCache.Description);
                //Console.WriteLine("Item Created: " + itemFromCache.ItemCreated);

                lazyConnection.Value.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception thrown: {ex.Message}: {ex.StackTrace}");
                throw;
            }
        }
    }
}