﻿using System.Threading;
using System.Threading.Tasks;
using Dasync.EETypes.Ioc;
using Dasync.Modeling;
using Microsoft.Extensions.Hosting;

namespace DasyncAspNetCore
{
    public class DasyncCoHost : IHostedService
    {
        private readonly ICommunicationModel _communicationModel;
        private readonly IDomainServiceProvider _domainServiceProvider;

        public DasyncCoHost(
            ICommunicationModel communicationModel,
            IDomainServiceProvider domainServiceProvider)
        {
            _communicationModel = communicationModel;
            _domainServiceProvider = domainServiceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            ResolveAllDomainServices();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            return;
        }

        /// <summary>
        /// Makes sure that every single domain service is valid, and the resolution also triggers event subscription in services' constructors.
        /// </summary>
        private void ResolveAllDomainServices()
        {
            foreach (var serviceDefinition in _communicationModel.Services)
            {
                if (serviceDefinition.Implementation != null)
                {
                    _domainServiceProvider.GetService(serviceDefinition.Implementation);
                }

                if (serviceDefinition.Interfaces?.Length > 0)
                {
                    foreach (var interfaceType in serviceDefinition.Interfaces)
                    {
                        _domainServiceProvider.GetService(interfaceType);
                    }
                }
            }
        }
    }
}
