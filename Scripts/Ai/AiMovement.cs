/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiMovement : AiInput
{
    public Vector2 direction = Vector2.right;

    Vector2 currentDirection = Vector2.zero;

    public void Turn()
    {
        direction = new Vector2(-direction.x, direction.y);
    }

    void OnEnable()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Send(direction);
    }
}
