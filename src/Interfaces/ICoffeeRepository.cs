﻿using Microsoft.Extensions.Caching.Distributed;

namespace CoffeeAPIMinimal.Interfaces
{
    public interface ICoffeeRepository
    {
        Task<object> GetCoffeeAsync(IDistributedCache cache, HttpContext context);

        Task<string> CounterAsync(string ipAddress, IDistributedCache cache);

        string GetIpAddress(HttpContext context);
    }
}
