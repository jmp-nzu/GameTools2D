/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FaceMovementBehavior : MonoBehaviour
{
    public bool ignoreXaxis = false;
    public bool ignoreYaxis = true;

    Vector2 previousMovement = Vector2.zero;

    void Start()
    {
        
    }

    void OnMove(Vector2 moveDirection)
    {
        if (!enabled)
        {
            return;
        }

        if (ignoreXaxis)
        {
            moveDirection.x = 0;
        }
        if (ignoreYaxis)
        {
            moveDirection.y = 0;
        }

        if (moveDirection.magnitude > 0 && moveDirection != previousMovement)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x);
            Vector3 newEulerAngles = transform.localEulerAngles;
            if (!ignoreXaxis) 
            {
                if (moveDirection.x < 0)
                {
                    newEulerAngles.y = 180;
                }
                else if (moveDirection.x > 0)
                {
                    newEulerAngles.y = 0;
                }
            }
            if (!ignoreYaxis)
            {
                newEulerAngles.z = Mathf.Asin(moveDirection.y) * 180f / Mathf.PI;
            }

            transform.localEulerAngles = newEulerAngles;

            previousMovement = moveDirection;
        }
    }

    void OnMove(InputValue input)
    {
        OnMove(input.Get<Vector2>());
    }
}
