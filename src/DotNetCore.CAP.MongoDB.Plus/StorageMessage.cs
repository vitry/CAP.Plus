﻿// Copyright (c) .NET Core Community. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.

using System;

namespace DotNetCore.CAP.MongoDB
{
    internal class ReceivedMessage
    {
        public long Id { get; set; }

        public string Version { get; set; }

        public string Group { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public DateTime Added { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public DateTime? NextRetryAt { get; set; }

        public DateTime LastUpdated { get; set; }

        public int Retries { get; set; }

        public string StatusName { get; set; }
    }

    internal class PublishedMessage
    {
        public long Id { get; set; }

        public string Version { get; set; }

        public string Name { get; set; }

        public string Content { get; set; }

        public DateTime Added { get; set; }

        public DateTime? ExpiresAt { get; set; }

        public DateTime? NextRetryAt { get; set; }

        public DateTime LastUpdated { get; set; }

        public int Retries { get; set; }

        public string StatusName { get; set; }
    }
}