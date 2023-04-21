using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    Camera cameraComponent;

    private void Awake()
    {
        cameraComponent = GetComponent<Camera>();
    }


    public void Zoom(float newCameraSize, float duration) 
    {
        StartCoroutine(CZoom(newCameraSize, duration));
    }

    IEnumerator CZoom(float newCameraSize, float duration) 
    {
        float currentSize = cameraComponent.orthographicSize;

        float timeElapsed = 0;

        while (!Mathf.Approximately(currentSize, newCameraSize))
        {
            currentSize = Mathf.Lerp(currentSize, newCameraSize, timeElapsed / duration);
            timeElapsed += Time.deltaTime;

            cameraComponent.orthographicSize = currentSize;
            yield return new WaitForEndOfFrame();
        }

        //After we check if they are aprox, we just clamp into new size
        cameraComponent.orthographicSize = newCameraSize;
        Debug.Log("Zooming Finished");

        yield return null;
    }
}
