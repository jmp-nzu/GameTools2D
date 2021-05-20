/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsBehavior : AiInput
{
    public Transform moveTarget;
    public bool isStatic = false;
    public float minDistance = 0.1f;

    // Update is called once per frame
    void Update()
    {
        if (moveTarget != null)
        {
            Vector2 delta = moveTarget.position - transform.position;
            Vector2 direction = Vector2.zero;
            if (delta.x < -minDistance)
            {
                direction.x = -1;
            }
            else if (delta.x > minDistance)
            {
                direction.x = 1;
            }
            if (delta.y < -minDistance)
            {
                direction.y = -1;
            }
            else if (delta.x > minDistance)
            {
                direction.y = 1;
            }

            Send(direction);

            if (isStatic)
            {
                Send(Vector2.zero);
            }
        }
        
    }
}
