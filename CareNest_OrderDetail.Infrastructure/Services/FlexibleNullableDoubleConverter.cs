using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CareNest_OrderDetail.Infrastructure.Services
{
    public class FlexibleNullableDoubleConverter : JsonConverter<double?>
    {
        public override double? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetDouble(out double number))
                {
                    return number;
                }
                return null;
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                string? stringValue = reader.GetString();
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    return null;
                }
                if (double.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double parsed))
                {
                    return parsed;
                }
                return null;
            }

            // Fallback: skip unsupported types
            reader.Skip();
            return null;
        }

        public override void Write(Utf8JsonWriter writer, double? value, JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteNumberValue(value.Value);
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}


