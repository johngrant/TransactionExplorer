using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApi.Converters;

/// <summary>
/// JSON converter for DateOnly to ensure it serializes as date-only string (YYYY-MM-DD)
/// instead of datetime with time component
/// </summary>
public class DateOnlyJsonConverter : JsonConverter<DateOnly>
{
    private const string DateFormat = "yyyy-MM-dd";

    public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var dateString = reader.GetString();
            if (DateOnly.TryParse(dateString, out var date))
            {
                return date;
            }
        }

        throw new JsonException($"Unable to convert \"{reader.GetString()}\" to DateOnly.");
    }

    public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateFormat));
    }
}
