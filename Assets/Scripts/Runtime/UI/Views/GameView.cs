using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using TMPro;
using UnityEngine;

namespace CodeBlack.UI
{
    public class GameView : View
    {
        [SerializeField] private TMP_Text _hint;

        public void SetHint(string hint)
        {
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
            _hint.text = string.Empty;
        }

        private void FixedUpdate()
        {
            _hint.text = string.Empty;
        }
    }
}
