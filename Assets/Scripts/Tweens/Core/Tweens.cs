using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Tweens 
{
    public interface ITweenEffect 
    {
        public IEnumerator Execute();
    }
    public interface ITweenOwnerListener
    {
        void OnTweenFinish(ITweenEffect tween);
    }

    public class TweenScaler : ITweenEffect
    {
        RectTransform _rectTransform;
        Vector2 _fromScale;
        Vector2 _toScale;
        float _duration;
        float _delay;
        AnimationCurve _curve;
        ITweenOwnerListener _ownerListener;
        Action _callBack;

        public TweenScaler(RectTransform rectTransform, Vector2 fromScale, Vector2 toScale, float duration, AnimationCurve animationCurve, float delay = 0) 
        {
            _rectTransform = rectTransform;
            _fromScale = fromScale;
            _toScale = toScale;
            _duration = duration;
            _delay = delay;
            _curve = animationCurve;
        }

        public TweenScaler(RectTransform rectTransform, Vector2 fromScale, Vector2 toScale, float duration, AnimationCurve animationCurve, ITweenOwnerListener listener, float delay = 0)
        {
            _rectTransform = rectTransform;
            _fromScale = fromScale;
            _toScale = toScale;
            _duration = duration;
            _delay = delay;
            _curve = animationCurve;
            _ownerListener = listener;
        }

        public TweenScaler(RectTransform rectTransform, Vector2 fromScale, Vector2 toScale, float duration, AnimationCurve animationCurve, Action callBack, float delay = 0)
        {
            _rectTransform = rectTransform;
            _fromScale = fromScale;
            _toScale = toScale;
            _duration = duration;
            _delay = delay;
            _curve = animationCurve;
            _callBack = callBack;
        }

        public IEnumerator Execute()
        {
            if (_delay > 0)
            {
                yield return new WaitForSeconds(_delay);
            }

            Vector2 scaleDelta = _fromScale;

            float timeElapse = 0;

            while (timeElapse < _duration) 
            {
                _rectTransform.localScale = Vector3.LerpUnclamped(scaleDelta, _toScale, _curve.Evaluate(timeElapse / _duration));

                timeElapse += Time.deltaTime;
                yield return null;
            }

            _rectTransform.localScale = _toScale;

            if (_ownerListener != null) 
            {
                _ownerListener.OnTweenFinish(this);
            }

            if (_callBack != null) 
            {
                _callBack();
            }

            yield return null;
        }
    }

    public class TweenTranslate : ITweenEffect
    {
        RectTransform _rectTransform;
        Vector2 _fromPos;
        Vector2 _toPos;
        float _duration;
        float _delay;
        AnimationCurve _curve;
        ITweenOwnerListener _ownerListener;
        Action _callBack;

        public TweenTranslate(RectTransform rectTransform, Vector2 fromPos, Vector2 toPos, float duration, AnimationCurve animationCurve, float delay = 0)
        {
            _rectTransform = rectTransform;
            _fromPos = fromPos;
            _toPos = toPos;
            _duration = duration;
            _delay = delay;
            _curve = animationCurve;
        }

        public TweenTranslate(RectTransform rectTransform, Vector2 fromPos, Vector2 toPos, float duration, AnimationCurve animationCurve, ITweenOwnerListener listener, float delay = 0)
        {
            _rectTransform = rectTransform;
            _fromPos = fromPos;
            _toPos = toPos;
            _duration = duration;
            _delay = delay;
            _curve = animationCurve;
            _ownerListener = listener;
        }


        public TweenTranslate(RectTransform rectTransform, Vector2 fromPos, Vector2 toPos, float duration, AnimationCurve animationCurve, Action callBack, float delay = 0)
        {
            _rectTransform = rectTransform;
            _fromPos = fromPos;
            _toPos = toPos;
            _duration = duration;
            _delay = delay;
            _curve = animationCurve;
            _callBack = callBack;
        }


        public IEnumerator Execute()
        {
            if (_delay > 0)
            {
                yield return new WaitForSeconds(_delay);
            }

            Vector2 posDelta = _fromPos;

            float timeElapse = 0;

            while (timeElapse < _duration)
            {
                _rectTransform.anchoredPosition = Vector3.LerpUnclamped(posDelta, _toPos, _curve.Evaluate(timeElapse / _duration));

                timeElapse += Time.deltaTime;
                yield return null;
            }

            _rectTransform.anchoredPosition = _toPos;

            if (_ownerListener != null)
            {
                _ownerListener.OnTweenFinish(this);
            }

            if (_callBack != null) 
            {
                _callBack();
            }

            yield return null;
        }
    }

    public class TweenImageAlpha : ITweenEffect
    {
        Image _image;
        float _fromAlpha;
        float _toAlpha;
        float _duration;
        float _delay;
        AnimationCurve _curve;
        ITweenOwnerListener _ownerListener;
        Action _callBack;

        public TweenImageAlpha(Image imageReference, float fromAlpha, float toAlpha, float duration, AnimationCurve animationCurve, float delay = 0)
        {
            _image = imageReference;
            _fromAlpha = fromAlpha;
            _toAlpha = toAlpha;
            _duration = duration;
            _delay = delay;
            _curve = animationCurve;
        }

        public TweenImageAlpha(Image imageReference, float fromAlpha, float toAlpha, float duration, AnimationCurve animationCurve, ITweenOwnerListener listener, float delay = 0)
        {
            _image = imageReference;
            _fromAlpha = fromAlpha;
            _toAlpha = toAlpha;
            _duration = duration;
            _delay = delay;
            _curve = animationCurve;
            _ownerListener = listener;
        }


        public TweenImageAlpha(Image imageReference, float fromAlpha, float toAlpha, float duration, AnimationCurve animationCurve, Action callBack, float delay = 0)
        {
            _image = imageReference;
            _fromAlpha = fromAlpha;
            _toAlpha = toAlpha;
            _duration = duration;
            _delay = delay;
            _curve = animationCurve;
            _callBack = callBack;
        }

        public IEnumerator Execute()
        {
            if (_delay > 0)
            {
                yield return new WaitForSeconds(_delay);
            }

            float alphaDelta = _fromAlpha;
            float timeElapse = 0;
            Color currentColor = _image.color;

            while (timeElapse < _duration)
            {
                currentColor.a = Mathf.LerpUnclamped(alphaDelta, _toAlpha, timeElapse / _duration);
                _image.color = currentColor;

               timeElapse += Time.deltaTime;
                yield return null;
            }

            currentColor = _image.color;
            currentColor.a = _toAlpha;

            _image.color = currentColor;

            if (_ownerListener != null)
            {
                _ownerListener.OnTweenFinish(this);
            }

            if (_callBack != null) 
            {
                _callBack();
            }

            yield return null;
        }
    }

    public class TweenPopUp : ITweenEffect
    {
        RectTransform _rectTransform;
        float _duration;
        float _popUpScale = 1;
        float _delay;
        AnimationCurve _curve;
        ITweenOwnerListener _ownerListener;
        Action _callBack;

        public TweenPopUp(RectTransform rectTransform, float popUpScale, float duration, AnimationCurve animationCurve, float delay = 0)
        {
            if (popUpScale < 0) 
            {
                popUpScale = 1;
            }

            _rectTransform = rectTransform;
            _duration = duration;
            _popUpScale = popUpScale;
            _delay = delay;
            _curve = animationCurve;
        }

        public TweenPopUp(RectTransform rectTransform, float popUpScale,float duration, AnimationCurve animationCurve, ITweenOwnerListener listener, float delay = 0)
        {
            if (popUpScale < 0)
            {
                popUpScale = 1;
            }

            _rectTransform = rectTransform;
            _duration = duration;
            _popUpScale = popUpScale;
            _delay = delay;
            _curve = animationCurve;
            _ownerListener = listener;
        }

        public TweenPopUp(RectTransform rectTransform, float popUpScale, float duration, AnimationCurve animationCurve, Action callBack, float delay = 0)
        {
            if (popUpScale < 0)
            {
                popUpScale = 1;
            }

            _rectTransform = rectTransform;
            _duration = duration;
            _popUpScale = popUpScale;
            _delay = delay;
            _curve = animationCurve;
            _callBack = callBack;
        }

        public IEnumerator Execute()
        {
            if (_delay > 0)
            {
                yield return new WaitForSeconds(_delay);
            }

            Vector2 startSize = new Vector2(1,1);
            float timeElapse = 0;

            while (timeElapse < _duration)
            {
                float curveValue = _curve.Evaluate(timeElapse);
                Vector2 toScale = startSize + (new Vector2(curveValue, curveValue) * _popUpScale);
                _rectTransform.localScale = Vector3.LerpUnclamped(startSize, toScale, _curve.Evaluate(timeElapse / _duration));
                timeElapse += Time.deltaTime;
                yield return null;
            }

            _rectTransform.localScale = startSize;

            if (_ownerListener != null)
            {
                _ownerListener.OnTweenFinish(this);
            }

            if (_callBack != null)
            {
                _callBack();
            }

            yield return null;
        }
    }

}