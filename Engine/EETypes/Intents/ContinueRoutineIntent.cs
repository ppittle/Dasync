﻿using System;
using System.Runtime.InteropServices;
using Dasync.EETypes.Descriptors;

namespace Dasync.EETypes.Intents
{
    [StructLayout(LayoutKind.Sequential)]
    public sealed class ContinueRoutineIntent
    {
        public string Id;

        public ServiceId ServiceId;

        public RoutineDescriptor Routine;

        public DateTimeOffset? ContinueAt;

        /// <summary>
        /// The <see cref="ContinuationDescriptor.Id"/> for awaited routine, which will be
        /// used to correlate serialized proxy tasks with <see cref="Result"/>.
        /// </summary>
        public string TaskId;

        /// <summary>
        /// The result of the awaited routine. 
        /// </summary>
        public TaskResult Result;

#warning Add state of the actual routine being resumed? That option would remove the need of persistant storage for the state - eveything is conveyed in messages. However, that can blow the size of a message - need overflow mechanism.
    }
}
