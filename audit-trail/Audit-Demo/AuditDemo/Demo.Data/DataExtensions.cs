using Demo.Audit;
using Demo.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Cfg.MappingSchema;
using NHibernate.Dialect;
using NHibernate.Event;
using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Data
{
    public static class DataExtensions
    {
        public static IServiceCollection AddNHibernate(this IServiceCollection services, string connectionString)
        {
            var mapper = new ModelMapper();
            mapper.AddMappings(typeof(DataExtensions).Assembly.ExportedTypes);
            HbmMapping domainMapping = mapper.CompileMappingForAllExplicitlyAddedEntities();

            var configuration = new Configuration();
            configuration.DataBaseIntegration(c =>
            {
                c.Dialect<MsSql2012Dialect>();
                c.ConnectionString = connectionString;
                c.KeywordsAutoImport = Hbm2DDLKeyWords.AutoQuote;
                c.SchemaAction = SchemaAutoAction.Validate;
                c.LogFormattedSql = true;
                c.LogSqlInConsole = true;
            });

            configuration.AddMapping(domainMapping);

            //var stack = new IPostUpdateEventListener[] { new DummyPostUpdateEventListener() };
            //configuration.EventListeners.PostUpdateEventListeners = stack;

            //var stack = new IPreUpdateEventListener[] { new DummyPreUpdateEventListener() };
            //configuration.EventListeners.PreUpdateEventListeners = stack;

            //var stack = new IFlushEventListener[] { new DummyFlushEventListener() };
            //configuration.EventListeners.FlushEventListeners = stack;

            //var stack = new IDirtyCheckEventListener[] { new DummyDirtyCheckEventListener() };
            //configuration.EventListeners.DirtyCheckEventListeners = stack;

            //TODO: figure out DI path for this
            //var stack = new IFlushEntityEventListener[] { new DummyFlushEntityEventListener() };
            //configuration.EventListeners.FlushEntityEventListeners = stack;

            services.AddSingleton<NHibernate.Cfg.Configuration>(configuration);
            services.AddScoped<IInterceptor, SqlLoggingInterceptor>();

            //var sessionFactory = configuration.BuildSessionFactory();            
            //services.AddSingleton(sessionFactory);
            //services.AddScoped(factory => sessionFactory.OpenSession());

            services.AddSingleton<ISessionFactory>((provider) =>
            {
                var nberConfig = provider.GetService<NHibernate.Cfg.Configuration>();
                var logger = provider.GetService<ILogger<DummyPostUpdateEventListener>>();
                var auditService = provider.GetService<IAuditService>();
                var stack = new IPostUpdateEventListener[] { new DummyPostUpdateEventListener(logger, auditService, nberConfig) };
                nberConfig.EventListeners.PostUpdateEventListeners = stack;

                return nberConfig.BuildSessionFactory();
            });

            services.AddScoped(serviceProvider =>
            {
                var interceptor = serviceProvider.GetService<IInterceptor>();
                var sessionFactory = serviceProvider.GetService<ISessionFactory>();
                return sessionFactory.OpenSession()
                                     .SessionWithOptions()
                                     .Interceptor(interceptor)
                                     .OpenSession();
            });
            services.AddScoped<IMapperSession, NHibernateMapperSession>();

            return services;
        }
    }
}
