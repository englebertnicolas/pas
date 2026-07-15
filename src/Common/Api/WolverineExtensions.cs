using System.Reflection;
using JasperFx;
using JasperFx.CodeGeneration;
using Microsoft.Extensions.DependencyInjection;
using PAS.Common.Domain;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Configuration;
using Wolverine.EntityFrameworkCore;
using Wolverine.ErrorHandling;
using Wolverine.FluentValidation;
using Wolverine.RabbitMQ;
using Wolverine.Runtime.Handlers;
using Wolverine.SqlServer;

namespace PAS.Common.Api;

public static class WolverineExtensions {

    public static IServiceCollection AddDefaultWolverine(this IServiceCollection services, string dbCnc, string dbSchemaName, 
        string rabbitMqCnc, bool autoProvisionRabbitMq = false, Assembly[]? discoveryAssemblies = null, 
        Action<WolverineOptions>? configure = null) {

        return services.AddWolverine(options => {
            // Scans external assemblies to discover message handlers, policies and validators
            if (discoveryAssemblies != null)
                foreach (var assembly in discoveryAssemblies) 
                    options.Discovery.IncludeAssembly(assembly);

            // Configures Microsoft SQL Server as both the backing store for Wolverine's database-backed message queues
            // and the transactional inbox/outbox storage
            options.UseSqlServerPersistenceAndTransport(dbCnc, dbSchemaName);

            // Integrates Entity Framework Core into Wolverine's execution pipeline,
            // allowing EF Core DbContexts to coordinate with Wolverine transactions
            options.UseEntityFrameworkCoreTransactions();

            // Automatically discovers and injects transactional middleware (and triggers SaveChangesAsync)
            // for handlers interacting with EF DbContexts
            //options.Policies.AutoApplyTransactions();
            options.Policies.Add<AutoApplyTransactionToCommands>();

            // Instructs Wolverine to automatically intercept, extract, and publish domain events
            // from tracked EF entities (that inherit from 'Entity') before committing changes
            options.PublishDomainEventsFromEntityFrameworkCore<Entity>(x => x.DomainEvents);

            // Plugs FluentValidation into the execution pipeline to automatically validate
            // incoming messages before they reach their respective handlers
            options.UseFluentValidation();

            // Configures RabbitMQ as an external message broker transport layer
            var rabbitMqOptions = options.UseRabbitMq(new Uri(rabbitMqCnc))
                // Automatically maps messages to RabbitMQ exchanges/queues based on naming conventions
                // (rather than manual registration)
                .UseConventionalRouting(conRoutingOptions => {
                    // Restricts conventional routing so that only message types
                    // whose names end with "IntegrationEvent" are routed out to RabbitMQ
                    conRoutingOptions.IncludeTypes(type => type.Name.EndsWith("IntegrationEvent"));
                });

            if (autoProvisionRabbitMq) {
                // Forces Wolverine to automatically create missing RabbitMQ queues, exchanges, and bindings at startup
                rabbitMqOptions.AutoProvision();
            }

            // Ensures guaranteed delivery by enforcing a Transactional Outbox pattern
            // for all outgoing messages (persisting them in SQL Server before sending)
            options.Policies.UseDurableOutboxOnAllSendingEndpoints();

            // Ensures resilient processing by enforcing a Transactional Inbox pattern
            // for all incoming listeners (persisting incoming messages to handle failures/retries safely)
            options.Policies.UseDurableInboxOnAllListeners();

            // Automatically executes Wolverine database schema migrations at application startup
            options.UseEntityFrameworkCoreWolverineManagedMigrations();

            // Configures retry policy
            options.Policies.OnException(ex => ex is TimeoutException || ex is HttpRequestException)
                .RetryWithCooldown(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5))
                .Then.ScheduleRetry(TimeSpan.FromHours(1), TimeSpan.FromHours(23));

            configure?.Invoke(options);
        });
    }

    class AutoApplyTransactionToCommands : IHandlerPolicy {
        public void Apply(IReadOnlyList<HandlerChain> chains, GenerationRules rules, IServiceContainer container) {
            var commandChains = chains.Where(chain => !chain.MessageType.Name.EndsWith("Query"));
            foreach (var chain in commandChains)
                new TransactionalAttribute().Modify(chain, rules, container);
        }
    }
}
