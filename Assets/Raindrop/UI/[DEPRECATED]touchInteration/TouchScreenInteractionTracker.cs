﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

/// <summary>
/// A instance of a user interacting with the screen.
/// Responsible for tracking fingers and counting them over their lifecycle.
/// Responsible for providing finger zoom delta and finger move delta.
/// </summary>
public class TouchScreenInteractionTracker
{

    // interaction state.
    internal enum InteractionType
    {
        none, //no touch/ more than 2 fingers
        zoom, //pinching
        pan   //single finger.
    }
    private InteractionType previousType = InteractionType.none;
    private bool interactionTypeHasChanged = false; // if pan->zoom / none->zoom, etc.

    // internal memory of touches.
    private List<Vector2> initialTouchPositions = new List<Vector2>(); //each finger's first touch position.
    private List<Vector2> currentTouchPositions = new List<Vector2>();
    //private Collider touchFocus; //what we think that the user is touching

    public Vector2 oneFingerMoveDelta { get; private set; }
    public float twoFingerPinchDelta { get; private set; }
    public Vector2 twoFingerMoveDelta { get; private set; }

    // The camera that we are interacting through
    public Camera camera; 

    // The object of our touch interaction instance
    public GameObject pointingContext;
    private LayerMask layerMask;

    /// <summary>
    /// update the internal status.
    /// </summary>
    public void Update()
    {
        //1. get interaction type from finger count 
        InteractionType presentType = fingerCountToInteractionType(Touch.activeFingers.Count);
        //2. check if the interaction has changed
        interactionTypeHasChanged = isStateChanged(previousType, presentType);
        //3a. if changed, 
        if (interactionTypeHasChanged)
        {
            ResetTouchTracking();
            //getProbableTouchFocus();
            return;
        }
        else
        {
            UpdateTouchPositions();
            UpdateTouchDeltas(presentType);
        }
    }

    internal void finaliseInteraction()
    {
        throw new NotImplementedException();
    }

    private void UpdateTouchDeltas(InteractionType presentType)
    {
        if (presentType == InteractionType.zoom)
        {
            twoFingerPinchDelta = Vector2.Distance(initialTouchPositions[0], initialTouchPositions[1]);
            Vector2 ave_initial = (initialTouchPositions[0] + initialTouchPositions[1]) / 2;
            Vector2 ave_current = (currentTouchPositions[0] + currentTouchPositions[1]) / 2;

            Vector2 direction = initialTouchPositions[0] - currentTouchPositions[0];
            twoFingerMoveDelta = direction;
        }
        if (presentType == InteractionType.pan)
        {
            Vector2 direction = initialTouchPositions[0] - currentTouchPositions[0];
            oneFingerMoveDelta = direction;
        }
    }

    internal InteractionType GetCurrentInteractionState()
    {
        return previousType;
    }

    public bool IsInteractionChanged()
    {
        return interactionTypeHasChanged;
    }

    //private void getProbableTouchFocus()
    //{
    //    if (Touch.activeFingers.Count == 0)
    //    {
    //        touchFocus = null;
    //    } else
    //    {
    //        Vector2 pos = Touch.activeFingers[0].screenPosition;
    //        touchFocus =  pos;

    //        worldPoint_startPos = cam.ScreenToWorldPoint(pos);
    //    }
    //}

    private void UpdateTouchPositions()
    {
        currentTouchPositions.Clear();
        for (int i = 0; i < Touch.activeFingers.Count; i++)
        {
            currentTouchPositions.Add(Touch.activeFingers[i].screenPosition);

        }
    }

    // reset the initial touch point.
    private void ResetTouchTracking()
    {
        initialTouchPositions.Clear();
        for (int i = 0; i < Touch.activeFingers.Count; i++)
        {
            initialTouchPositions.Add(Touch.activeFingers[i].screenPosition);
        }
        if (initialTouchPositions.Count > 0)
        {
            pointingContext = getPointingContext(this.camera, initialTouchPositions[0], layerMask);
        } else

        {
            pointingContext = null;
        }


    }

    /// <summary>
    /// Return the gameobejct that the finger is touching.
    /// </summary>
    /// <returns></returns>
    private GameObject getPointingContext(Camera cam , Vector2 touchPos, LayerMask layerMask)
    {
        RaycastHit hit;
        Vector3 worldPoint = camera.ScreenToWorldPoint(touchPos);
        Ray ray = camera.ScreenPointToRay(touchPos);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);

        RaycastHit rayHit;
        GameObject res = null;
        if (Physics.Raycast(ray, out rayHit, 500, layerMask))
        {
            res = rayHit.transform.gameObject;
        }
        return res;
    }

    /// <summary>
    /// convert numberOfFingers into InteractionState
    /// </summary>
    /// <param name="fingerCount"></param>
    /// <returns></returns>
    private InteractionType fingerCountToInteractionType(int fingerCount)
    {
        switch (fingerCount)
        {
            case 0:
                return InteractionType.none;
            case 1:
                return InteractionType.pan;
            case 2:
                return InteractionType.zoom;
            default:
                return InteractionType.none;
        }

    }

    /// <summary>
    /// get the relative zoom change from present zoom (0).
    /// </summary>
    /// <returns></returns>
    public float getZoomDelta()
    {
        return twoFingerPinchDelta;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool isZoom()
    {
        return previousType == InteractionType.zoom;
    }

    /// <summary>
    /// get the relative pan change from present pan (0,0).
    /// </summary>
    /// <returns></returns>
    public Vector2 getPanDelta()
    {
        //var direction = touchInitialPosition - worldPoint_now;
        return oneFingerMoveDelta;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool isPan()
    {
        return previousType == InteractionType.pan;
    }


    /// <summary>
    /// Check if internal 'interaction' state is changed.
    /// </summary>
    /// <param name="newState"></param>
    private bool isStateChanged(InteractionType oldState, InteractionType newState)
    {
        if (oldState == newState)
        {
            return false;
        }
        return true;
    }
}

