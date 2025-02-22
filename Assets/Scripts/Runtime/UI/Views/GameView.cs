using CodeBlack.Events;
using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace CodeBlack.UI
{
    public class GameView : View
    {
        [SerializeField] private TMP_Text _hint;

        float currentPiroty = 0;

        public void SetHint(string hint, int piroty = 0)
        {
            if (hint == string.Empty) currentPiroty = 0;
            if (piroty < currentPiroty) return;
            currentPiroty = piroty;
            if (!GameManager.GetMonoSystem<IUIMonoSystem>().GetView<SettingsView>().EnabledHints) return;
            _hint.text = hint;
        }

        public override void Show()
        {
            base.Show();
            GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(1, PlazmaGames.Audio.AudioType.Music, true, false);
        }

        public override void Init()
        {
            SetHint(string.Empty);
        }

        private void FixedUpdate()
        {
            SetHint(string.Empty);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape) && !CodeBlackGameManager.ending)
            {
                if (GameManager.HasMonoSystem<IAudioMonoSystem>()) GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio(2, PlazmaGames.Audio.AudioType.Music, true, false);
                GameManager.EmitEvent(new CBEvents.OpenMenu(true, true, typeof(PauseMenuView)));
            }
        }
    }
}
