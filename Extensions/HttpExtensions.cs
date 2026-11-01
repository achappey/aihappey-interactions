using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace AIHappey.Interactions.Extensions;

public static class HttpExtensions
{
    private static readonly MediaTypeWithQualityHeaderValue AcceptJson = new("application/json");
    private static readonly MediaTypeWithQualityHeaderValue AcceptSse = new("text/event-stream");

    private static readonly JsonSerializerOptions jsonOpts = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// POST JSON and deserialize a steps-based interaction response.
    /// </summary>
    public static Task<Interaction> GetInteraction(
        this HttpClient client,
        InteractionRequest options,
        string providerId,
        string relativeUrl = "v1beta/interactions",
        CancellationToken ct = default)
        => client.GetInteraction(options, providerId, observer: null, relativeUrl: relativeUrl, ct: ct);

    public static async Task<Interaction> GetInteraction(
        this HttpClient client,
        InteractionRequest options,
        string providerId,
        IInteractionTransportObserver? observer,
        string relativeUrl = "v1beta/interactions",
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (string.IsNullOrWhiteSpace(relativeUrl)) throw new ArgumentNullException(nameof(relativeUrl));

        using var req = new HttpRequestMessage(HttpMethod.Post, relativeUrl);
        req.Headers.Accept.Clear();
        req.Headers.Accept.Add(AcceptJson);
        var payload = JsonSerializer.SerializeToElement(options, jsonOpts);
        req.Content = new StringContent(payload.GetRawText(), Encoding.UTF8, "application/json");

        if (observer is not null)
            await observer.OnBodyAsync("request-body", payload.GetRawText(), "application/json", ct);
        using var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
        var body = await resp.Content.ReadAsStringAsync(ct);
        if (observer is not null)
            await observer.OnBodyAsync("response-body", body, resp.Content.Headers.ContentType?.MediaType ?? "application/json", ct);
        await ThrowIfNotSuccess(resp, ct);

        var result = JsonSerializer.Deserialize<Interaction>(body)
            ?? throw new InvalidOperationException($"Empty JSON response for {relativeUrl}.");

        NormalizeInteractionModel(result, providerId);
        return result;
    }

    /// <summary>
    /// POST JSON with stream=true and parse steps-based SSE events.
    /// </summary>
    public static IAsyncEnumerable<InteractionStreamEventPart> GetInteractions(
        this HttpClient client,
        InteractionRequest options,
        string providerId,
        string relativeUrl = "v1beta/interactions",
        CancellationToken ct = default)
        => client.GetInteractions(options, providerId, observer: null, relativeUrl: relativeUrl, ct: ct);

    public static async IAsyncEnumerable<InteractionStreamEventPart> GetInteractions(
        this HttpClient client,
        InteractionRequest options,
        string providerId,
        IInteractionTransportObserver? observer,
        string relativeUrl = "v1beta/interactions",
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(client);
        if (string.IsNullOrWhiteSpace(relativeUrl)) throw new ArgumentNullException(nameof(relativeUrl));

        using var req = new HttpRequestMessage(HttpMethod.Post, relativeUrl);

        req.Headers.Accept.Clear();
        req.Headers.Accept.Add(AcceptSse);
        req.Headers.CacheControl = new CacheControlHeaderValue { NoCache = true };
        var payload = JsonSerializer.SerializeToElement(options, jsonOpts);

        req.Content = new StringContent(payload.GetRawText(), Encoding.UTF8, "application/json");

        if (observer is not null)
            await observer.OnBodyAsync("request-body", payload.GetRawText(), "application/json", ct);
        using var resp = await client.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
        if (!resp.IsSuccessStatusCode && observer is not null)
            await observer.OnBodyAsync("response-body", await resp.Content.ReadAsStringAsync(ct),
                resp.Content.Headers.ContentType?.MediaType ?? "application/json", ct);
        await ThrowIfNotSuccess(resp, ct);

        await using var stream = await resp.Content.ReadAsStreamAsync(ct);
        using var observed = observer?.ObserveStream(stream, resp.Content.Headers.ContentType?.MediaType ?? "text/event-stream");
        using var reader = new StreamReader(observed ?? stream);

        var dataLines = new List<string>();
        string? line;
        while ((line = await reader.ReadLineAsync(ct)) != null)
        {
            if (line.Length == 0)
            {
                if (TryParseSseData(dataLines, out var data))
                    yield return DeserializeStreamEvent(data, providerId);
                dataLines.Clear();
                continue;
            }

            if (line[0] == ':')
                continue;

            if (line.StartsWith("data:", StringComparison.Ordinal))
                dataLines.Add(line.Length > 5 && line[5] == ' ' ? line[6..] : line[5..]);
        }

        if (TryParseSseData(dataLines, out var finalData))
            yield return DeserializeStreamEvent(finalData, providerId);
    }

    private static bool TryParseSseData(List<string> dataLines, out string data)
    {
        data = string.Join('\n', dataLines);
        return dataLines.Count > 0 && data.Length > 0;
    }

    private static InteractionStreamEventPart DeserializeStreamEvent(string data, string providerId)
    {
        InteractionStreamEventPart evt;
        try
        {
            evt = JsonSerializer.Deserialize<InteractionStreamEventPart>(data)
                ?? throw new JsonException("Interaction stream event must be a non-null object.");
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"Failed to parse SSE json event: {data}", ex);
        }

        switch (evt)
        {
            case InteractionCreatedEvent created:
                NormalizeInteractionModel(created.Interaction, providerId);
                break;
            case InteractionCompletedEvent completed:
                NormalizeInteractionModel(completed.Interaction, providerId);
                break;
        }

        return evt;
    }

    private static async Task ThrowIfNotSuccess(HttpResponseMessage resp, CancellationToken ct)
    {
        if (resp.IsSuccessStatusCode) return;

        var body = resp.Content is null ? "" : await resp.Content.ReadAsStringAsync(ct);
        throw new HttpRequestException($"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}: {body}");
    }

    private static void NormalizeInteractionModel(Interaction? interaction, string providerId)
    {
        if (interaction is null || string.IsNullOrWhiteSpace(interaction.Model))
            return;

        interaction.Model = EnsureProviderScopedModel(interaction.Model, providerId);
    }

    private static string EnsureProviderScopedModel(string model, string providerId)
    {
        if (string.IsNullOrWhiteSpace(model) || string.IsNullOrWhiteSpace(providerId))
            return model;

        var normalizedProviderPrefix = $"{providerId}/";
        return model.StartsWith(normalizedProviderPrefix, StringComparison.OrdinalIgnoreCase)
            ? model
            : $"{providerId}/{model}";
    }

}
