/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaycastBehavior : MonoBehaviour
{
    public float directionDegrees = 0;
    public float length = 3;
    public bool followMovement = true;

    public string targetTag = "";

    public string[] ignoreTags;

    public UnityEvent hitActions;

    Rigidbody2D parentRigidbody;

    public Vector2 direction { 
        get {
            float angle = directionDegrees * Mathf.PI / 180f;
            
            return transform.TransformDirection(new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0));
        } 
    }
    

    // Start is called before the first frame update
    void Start()
    {
        parentRigidbody = GetComponentInParent<Rigidbody2D>();    
    }

    // Update is called once per frame
    void Update()
    {
        bool oldVal = Physics2D.queriesStartInColliders;
        Physics2D.queriesStartInColliders = false;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, length);
        Physics2D.queriesStartInColliders = oldVal;
        if (hit.collider != null && (hit.collider.tag == targetTag || targetTag == ""))
        {
            if (parentRigidbody != null && parentRigidbody == hit.rigidbody) {
                return;
            }

            foreach(string ignoreTag in ignoreTags) {
                if (hit.collider.tag == ignoreTag) {
                    return;
                }
            }

            hitActions?.Invoke();
        }
    }
}
