/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerActions : TriggerBase
{
    public UnityEvent<GameObject> OnEnter;
    public UnityEvent<GameObject> OnExit;

    private void Start()
    {
        
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    protected override void ObjectEntered(GameObject otherObject)
    {
        OnEnter?.Invoke(gameObject);
    }

    protected override void ObjectExited(GameObject otherObject)
    {
        OnExit?.Invoke(gameObject);
    }
}
