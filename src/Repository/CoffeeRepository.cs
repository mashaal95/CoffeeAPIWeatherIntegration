namespace CoffeeAPIMinimal.Repository
{
    public class CoffeeRepository : ICoffeeRepository
    {
        private readonly IConfiguration _config;

        private readonly string city = "Melbourne";

        public CoffeeRepository(IConfiguration config) =>
            // For the retrieval of the OpenWeatherAPI key
            _config = config;

        public async Task<string> UpdateCount(string key, IDistributedCache cache)
        {
            int currentCount = await GetCount(key, cache);
            currentCount++;
            await SetCount(key, cache, currentCount);

            return currentCount.ToString();
        }

        public async Task<int> GetCount(string key, IDistributedCache cache)
        {
            string response = await cache.GetStringAsync(key);
            response ??= "0";
            int currentCount = int.Parse(response);
            
            return currentCount;
        }

        public async Task SetCount(string key, IDistributedCache cache, int newCount)
        {
            await cache.SetStringAsync(key, newCount.ToString());

        }

        public async Task<object> GetCoffeeAsync(IDistributedCache cache, HttpContext context)
        {
            ////Declaring a key for the Redis cache
            //string recordKey = $"Coffee_{DateTime.Now.ToString("yyyyMMdd_hhmm")}";
            //object coffee = await cache.GetRecordAsync<Coffee>(recordKey);

            // Using the IP address to uniquely identify the user of the API
            string ipAddress = GetIpAddress(context);

            string currentCount = await UpdateCount(ipAddress, cache);
            
            string weather = await GetCurrentWeather();

            object coffee = new();

            coffee = PreparingCoffeeWeatherCheck(coffee, weather);


            if (coffee is not null)
                coffee = AmITeapot(coffee, currentCount, context, DateTime.Now);

            return coffee;
        }

        public async Task<string> GetCurrentWeather()
        {
            // Using the Secret Manager to store and retrieve the API key for the OpenWeatherAPI
            var weatherApiKey = _config["OpenWeatherApi:ServiceApiKey"];
            WeatherClient Client = new WeatherClient(weatherApiKey);

            WeatherModel currentWeather = await Client.GetCurrentWeatherAsync(city, Weather.NET.Enums.Measurement.Metric);
            //await cache.SetStringAsync(weatherRedisKey, currentWeather.Main.Temperature.ToString());
            return currentWeather.Main.Temperature.ToString();
        }

        private object AmITeapot(object coffees, string counterString, HttpContext context, DateTime dateTime)
        {
            
                if (!(dateTime.Day == 1 && dateTime.Month == 4))
                {

                return CheckService(coffees, counterString, context);
                }
                else
                {
                    context.Response.StatusCode = 418;
                    return string.Empty;
                }

        }

        private object CheckService(object coffees, string counterString, HttpContext context)
        {
            if (int.Parse(counterString) % 5 == 0)
            {
                context.Response.StatusCode = 503;
                return string.Empty;
            }
            else
            {
                return coffees;
            }
        }


        private Coffee PreparingCoffeeWeatherCheck(object coffees,string weather)
        {
            if (double.Parse(weather) > 30.00)
            {
                coffees = new Coffee
                {
                    Message = "Your refreshing iced coffee is ready",
                    Prepared = DateTimeOffset.Now
                };
            }
            else
            {
                coffees = new Coffee
                {
                    Message = "Your piping hot coffee is ready",
                    Prepared = DateTimeOffset.Now
                };

            }

            return (Coffee)coffees;
        }

        public string GetIpAddress(HttpContext context)
        {
            var remoteIp = context.Connection.RemoteIpAddress;
            string clientIp = "";
            if (remoteIp != null)
            {
                clientIp = remoteIp.ToString();
            }

            return clientIp;
        }

    }
}

