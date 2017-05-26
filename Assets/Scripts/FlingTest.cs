using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(GraphicRaycaster))]
public class FlingTest : MonoBehaviour {
    Rigidbody2D rb;
    bool swipeFromObject = false;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    void OnEnable () {
        Lean.Touch.LeanTouch.OnFingerDown += OnFingerDown;
        Lean.Touch.LeanTouch.OnFingerSwipe += OnFingerSwipe;
	}
	
	void OnDisable () {
        Lean.Touch.LeanTouch.OnFingerDown -= OnFingerDown;
        Lean.Touch.LeanTouch.OnFingerSwipe -= OnFingerSwipe;
    }

    void OnFingerDown(Lean.Touch.LeanFinger finger)
    {
        if (finger.IsOverGui)
        {
            Debug.Log("Finger " + finger.Index + " something " + finger.ScreenDelta);
        }

        GraphicRaycaster gRaycast = this.gameObject.GetComponent<GraphicRaycaster>();
        PointerEventData ped = new PointerEventData(null);
        ped.position = finger.GetSnapshotScreenPosition(1f);
        List<RaycastResult> results = new List<RaycastResult>();
        gRaycast.Raycast(ped, results);

        if (results != null)
        {
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.name == this.gameObject.name)
                {
                    if (rb.velocity.x < .2f && rb.velocity.y < .2f)
                        swipeFromObject = true;
                }
            }
        }
    }

    void OnFingerSwipe(Lean.Touch.LeanFinger finger)
    {
        if (swipeFromObject == true)
        {
            rb.AddForce(finger.SwipeScaledDelta * 20f);
            swipeFromObject = false;
        }

    }
}
