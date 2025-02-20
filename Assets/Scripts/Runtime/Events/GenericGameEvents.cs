using CodeBlack.UI;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.Core.Events;
using PlazmaGames.UI;
using UnityEngine;

namespace CodeBlack.Events
{
    public static class GenericGameEvents
    {
        private static EventResponse _quitResponse;
        private static EventResponse _startResponse;

        public static EventResponse QuitResponse { 
            get { 
                _quitResponse ??= new EventResponse(QuitEvent);
                return _quitResponse;
            }
        }

        public static EventResponse StartResponse
        {
            get
            {
                _startResponse ??= new EventResponse(StartEvent);
                return _startResponse;
            }
        }

        private static void QuitEvent(Component _, object __)
        {
            Application.Quit();
        }

        private static void StartEvent(Component _, object __)
        {
            CodeBlackGameManager.hasStarted = true;
            GameManager.GetMonoSystem<IUIMonoSystem>().Show<GameView>();
        }
    }
}
