using Lanchonete.Application.Cardapio.Dtos.Requests;
using Lanchonete.Application.Shared.Helper;
using Lanchonete.Domain.Models.CardapioAggregate;
using Lanchonete.Domain.Models.PedidoAggregate;
using Lanchonete.Domain.Service;
using Lanchonete.Domain.Service.Interfaces;
using Lanchonete.Domain.Shared.interfaces;
using Lanchonete.Infra.EF.DbCOntext;
using Lanchonete.Infra.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SistemaControle.Infra.DI;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddServiceCollection(this IServiceCollection services, IConfiguration configuration)
    {
        AddDbContextConfig(services, configuration);
        AddRepositories(services);
        AddServices(services);
        AddHandlers(services);
        return services;
    }

    private static void AddDbContextConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LanchoneteContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                x => x.EnableRetryOnFailure()));
    }

    #region Repositorios
    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ICardapioRepository, CardapioRepository>();
        services.AddScoped<IPedidoRepository, PedidoRepository>();
    }
    #endregion

    #region Services
    private static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IMemoryCacheHelper, MemoryCacheHelper>();
        services.AddScoped<IPedidoService, PedidoService>();
    }
    #endregion

    private static void AddHandlers(this IServiceCollection services)
    {
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<CardapioRequestDto>());
    }
}