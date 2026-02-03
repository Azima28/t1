using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeController : MonoBehaviour
{
    [SerializeField] private float minSwipeDistance = 50f;
    [SerializeField] private float maxTime = 1f;
    [SerializeField] private LevelPagingController pagingController;

    private Vector2 startPoint;
    private Vector2 endPoint;
    private float startTime;
    private float endTime;

    private void Update()
    {
        // Touch Input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startPoint = touch.position;
                startTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endPoint = touch.position;
                endTime = Time.time;
                DetectSwipe();
            }
        }
        
        // Mouse Input for Editor Testing
        if (Input.GetMouseButtonDown(0))
        {
            startPoint = Input.mousePosition;
            startTime = Time.time;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endPoint = Input.mousePosition;
            endTime = Time.time;
            DetectSwipe();
        }
    }

    private void DetectSwipe()
    {
        float distance = Vector2.Distance(startPoint, endPoint);
        float time = endTime - startTime;

        if (distance > minSwipeDistance && time < maxTime)
        {
            Vector2 direction = endPoint - startPoint;
            
            // Only handle horizontal swipes for paging
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                if (pagingController == null)
                {
                    Debug.LogWarning("Swipe detected but pagingController is not assigned!");
                    return;
                }

                if (direction.x > 0)
                {
                    Debug.Log("Swipe Right -> Previous Page");
                    pagingController.Previous();
                }
                else
                {
                    Debug.Log("Swipe Left -> Next Page");
                    pagingController.Next();
                }
            }
        }
    }
}
