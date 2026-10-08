using Kolibri.Kino.Controllers.Settings;

namespace Kolibri.Kino.Tests;

public sealed class DatabaseChoiceTests : IDisposable
{
    private readonly string _folder = Directory.CreateTempSubdirectory("kino-dbchoice-").FullName;
    private string Current => Path.Combine(_folder, "current", "SilverScreen.db");

    public void Dispose() => Directory.Delete(_folder, recursive: true);

    [Fact]
    public void A_folder_gets_the_current_file_name_and_may_have_no_database_yet()
    {
        var choice = SettingsController.CheckDatabasePath(_folder, Current);

        Assert.Equal(new DatabaseChoice(Path.Combine(_folder, "SilverScreen.db"), FolderExists: true, FileExists: false), choice);
    }

    [Fact]
    public void An_existing_database_file_is_used_as_it_is()
    {
        var file = Path.Combine(_folder, "Other.db");
        File.WriteAllText(file, "");

        Assert.Equal(new DatabaseChoice(file, true, true), SettingsController.CheckDatabasePath($"\"{file}\"", Current));
    }

    [Fact]
    public void A_missing_folder_is_reported()
    {
        var choice = SettingsController.CheckDatabasePath(Path.Combine(_folder, "nope"), Current);

        Assert.Equal(Path.Combine(_folder, "nope", "SilverScreen.db"), choice!.Path);
        Assert.False(choice.FolderExists);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("  ")]
    public void Empty_means_unchanged(string? entry) => Assert.Null(SettingsController.CheckDatabasePath(entry, Current));

    [Fact]
    public void The_current_file_is_unchanged_whatever_its_case() =>
        Assert.Null(SettingsController.CheckDatabasePath(Current.ToUpperInvariant(), Current));
}
