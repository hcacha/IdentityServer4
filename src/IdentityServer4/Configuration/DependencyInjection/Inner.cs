// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;

namespace IdentityServer4.Configuration.DependencyInjection
{
    public class Inner<TService>
    {
        public TService Instance { get; set; }

        public Inner(TService instance)
        {
            Instance = instance;
        }
    }

    public class Inner<TService, TImpl> : Inner<TService>
        where TImpl : class, TService
    {
        public Inner(TImpl instance) : base(instance)
        {
        }
    }

    public class DisposableInner<TService> : Inner<TService>, IDisposable
    {
        public DisposableInner(TService instance) : base(instance)
        {
        }

        public void Dispose()
        {
            (Instance as IDisposable)?.Dispose();
        }
    }
}
