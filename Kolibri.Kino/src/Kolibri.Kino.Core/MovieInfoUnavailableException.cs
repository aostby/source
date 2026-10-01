namespace Kolibri.Kino.Core;

/// <summary>
/// The movie information service can't answer at all right now (bad API key, daily limit reached),
/// as opposed to "this title was not found". Long-running work should stop instead of failing every item.
/// </summary>
public sealed class MovieInfoUnavailableException(string message, Exception? inner = null) : Exception(message, inner);
