using Kolibri.Kino.Core.Models;

namespace Kolibri.Kino.Core.Abstractions;

public interface IMovieFileNameParser
{
    ParsedMovieFile Parse(string filePath);
}
