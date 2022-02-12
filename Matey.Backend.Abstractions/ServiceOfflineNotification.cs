﻿using System.Collections.Immutable;

namespace Matey.Backend.Abstractions
{
    public record ServiceOfflineNotification(
        string Provider,
        string ServiceName,
        ImmutableArray<string> Backends) : IServiceNotification;
}