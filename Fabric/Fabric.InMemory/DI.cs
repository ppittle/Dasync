﻿using System.Collections.Generic;
using Dasync.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace Dasync.Fabric.InMemory
{
    public static class DI
    {
        public static readonly IEnumerable<ServiceDescriptor> Bindings = new ServiceDescriptorList().Configure();

        public static IServiceCollection Configure(this IServiceCollection services)
        {
            Dasync.Communication.InMemory.DI.Configure(services);
            Dasync.Persistence.InMemory.DI.Configure(services);
            return services;
        }
    }
}
