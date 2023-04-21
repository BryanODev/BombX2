using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.Tweens;

public class ScreenTransition : MonoBehaviour, ITweenOwnerListener
{
    ITweenEffect _transitionIn;
    ITweenEffect _transitionOut;
    public Vector2 _aPosition;
    public Vector2 _bPosition;
    public AnimationCurve curve;
    public float duration = .5f;

    private void Awake()
    {
        _transitionIn = new TweenScaler(GetComponent<RectTransform>(), _aPosition, _bPosition, duration, curve, this);
        _transitionOut = new TweenScaler(GetComponent<RectTransform>(), _bPosition, _aPosition, duration, curve, this);
    }

    public void OnTweenFinish(ITweenEffect tween)
    {
        if (_transitionIn == tween) 
        {
            OnTransitionInFinish();
        }

        if (_transitionOut == tween) 
        {
            OnTransitionOutFinish();
        }

    }

    public void TransitionIn()
    {
        if (_transitionIn != null) 
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            StartCoroutine(_transitionIn.Execute());
        }
    }

    public void OnTransitionInFinish() 
    {
        //Debug.Log("TransitionInFinished");
        gameObject.SetActive(false);
    }

    public void TransitionOut()
    {
        if (_transitionOut != null)
        {
            if (!gameObject.activeSelf) 
            {
                gameObject.SetActive(true);
            }

            StartCoroutine(_transitionOut.Execute());
        }
    }

    public void OnTransitionOutFinish()
    {
        //Debug.Log("TransitionInFinished");
    }
}
