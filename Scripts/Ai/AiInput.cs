/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiInput : MonoBehaviour
{
    public string actionName;
    public GameObject target;

    string messageName = "";

    // Start is called before the first frame update
    void Awake()
    {
        messageName = "On" + actionName;
    }

    protected void Send<T>(T value)
    {
        if (target == null)
        {
            BroadcastMessage(messageName, value, SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            target.BroadcastMessage(messageName, value, SendMessageOptions.DontRequireReceiver);
        }
    }
}
