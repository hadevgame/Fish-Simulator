using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UCExtension.Animation
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GUIAnim : MonoBehaviour
    {
        [SerializeField] bool animatingInteractable = true;

        [SerializeField] bool skipOpenAnim;

        [SerializeField] bool skipCloseAnim;

        List<BaseGraphicAnim> anims;

        public CanvasGroup group { get; private set; }

        private void Awake()
        {
            anims = new List<BaseGraphicAnim>();
            var temps = GetComponentsInChildren<BaseGraphicAnim>();
            foreach (var item in temps)
            {
                if (item.ControlByGUIAnim)
                {
                    anims.Add(item);
                }
            }
            BaseGraphicAnim comp = GetComponent<BaseGraphicAnim>();
            if (comp && comp.ControlByGUIAnim)
            {
                anims.Add(comp);
            }
            group = GetComponent<CanvasGroup>();
        }

        public void PlayAnim(GraphicState state, UnityAction finish = null)
        {
            if (!NeedPlayAnim(state))
            {
                foreach (var item in anims)
                {
                    item.SetState(state);
                }
                finish?.Invoke();
            }
            else
            {
                int waitTime = anims.Count;
                group.interactable = animatingInteractable;
                foreach (var item in anims)
                {
                    item.KillAnimation();
                    item.PlayAnimation(state, () =>
                    {
                        waitTime--;
                        if (waitTime == 0)
                        {
                            group.interactable = true;
                            finish?.Invoke();
                        }
                    });
                }
            }
        }

        bool NeedPlayAnim(GraphicState state)
        {
            if (state == GraphicState.Hidden && skipCloseAnim) return false;
            if (state == GraphicState.Visible && skipOpenAnim) return false;
            if (anims.Count == 0) return false;
            return true;
        }

#if UNITY_EDITOR
        [Button("Fix Grey Buttons")]
        public void WhiteAllButtons()
        {
            var buttons = transform.GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                var newColors = new ColorBlock();
                newColors.normalColor = Color.white;
                newColors.highlightedColor = Color.white;
                newColors.pressedColor = Color.white;
                newColors.selectedColor = Color.white;
                newColors.disabledColor = new Color(200f / 256, 200f / 256f, 200f / 256, 128f / 255);
                newColors.colorMultiplier = 1;
                button.colors = newColors;
                button.SetDirtyAndSave();
            }
        }

#endif
    }
}