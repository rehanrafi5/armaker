using UnityEngine;
using UnityEngine.Playables;
using System.Collections;

namespace ARMarker
{

    public class TimelinePlayer : MonoBehaviour
    {

        [SerializeField]
        private PlayableDirector director;

        private Coroutine reverseCoroutine;
        private bool lastPlayedForward = false;

        public void PlayAuto()
        {
            if (lastPlayedForward)
            {
                PlayReverse();
            }
            else
            {
                Play();
            }
        }

        public void Play()
        {
            if(lastPlayedForward)
            {
                return;
            }

            if (reverseCoroutine != null)
            {
                StopCoroutine(reverseCoroutine);
            }

            director.time = 0;
            director.Play();
            lastPlayedForward = true;
        }

        public void PlayReverse()
        {
            if (!lastPlayedForward)
            {
                return;
            }

            if (reverseCoroutine != null)
            {
                StopCoroutine(reverseCoroutine);
            }

            reverseCoroutine = StartCoroutine(C_PlayReverse());
            lastPlayedForward = false;
        }

        private IEnumerator C_PlayReverse()
        {
            director.time = director.duration;
            while (director.time > 0)
            {
                director.time -= Time.deltaTime;
                director.Evaluate();
                yield return null;
            }

            director.time = 0;
            director.Evaluate();
            director.Pause();
            reverseCoroutine = null;
        }

    }

}