﻿using Dasync.EETypes;
using Dasync.EETypes.Communication;
using Dasync.EETypes.Descriptors;
using Dasync.EETypes.Intents;
using Dasync.ValueContainer;

namespace Dasync.ExecutionEngine.Utils
{
    internal static class InvocationDataUtils
    {
        private static readonly IValueContainerProxyFactory TaskResultProxyFactory = ValueContainerFactory.GetProxyFactory(typeof(TaskResult));

        public static MethodInvocationData CreateMethodInvocationData(ExecuteRoutineIntent intent, ITransitionContext context)
        {
            return new MethodInvocationData
            {
                IntentId = intent.Id,
                Service = intent.Service,
                Method = intent.Method,
                Parameters = intent.Parameters,
                Continuation = intent.Continuation,
                Caller = context?.Caller,
                FlowContext = context?.FlowContext
            };
        }

        public static MethodContinuationData CreateMethodContinuationData(ContinueRoutineIntent intent, ITransitionContext context)
        {
            return new MethodContinuationData
            {
                IntentId = intent.Id,
                ContinueAt = intent.ContinueAt,
                Service = intent.Service,
                Method = intent.Method,
                TaskId = intent.TaskId,
                Caller = context.CurrentAsCaller(),
                Result = TaskResultProxyFactory.Create(intent.Result)
            };
        }
    }
}
