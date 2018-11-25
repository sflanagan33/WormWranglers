using UnityEngine;
using UnityEngine.UI;

using WormWranglers.Util;

namespace WormWranglers.Core
{
    public class RaceOverlay : MonoBehaviour
    {
        [SerializeField] private Color firstPlace;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Image overlay;
        [SerializeField] private Text text;

        private LerpFloat fade = new LerpFloat(0f, 0f, 0.25f, 2, 0f, 1f);
        private JiggleFloat scale = new JiggleFloat(0f, 0f, 0.25f, 0.4f, 0f);

        private bool show; // prevents multiple calls to Show()

        private void Start()
        {
            AnimatedFloatManager.Add(this, fade, false);
            AnimatedFloatManager.Add(this, scale, false);
        }

        private void Update()
        {
            group.alpha = fade;
            text.transform.localScale = scale * Vector3.one;
        }

        public void Show(int place)
        {
            if (!show)
            {
                show = true;
                fade.target = scale.target = 1f;

                if (place == 1)
                    text.text = "1<size=80>st</size>";
                else if (place == 2)
                    text.text = "2<size=80>nd</size>";
                else if (place == 3)
                    text.text = "3<size=80>rd</size>";
                else
                    text.text = "4<size=80>th</size>";

                if (place == 1)
                {
                    overlay.color = firstPlace;
                    scale.target = 1.5f;
                }
            }
        }
    }
}