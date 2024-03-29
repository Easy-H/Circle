﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class UiAnimation {

    public UiAnimationType _eventType;

    public Image _target = null;

    public float _valueOfEvent = 0f;
    public float _timeOfEvent = 0f;

    public AudioSource _sound;

}

public enum UiAnimationType {
    Rest,
    FadeIn,
    FillImage,
    Close
}

[System.Serializable]
public class UiAnimationGenerator {

    public string _actionName;

    [SerializeField] UiAnimation[] _animations = null;

    int _actnum = 0;

    public void Action() {
        if (_actnum >= _animations.Length) {
            _actnum = 0;
            return;

        }

        UiAnimation ua = _animations[_actnum];

        _actnum++;

        if (ua._sound)
            ua._sound.Play();
        
        if (ua._eventType == UiAnimationType.Rest) {
            UiManager.Instance.StartCoroutine(Justrest(ua._timeOfEvent));
        }
        else {
            ua._target.gameObject.SetActive(true);

            switch (ua._eventType) {
                case UiAnimationType.Close:
                    ua._target.gameObject.SetActive(false);
                    Action();
                    break;
                case UiAnimationType.FadeIn:
                    UiManager.Instance.StartCoroutine(Fade(ua._target, ua._valueOfEvent, ua._timeOfEvent));
                    break;
                case UiAnimationType.FillImage:
                    UiManager.Instance.StartCoroutine(FillImage(ua._target, ua._valueOfEvent, ua._timeOfEvent));
                    break;
                default:
                    break;
                    
            }
        }

    }

    IEnumerator FillImage(Image panel, float goalFill,  float time) {
        float startTime = Time.realtimeSinceStartup;
        float useTime = Time.realtimeSinceStartup;
        float endTime = useTime + time;
        float firstFill = panel.fillAmount;
        
        while (useTime < endTime) {

            useTime = Time.realtimeSinceStartup;
            panel.fillAmount = Mathf.Lerp(firstFill, goalFill, (useTime - startTime) / time);

            yield return new WaitForEndOfFrame();
        }
        panel.fillAmount = goalFill;

        Action();

    }

    IEnumerator Justrest(float time) {

        float useTime = Time.realtimeSinceStartup;
        float endTime = useTime + time;

        while (useTime < endTime) {
            useTime = Time.realtimeSinceStartup;
            yield return new WaitForEndOfFrame();

        }

        Action();

    }

    IEnumerator Fade(Image target, float goalAlpha,  float time) {

        float startTime = Time.realtimeSinceStartup;
        float useTime = Time.realtimeSinceStartup;
        float endTime = useTime + time;

        float firstAlpha = target.color.a;

        target.color = new Color(target.color.r, target.color.r, target.color.r, firstAlpha);
        
        while (useTime < endTime) {

            useTime = Time.realtimeSinceStartup;

            float alpha = Mathf.Lerp(firstAlpha, goalAlpha, (useTime - startTime) / time);

            yield return new WaitForEndOfFrame();

            target.color = new Color(target.color.r, target.color.r, target.color.r, alpha);

        }

        target.color = new Color(target.color.r, target.color.r, target.color.r, goalAlpha);
        
        Action();

    }
}