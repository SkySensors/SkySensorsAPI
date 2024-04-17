using System.Net.NetworkInformation;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SkySensorsAPI.Utilities;

public class PhysicalAddressConverter : JsonConverter<PhysicalAddress>
{
	public override PhysicalAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.String)
		{
			string addressString = reader.GetString();
			try
			{
				return PhysicalAddress.Parse(addressString);
			}
			catch (FormatException)
			{
				throw new JsonException($"Invalid MAC address format: {addressString}");
			}
		}

		throw new JsonException($"Unexpected token type: {reader.TokenType}");
	}

	public override void Write(Utf8JsonWriter writer, PhysicalAddress value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(BitConverter.ToString(value.GetAddressBytes()));
	}
}
