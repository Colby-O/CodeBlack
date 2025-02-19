using PlazmaGames.Core;
using PlazmaGames.Core.Events;
using PlazmaGames.Core.Debugging;
using PlazmaGames.UI;
using PlazmaGames.UI.Views;
using UnityEngine;
using CodeBlack.UI;

namespace CodeBlack.Events
{
    public static class UIGameEvents
    {
        private static EventResponse _openMenuResponse;
        private static EventResponse _closeMenuResponse;
        private static EventResponse _pauseResponse;

        public static EventResponse OpenMenuResponse
        {
            get
            {
                _openMenuResponse ??= new EventResponse(OpenMenuEvent);
                return _openMenuResponse;
            }
        }

        public static EventResponse CloseMenuResponse
        {
            get
            {
                _closeMenuResponse ??= new EventResponse(CloseMenuEvent);
                return _closeMenuResponse;
            }
        }

        private static void OpenMenuEvent(Component _, object raw)
        {
            if (raw == null || raw is not CBEvents.OpenMenu)
            {
                PlazmaDebug.LogError("Failed to opne view, data passed is null or wrong type.", "Event Error");
                return;
            }

            CBEvents.OpenMenu data = raw as CBEvents.OpenMenu;

            bool hidePreviousView = data.HidePreviousView;
            bool rememberView = data.RememberView;
            System.Type viewType = data.ViewType;

            if (viewType == null)
            {
                PlazmaDebug.LogError("Trying to open an invaild View Type.", "Event Error");
                return;
            }

           GameManager.GetMonoSystem<IUIMonoSystem>().Show(viewType, rememberView, hidePreviousView);
        }

        private static void CloseMenuEvent(Component _, object raw)
        {
            GameManager.GetMonoSystem<IUIMonoSystem>().ShowLast();
        }
    }
}
