using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nether.Web.Features.Analytics
{
    public class EndpointInfo
    {
        public string KeyName { get; set; }
        public string AccessKey { get; set; }
        public string Resource { get; set; }
        public TimeSpan Ttl { get; set; }
    }

    public static class EndpointInfoServicesExtensions
    {
        public static IServiceCollection AddEndpointInfo(this IServiceCollection services, IConfiguration configuration, string key)
        {
            var endpointInfo = GetEndpointInfo(configuration.GetSection(key));
            services.AddSingleton(endpointInfo);
            return services;
        }
        public static EndpointInfo GetEndpointInfo(IConfiguration configuration)
        {
            return new EndpointInfo
            {
                KeyName = configuration["KeyName"],
                AccessKey = configuration["AccessKey"],
                Resource = configuration["Resource"],
                Ttl = TimeSpan.Parse(configuration["Ttl"])
            };
        }
    }
}
