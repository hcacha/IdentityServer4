// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using IdentityServer4.Configuration.DependencyInjection;
using System;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void DecorateSingleton<TService, TImpl>(this IServiceCollection services)
                    where TService : class
                    where TImpl : class, TService
        {
            services.Decorate<TService>();
            services.AddSingleton<TService, TImpl>();
        }

        public static void DecorateSingleton<TService, TImpl>(this IServiceCollection services, TService instance)
            where TService : class
            where TImpl : class, TService
        {
            services.Decorate<TService>();
            services.AddSingleton<TService>(instance);
        }

        public static void DecorateScoped<TService, TImpl>(this IServiceCollection services)
            where TService : class
            where TImpl : class, TService
        {
            services.Decorate<TService>();
            services.AddScoped<TService, TImpl>();
        }

        public static void DecorateTransient<TService, TImpl>(this IServiceCollection services)
            where TService : class
            where TImpl : class, TService
        {
            services.Decorate<TService>();
            services.AddTransient<TService, TImpl>();
        }

        public static void Decorate<TService>(this IServiceCollection services)
        {
            var registration = services.FirstOrDefault(x => x.ServiceType == typeof(TService));
            if (registration != null)
            {
                if (registration.ServiceType == null)
                {
                    throw new InvalidOperationException("Decorator requires a service type.");
                }
                if (services.Any(x => x.ServiceType == typeof(Inner<TService>)))
                {
                    throw new InvalidOperationException("Decorator already registered for type: " + typeof(TService).Name + " .");
                }

                services.Remove(registration);

                if (registration.ImplementationInstance != null)
                {
                    var type = typeof(Inner<,>).MakeGenericType(typeof(TService), registration.ImplementationInstance.GetType());
                    services.Add(new ServiceDescriptor(typeof(Inner<TService>), type, ServiceLifetime.Transient));
                    services.Add(new ServiceDescriptor(registration.ImplementationInstance.GetType(),
                        registration.ImplementationInstance));
                }
                else if (registration.ImplementationFactory != null)
                {
                    services.Add(new ServiceDescriptor(typeof(Inner<TService>), provider =>
                    {
                        return new DisposableInner<TService>((TService)registration.ImplementationFactory(provider));
                    }, registration.Lifetime));
                }
                else
                {
                    var type = typeof(Inner<,>).MakeGenericType(typeof(TService), registration.ImplementationType);
                    services.Add(new ServiceDescriptor(typeof(Inner<TService>), type, ServiceLifetime.Transient));
                    services.Add(new ServiceDescriptor(registration.ImplementationType, registration.ImplementationType, registration.Lifetime));
                }
            }
        }
    }
}
