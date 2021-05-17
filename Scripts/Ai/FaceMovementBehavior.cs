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

    Vector2 offset;
    float length = 0;

    private void Awake()
    {
        offset = transform.localPosition;
        Vector2 v = offset;
        if (ignoreXaxis)
        {
            v.x = 0;
        }
        if (ignoreYaxis)
        {
            v.y = 0;
        }
        length = v.magnitude;
    }

    void OnMove(Vector2 moveDirection)
    {
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
            Vector3 newLocalPosition = Vector3.zero;
            Vector3 newEulerAngles = Vector3.zero;
            if (ignoreXaxis) 
            {
                newLocalPosition.x = offset.x;
            }
            else
            {
                newLocalPosition.x = Mathf.Cos(angle) * length;
                if (moveDirection.x < 0)
                {
                    newEulerAngles.y = 180;
                }
                else if (moveDirection.x > 0)
                {
                    newEulerAngles.y = 0;
                }
            }
            if (ignoreYaxis)
            {
                newLocalPosition.y = offset.y;
            }
            else
            {
                newLocalPosition.y = Mathf.Sin(angle) * length;
                newEulerAngles.z = Mathf.Asin(moveDirection.y) * 180f / Mathf.PI;
            }

            transform.localPosition = newLocalPosition;
            transform.localEulerAngles = newEulerAngles;

            previousMovement = moveDirection;
        }
    }

    void OnMove(InputValue input)
    {
        OnMove(input.Get<Vector2>());
    }
}
