﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dasync.Collections;
using Dasync.EETypes.Communication;
using Dasync.EETypes.Engine;
using Dasync.Serialization;

namespace Dasync.Communication.InMemory
{
    public interface IMessageHandler
    {
        Task Run(CancellationToken stopToken);
    }

    public class InMemoryMessageHandler : IMessageHandler
    {
        private readonly IMessageHub _messageHub;
        private readonly ILocalMethodRunner _localTransitionRunner;
        private readonly ISerializerProvider _serializerProvider;

        public InMemoryMessageHandler(
            IMessageHub messageHub,
            ILocalMethodRunner localTransitionRunner,
            ISerializerProvider serializerProvider)
        {
            _messageHub = messageHub;
            _localTransitionRunner = localTransitionRunner;
            _serializerProvider = serializerProvider;
        }

        public async Task Run(CancellationToken stopToken)
        {
            try
            {
                await StreamMessages(stopToken).ParallelForEachAsync(HandleMessage);
            }
            catch (OperationCanceledException)
            {
            }
        }

        private IAsyncEnumerable<Message> StreamMessages(CancellationToken stopToken) =>
            new AsyncEnumerable<Message>(async yield =>
            {
                while (!stopToken.IsCancellationRequested)
                {
                    try
                    {
                        var message = await _messageHub.GetMessage(stopToken)
                            .ContinueWith(task => task.IsCanceled ? null : task.Result);
                        if (message == null)
                            break;
                        await yield.ReturnAsync(message);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            });

        private async Task HandleMessage(Message message)
        {
            switch (message.Type)
            {
                case MessageType.InvokeMethod:
                    await HandleCommandOrQuery(message);
                    break;

                case MessageType.Response:
                    await HandleResponse(message);
                    break;

                case MessageType.Event:
                    await HandleEvent(message);
                    break;

                default:
                    throw new ArgumentException($"Unknown message type '{message.Type}'.");
            }
        }

        private async Task HandleCommandOrQuery(Message message)
        {
            var invocationData = new MethodInvocationData(message, _serializerProvider);
            var communicationMessage = new CommunicationMessage(message);
            var continuationState = TryGetMethodContinuationState(message);
            var result = await _localTransitionRunner.RunAsync(invocationData, communicationMessage, continuationState);

            if (message.Data.TryGetValue("Notification", out var sink) &&
                sink is TaskCompletionSource<InvokeRoutineResult> tcs)
            {
                tcs.TrySetResult(result);
            }
        }

        private async Task HandleResponse(Message message)
        {
            var continuationData = new MethodContinuationData(message);
            continuationData.SerializedResult = (string)message.Data["Result"];
            continuationData.Serializer = _serializerProvider.GetSerializer((string)message.Data["ContentType"]);
            var communicationMessage = new CommunicationMessage(message);
            var continuationState = TryGetMethodContinuationState(message);
            await _localTransitionRunner.ContinueAsync(continuationData, communicationMessage, continuationState);
        }

        private async Task HandleEvent(Message message)
        {
            throw new NotImplementedException();
        }

        private SerializedMethodContinuationState TryGetMethodContinuationState(Message message)
        {
            if (message.Data.TryGetValue("Continuation:State", out var stateObj) && stateObj is byte[] state && state.Length > 0)
            {
                return new SerializedMethodContinuationState
                {
                    State = state,
                    ContentType =
                        message.Data.TryGetValue("Continuation:ContentType", out var contentTypeObj) && contentTypeObj is string contentType
                        ? contentType
                        : null
                };
            }

            return null;
        }
    }
}
