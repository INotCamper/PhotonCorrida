using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MyComapany.MyGame
{
    public class ThreeDots : MonoBehaviour
    {
        #region Private Fields

        [Tooltip("The text that will be added three dots and then remove the dots")]
        [SerializeField]
        public Text textToAddDots;
        [Tooltip("The time between adicion of dots")]
        [Min(0)]
        [SerializeField]
        public float time;
        [Tooltip("How many dots wil be added to the text")]
        [Min(0)]
        [SerializeField]
        public int dotsQuantity;
        [Tooltip("How many times the dots will repeat, if 'infinite' set to 999+(depends on the time setted)")]
        [Min(0)]
        [SerializeField]
        public int repeats;

        #endregion



        #region MonoBehaviour Callbacks

        void Start()
        {
            if (textToAddDots == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> text Reference.", this);
            }
            else if (time <=0)
            {
                Debug.LogWarning("<Color=Red><a>Little</a></Color> time to animate.", this);
            }
            else if (dotsQuantity <=0)
            {
                Debug.LogWarning("<Color=Red><a>Not</a></Color> needed if the dots quantity is 0.", this);
            }
            else
            {
                StartCoroutine(ThreeDotsAnimation(time, dotsQuantity, repeats));
            }
        }

        #endregion



        #region IEnumerator Callbacks

        IEnumerator ThreeDotsAnimation(float t, int quant, int rep)
        {
            string backupText = textToAddDots.text;
            for (int j = 0; j < rep; j++)
            {
                for (int i = 0; i < quant; i++)
                {
                    textToAddDots.text += ".";
                    yield return new WaitForSeconds(t);
                }
                textToAddDots.text = backupText;
            }
        }

        #endregion
    }
}
