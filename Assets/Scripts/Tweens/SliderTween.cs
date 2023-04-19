using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Tweens;

public class SliderTween : MonoBehaviour, ITweenOwnerListener
{
    ITweenEffect _translateIn;
    ITweenEffect _translateOut;
    public Vector2 _aPosition;
    public Vector2 _bPosition;
    public AnimationCurve curve;
    public float duration = .5f;

    private void Awake()
    {
        _translateIn = new TweenTranslate(GetComponent<RectTransform>(), _aPosition, _bPosition, duration, curve, this);
        _translateOut = new TweenTranslate(GetComponent<RectTransform>(), _bPosition, _aPosition, duration, curve, this);
    }

    public void OnTweenFinish(ITweenEffect tween)
    {
        if (_translateIn == tween)
        {
            OnTranslateInFinish();
        }

        if (_translateOut == tween)
        {
            OnTranslateOutFinish();
        }

    }

    public void TranslateIn()
    {
        if (_translateIn != null)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            StopAllCoroutines();
            StartCoroutine(_translateIn.Execute());
        }
    }

    public void OnTranslateInFinish()
    {
        //Debug.Log("TransitionInFinished");
    }

    public void TranslateOut()
    {
        if (_translateOut != null)
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            StopAllCoroutines();
            StartCoroutine(_translateOut.Execute());
        }
    }

    public void OnTranslateOutFinish()
    {
        //Debug.Log("TransitionInFinished");
        gameObject.SetActive(false);
    }
}
