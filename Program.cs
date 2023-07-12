using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;


var host = new HostBuilder().ConfigureAppConfiguration(builder =>
        {
            String cs ="Endpoint=https://datakubeconfig.azconfig.io;Id=O/Fv;Secret=0W5LzEbGvm9FNMKyWVK7TOyZUT0IYRUwGi3Nes2eA0Y=";
            builder.AddAzureAppConfiguration(cs);
        })
    .ConfigureFunctionsWorkerDefaults()
    .Build();

host.Run();
