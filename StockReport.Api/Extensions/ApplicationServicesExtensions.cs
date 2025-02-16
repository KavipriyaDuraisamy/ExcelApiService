
using StockReport.Application;
using StockReport.Infrastructure;

namespace StockReport.Extensions;
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<StockReportRepository, StockReportService>();
            return services;
            
        }
    }
