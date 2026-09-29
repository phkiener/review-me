using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace ReviewMe.Config;

/// <summary>
/// A connection string for an LLM.
/// </summary>
/// <param name="Provider">Provider hosting the model; defines the API surface.</param>
/// <param name="Endpoint">URL of the provider.</param>
/// <param name="Model">Name of the model to be used.</param>
/// <param name="ApiKey">API key to use for authentication.</param>
[TypeConverter(typeof(Converter))]
public sealed record ModelConnectionString(string Provider, Uri Endpoint, string Model, string? ApiKey = null)
{
    /// <inheritdoc />
    public override string ToString()
    {
        return $"Provider={Provider};Endpoint={Endpoint};Model={Model}{(ApiKey is null ? ";" : $";ApiKey={ApiKey}")}";
    }

    /// <summary>
    /// Tries to parse the given string into a <see cref="ModelConnectionString"/>.
    /// </summary>
    /// <param name="connectionString">The connection string to parse.</param>
    /// <param name="result">The parsing result or <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if parsing was successful, <see langword="false"/> otherwise.</returns>
    public static bool TryParse(string connectionString, [NotNullWhen(true)] out ModelConnectionString? result)
    {
        var segments = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var resultingValue = new ModelConnectionString(null!, null!, null!, null);
        var state = ParserState.Empty;

        foreach (var segment in segments)
        {
            if (segment.Split('=', 2) is not [var key, var value])
            {
                result = null;
                return false;
            }

            if (key is "Provider")
            {
                resultingValue = resultingValue with { Provider = value };
                state |= ParserState.ProviderSeen;

                continue;
            }

            if (key is "Endpoint" && Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                resultingValue = resultingValue with { Endpoint = uri };
                state |= ParserState.EndpointSeen;

                continue;
            }

            if (key is "Model")
            {
                resultingValue = resultingValue with { Model = value };
                state |= ParserState.ModelSeen;

                continue;
            }

            if (key is "ApiKey")
            {
                resultingValue = resultingValue with { ApiKey = value };
                state |= ParserState.ApiKeySeen;

                continue;
            }
        }

        if (state.HasFlag(ParserState.ProviderSeen) && state.HasFlag(ParserState.EndpointSeen) && state.HasFlag(ParserState.ModelSeen))
        {
            result = resultingValue;
            return true;
        }

        result = null;
        return false;
    }

    internal sealed class Converter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
        {
            if (value is string connectionString)
            {
                return TryParse(connectionString, out var result) ? result : base.ConvertFrom(context, culture, value);
            }

            return base.ConvertFrom(context, culture, value);

        }
    }

    [Flags]
    private enum ParserState
    {
        Empty = 0,
        ProviderSeen = 1 << 1,
        EndpointSeen = 1 << 2,
        ModelSeen = 1 << 3,
        ApiKeySeen = 1 << 4,
    }
}
