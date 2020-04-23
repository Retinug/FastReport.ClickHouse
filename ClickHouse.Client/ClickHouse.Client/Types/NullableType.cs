﻿using System;
using ClickHouse.Client.Utility;

namespace ClickHouse.Client.Types
{
    internal class NullableType : ParameterizedType
    {
        public override ClickHouseTypeCode TypeCode => ClickHouseTypeCode.Nullable;

        public ClickHouseType UnderlyingType { get; set; }

        public override Type FrameworkType
        {
            get
            {
                var underlyingFrameworkType = UnderlyingType.FrameworkType;
                return underlyingFrameworkType.IsValueType ? typeof(Nullable<>).MakeGenericType(underlyingFrameworkType) : underlyingFrameworkType;
            }
        }

        public override string Name => "Nullable";

        public override ParameterizedType Parse(string typeName, Func<string, ClickHouseType> typeResolverFunc)
        {
            if (!typeName.StartsWith(Name))
            {
                throw new ArgumentException(nameof(typeName));
            }

            return new NullableType
            {
                UnderlyingType = typeResolverFunc(typeName.Substring(Name.Length).TrimRoundBrackets()),
            };
        }

        public override string ToString() => $"{Name}({UnderlyingType.ToString()})";
    }
}
