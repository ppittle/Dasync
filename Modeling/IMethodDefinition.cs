﻿using System.Collections.Generic;
using System.Reflection;

namespace Dasync.Modeling
{
    public interface IMethodDefinition : IPropertyBag
    {
        IServiceDefinition Service { get; }

        MethodInfo MethodInfo { get; }

        /// <summary>
        /// Mapping of the <see cref="MethodInfo"/> to methods implemented by interface(s) of the service.
        /// Not applicable to <see cref="ServiceType.External"/>.
        /// </summary>
        MethodInfo[] InterfaceMethods { get; }

        /// <summary>
        /// Tells is a method is part of a service contract and can be executed in a reliable way.
        /// </summary>
        bool IsRoutine { get; }

        /// <summary>
        /// Tells if the method is 'read-only' and does not modify any data.
        /// </summary>
        bool IsQuery { get; }
    }
}
