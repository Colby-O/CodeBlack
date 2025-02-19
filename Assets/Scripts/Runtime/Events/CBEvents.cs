using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit { }
}

namespace CodeBlack.Events
{
    public class CBEvents
    {
        public record OpenMenu(bool HidePreviousView = true, bool RememberView = true, System.Type ViewType = default);
        public record CloseMenu();
        public record Start();
        public record Quit();
    }
}
