using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Order.Application.Contracts.Infastructure;
using Order.Application.Contracts.Persistence;
using Order.Application.Models;
using Order.Infrastructure.Mail;
using Order.Infrastructure.Persistence;
using Order.Infrastructure.Repositories;

namespace Order.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<OrderContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("OrderingConnectionString")));

            services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
            services.AddScoped<IOrderRepository, OrderRepository>();

            services.Configure<EmailSettings>(c => {
                var emailSettings = configuration.GetSection("EmailSettings");
                c.ApiKey = emailSettings["ApiKey"];
                c.FromAddress = emailSettings["FromAddress"];
                c.FromName = emailSettings["FromName"];
            });
            services.AddTransient<IEmailService, EmailService>();

            return services;
        }
    }
}
