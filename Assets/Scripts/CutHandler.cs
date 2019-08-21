using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutHandler : MonoBehaviour
{
    [SerializeField] private float _swipeThreshold = 50f;
    
    [SerializeField] private Camera _cutcamera;
    private Vector3 _touchStart;
    private Vector3 _touchUp;    
    
    private Vector3 _drawDouwn;
    private Vector3 _drawUp;

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
        _touchStart = gesture.GetTouchToWorldPoint((Vector3)gesture.position+Vector3.forward);
    }
    
    private void OnTouchUp(Gesture gesture)
    {
        Debug.LogError("cut");

        _touchUp = gesture.GetTouchToWorldPoint((Vector3)gesture.position+Vector3.forward);

        float magnitude = (_touchStart - _touchUp).magnitude;
        if(magnitude <  _swipeThreshold) return;
        
        Plane cutPlane = new Plane(_touchStart,_touchUp,_cutcamera.transform.position);

        if (OnCutPlaneCreate != null)
        {
            OnCutPlaneCreate(cutPlane);
        }
    }
}