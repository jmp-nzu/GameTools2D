/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(RaycastBehavior))]
[CanEditMultipleObjects]
public class RaycastBehaviorEditor : Editor
{
    private void OnEnable()
    {
        
    }

    public void OnSceneGUI()
    {
        RaycastBehavior t = (target as RaycastBehavior);
        Color color = Color.red;
        Handles.color = color;
        Handles.DrawLine(t.transform.position, t.transform.position + (Vector3)t.direction * t.length, 3);
    }
}
