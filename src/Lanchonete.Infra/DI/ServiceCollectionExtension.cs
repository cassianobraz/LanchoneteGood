using Lanchonete.Application.Cardapio.Dtos.Requests;
using Lanchonete.Domain.Models.CardapioAggregate;
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
    }
    #endregion

    private static void AddHandlers(this IServiceCollection services)
    {
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<CardapioRequestDto>());
    }
}