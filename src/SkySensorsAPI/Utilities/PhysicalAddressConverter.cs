using System.Net.NetworkInformation;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SkySensorsAPI.Utilities;

public class PhysicalAddressConverter : JsonConverter<PhysicalAddress>
{
	/// <summary>
	/// Extenstion used to read PhysicalAddress from json field
	/// </summary>
	public override PhysicalAddress Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType == JsonTokenType.String)
		{
			string addressString = reader.GetString() ?? string.Empty;
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

	/// <summary>
	/// Used to write PhysicalAddress to json string
	/// </summary>
	public override void Write(Utf8JsonWriter writer, PhysicalAddress value, JsonSerializerOptions options)
	{
		writer.WriteStringValue(BitConverter.ToString(value.GetAddressBytes()));
	}
}
