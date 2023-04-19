using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI.Tweens;

public class LinealPopUp : MonoBehaviour
{
    ITweenEffect _tweenEffect;
    public Vector2 _fromPosition;
    public Vector2 _toPosition;
    public AnimationCurve curve;
    public float duration = .5f;

    private void Awake()
    {
        _tweenEffect = new TweenScaler(GetComponent<RectTransform>(), _fromPosition, _toPosition, duration, curve);
    }

    public void OnEnable()
    {
        if (_tweenEffect != null) 
        {
            StartCoroutine(_tweenEffect.Execute());
        }
    }

}
