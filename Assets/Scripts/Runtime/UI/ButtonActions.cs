using PlazmaGames.Audio;
using PlazmaGames.Core;
using PlazmaGames.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace CodeBlack.UI
{
    public enum ButtonActions
    {
        OnHover,
        OnClick
    }

    public static class ButtonActionHandler
    {
        private static Dictionary<ButtonActions, UnityAction> _actions = new Dictionary<ButtonActions, UnityAction>()
        {
            {
                ButtonActions.OnHover, () =>
                {
                    GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("ButtonHover", PlazmaGames.Audio.AudioType.Sfx, false, true);
                }
            },
            {
                ButtonActions.OnClick, () =>
                {
                    GameManager.GetMonoSystem<IAudioMonoSystem>().PlayAudio("ButtonClick", PlazmaGames.Audio.AudioType.Sfx, false, true);
                }
            },
        };

        public static void CallEvent(ButtonActions action)
        {
            if (_actions.ContainsKey(action))
            {
                _actions[action]?.Invoke();
            }
            else
            {
                Debug.LogWarning($"Invaild Button Action Call Of Type {action}.");
            }
        }
    }
}
