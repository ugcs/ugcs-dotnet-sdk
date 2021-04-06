using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using UGCS.Sdk.Protocol.Encoding;

namespace UgCS.SDK.Examples.Common
{
    public static class TelemetryUtils
    {
        public sealed class WellKnownTelemetry
        {
            private readonly string _code;
            private readonly Semantic _semantic;
            private readonly Subsystem _subsystem;


            public static readonly WellKnownTelemetry Latitude = new WellKnownTelemetry(
                "latitude", Semantic.S_LATITUDE, Subsystem.S_FLIGHT_CONTROLLER);
            public static readonly WellKnownTelemetry Longitude = new WellKnownTelemetry(
                "longitude", Semantic.S_LONGITUDE, Subsystem.S_FLIGHT_CONTROLLER);
            public static readonly WellKnownTelemetry Heading = new WellKnownTelemetry(
                "heading", Semantic.S_HEADING, Subsystem.S_FLIGHT_CONTROLLER);
            public static readonly WellKnownTelemetry AltitudeRaw = new WellKnownTelemetry(
                "altitude_raw", Semantic.S_ALTITUDE_RAW, Subsystem.S_FLIGHT_CONTROLLER);


            private WellKnownTelemetry(string code, Semantic semantic, Subsystem subsystem)
            {
                this._code = code;
                this._semantic = semantic;
                this._subsystem = subsystem;
            }

            public bool Is(TelemetryField f)
            {
                // To simplify code we skip subsystemId believing that the vehicle has
                // only one instance of each subsystem.
                return f.CodeSpecified && f.Code == _code
                    && f.SemanticSpecified && f.Semantic == _semantic
                    && f.SubsystemSpecified && f.Subsystem == _subsystem;
            }

        }

        public sealed class TlmValue<T>
            where T : struct
        {
            private readonly T _value;


            public T Value
            {
                get
                {
                    if (IsUnknown)
                    {
                        throw new InvalidOperationException("Value is unknown.");
                    }
                    return _value;
                }
            }

            public bool IsUnknown { get; }

            private TlmValue(T value)
            {
                _value = value;
                IsUnknown = false;
            }

            private TlmValue()
            {
                IsUnknown = true;
            }


            public static TlmValue<T> CreateKnown(T value)
            {
                return new TlmValue<T>(value);
            }

            public static TlmValue<T> CreateUnknown()
            {
                return new TlmValue<T>();
            }

            public static implicit operator T? (TlmValue<T> v)
            {
                if (v.IsUnknown)
                    return null;
                else
                    return v.Value;
            }
        }


        public static bool Is(this TelemetryField f, WellKnownTelemetry wf)
        {
            if (f == null)
                return false;
            return wf.Is(f);
        }

        public static bool Contains(this List<Telemetry> telemetry, WellKnownTelemetry field,
            out TlmValue<double> value)
        {
            Telemetry firstFound = telemetry.FirstOrDefault(x => x.TelemetryField.Is(field));
            if (firstFound == null)
            {
                value = null;
                return false;
            }

            if (firstFound.Value == null)
            {
                value = TlmValue<double>.CreateUnknown();
            }
            else
            {
                value = TlmValue<double>.CreateKnown(
                    getNumeric(firstFound.Value));
            }
            return true;
        }

        private static double getNumeric(Value v)
        {
            if (v.DoubleValueSpecified)
                return v.DoubleValue;
            else if (v.FloatValueSpecified)
                return v.FloatValue;
            else if (v.LongValueSpecified)
                return v.LongValue;
            else if (v.IntValueSpecified)
                return v.IntValue;
            else
                throw new ArgumentException("It's not a number.");
        }
    }
}
