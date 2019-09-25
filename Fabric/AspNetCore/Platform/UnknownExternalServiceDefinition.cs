﻿using System;
using System.Collections.Generic;
using Dasync.Modeling;

namespace Dasync.AspNetCore.Platform
{
    internal class UnknownExternalServiceDefinition : PropertyBag, IServiceDefinition
    {
        public UnknownExternalServiceDefinition(string name) : this(name, null) { }

        public UnknownExternalServiceDefinition(string name, ICommunicationModel model)
        {
            Name = name;
            Model = model;
        }

        public ICommunicationModel Model { get; }

        public string Name { get; }

        public string[] AlternateNames { get; } = Array.Empty<string>();

        public ServiceType Type => ServiceType.External;

        public Type[] Interfaces => null;

        public Type Implementation => null;

        public IEnumerable<IMethodDefinition> Methods { get; } = Array.Empty<IMethodDefinition>();

        public IEnumerable<IEventDefinition> Events { get; } = Array.Empty<IEventDefinition>();

        public IMethodDefinition FindMethod(string methodName) => null;

        public IEventDefinition FindEvent(string eventName) => null;
    }
}
