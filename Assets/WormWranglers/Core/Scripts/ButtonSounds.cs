using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace WormWranglers.Core
{
	public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
	{
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
			MusicManager.Play(Sound.Tick);
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
			MusicManager.Play(Sound.Select);
        }
    }
}