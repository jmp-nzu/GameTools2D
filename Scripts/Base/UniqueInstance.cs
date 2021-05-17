/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public abstract class UniqueInstance : MonoBehaviour
{
    public string uid = "";

    void Start()
    {
        if (!Application.IsPlaying(gameObject))
        {
            uid = System.Guid.NewGuid().ToString();
            EditorUtility.SetDirty(this);
        }
    }
}
