﻿using System;
using System.Data.Common;
using System.Globalization;

namespace ClickHouse.Ado
{
    public class ClickHouseConnectionStringBuilder : DbConnectionStringBuilder
    {
        public ClickHouseConnectionStringBuilder()
        {
        }

        public string Database
        {
            get => TryGetValue("Database", out var value) ? value as string : "default";
            set => this["Database"] = value;
        }

        public string User
        {
            get => TryGetValue("User", out var value) ? value as string : "default";
            set => this["User"] = value;
        }

        public string Password
        {
            get => TryGetValue("Password", out var value) ? value as string : string.Empty;
            set => this["Password"] = value;
        }

        public string Host
        {
            get => TryGetValue("Host", out var value) ? value as string : "localhost";
            set => this["Host"] = value;
        }

        public bool Compression
        {
            get => TryGetValue("Compression", out var value) ? "true".Equals(value as string, StringComparison.OrdinalIgnoreCase) : false;
            set => this["Compression"] = value;
        }

        public bool UseSession
        {
            get => TryGetValue("UseSession", out var value) ? "true".Equals(value as string, StringComparison.OrdinalIgnoreCase) : false;
            set => this["UseSession"] = value;
        }

        public string SessionId
        {
            get => TryGetValue("SessionId", out var value) ? value as string : null;
            set => this["SessionId"] = value;
        }

        public ushort Port
        {
            get
            {
                if (TryGetValue("Port", out var value) && value is string @string && ushort.TryParse(@string, out var @ushort))
                {
                    return @ushort;
                }

                return 8123;
            }
            set => this["Port"] = value;
        }

        public TimeSpan Timeout
        {
            get
            {
                if (TryGetValue("Timeout", out var value) && value is string @string && double.TryParse(@string, NumberStyles.Any, CultureInfo.InvariantCulture, out var timeout))
                {
                    return TimeSpan.FromSeconds(timeout);
                }

                return TimeSpan.FromMinutes(2);
            }
            set => this["Timeout"] = value.TotalSeconds;
        }
    }
}
