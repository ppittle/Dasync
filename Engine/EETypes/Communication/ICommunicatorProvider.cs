﻿namespace Dasync.EETypes.Communication
{
    public interface ICommunicatorProvider
    {
        ICommunicator GetCommunicator(ServiceId serviceId, MethodId methodId, bool assumeExternal = false);

        ICommunicator GetCommunicator(ServiceId serviceId, EventId methodId);
    }
}
