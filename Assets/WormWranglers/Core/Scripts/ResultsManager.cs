using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WormWranglers.Core
{
    public class ResultsManager : MonoBehaviour
    {
        /*public Text labels;
        public Text results;

        private void Awake()
        {
            float[] times = Settings.loseTimes;

            string l = "";
            string r = "";
            for (int i = 0; i < times.Length; i++)
            {
                l += string.Format("Beetle {0}:\n", i + 1);

                if (times[i] == -1)
                    r += "Winner!\n";
                else
                    r += string.Format("{0:.00} s\n", times[i]);
            }

            Labels.text = l;
            Results.text = r;
        }*/

        public void ToScene(int index)
        {
			MusicManager.Play(MusicTrack.None);
            TransitionManager.ToScene(index);
        }
    }
}