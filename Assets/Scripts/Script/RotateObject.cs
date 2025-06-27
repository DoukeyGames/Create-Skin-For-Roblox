using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    private Touch _touch;

    private Vector2 touchPosition;

    private Quaternion rotationY;

    private float rotationSpeedModifier = 0.5f;
    
  

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount > 0)
        {
            _touch = Input.GetTouch(0);

            if (_touch.phase == TouchPhase.Moved)
            {
                rotationY = Quaternion.Euler(0f, - _touch.deltaPosition.x * rotationSpeedModifier, 0f);

                transform.rotation = rotationY * transform.rotation;
            }
        }
    }
}
