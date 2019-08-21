using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutHandler : MonoBehaviour
{
    [SerializeField] private Camera _cutcamera;
    private Vector3 _touchStart;
    private Vector3 _touchUp;

    public static Action<Plane> OnCutPlaneCreate;
    
    private void OnEnable()
    {
        EasyTouch.On_TouchStart += OnTouchStart;
        EasyTouch.On_TouchUp += OnTouchUp;
    }

    private void OnDisable()
    {
        EasyTouch.On_TouchStart -= OnTouchStart;
        EasyTouch.On_TouchUp -= OnTouchUp;
    }

    private void OnTouchStart(Gesture gesture)
    {
        _touchStart = gesture.GetTouchToWorldPoint((Vector3)gesture.position);
    }
    
    private void OnTouchUp(Gesture gesture)
    {
        _touchUp = gesture.GetTouchToWorldPoint((Vector3)gesture.position);
        Plane cutPlane = new Plane(_touchStart,_touchUp,_cutcamera.transform.position);
        OnCutPlaneCreate?.Invoke(cutPlane);
    }
}