using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Gameplay.GameplayUtilities 
{
    public static class GameplayUtilities 
    {
        public static IEnumerator DoCameraShake(float duration, float XMagnitude, float YMagnitude)
        {
            Transform camera = Camera.main.transform;

            if (camera != null)
            {
                Vector3 originalPosition = camera.localPosition;
                float elapsed = 0;

                while (elapsed < duration)
                {
                    float x = Random.Range(-1f, 1f) * XMagnitude;
                    float y = Random.Range(-1f, 1f) * YMagnitude;

                    camera.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

                    elapsed += Time.unscaledDeltaTime;

                    yield return null;
                }

                camera.localPosition = originalPosition;
            }

            yield return null;
        }

        public static IEnumerator SlowTimeForSecondsRealtime(float timeScale, float seconds) 
        {
            Time.timeScale = timeScale;
            yield return new WaitForSecondsRealtime(seconds);
            Time.timeScale = 1;
            yield return null;
        }
    }
} 