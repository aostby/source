using System.Runtime.CompilerServices;
using Kolibri.Kino.Data;

namespace Kolibri.Kino.Tests;

internal static class TestSetup
{
    /// <summary>
    /// Test classes run in parallel and some write through LiteDB directly (like SilverScreen) before any
    /// KinoDatabase exists, so build LiteDB's mappings before the first test (see <see cref="LiteDbMapping"/>).
    /// </summary>
    [ModuleInitializer]
    internal static void WarmUpLiteDb() => LiteDbMapping.WarmUp();
}
