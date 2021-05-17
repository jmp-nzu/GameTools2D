/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ContactActions : MonoBehaviour
{
    public string targetTag = "";

    public bool destroyOnContact = false;
    public bool useContactDirection = false;
    public float contactDirection = 90;
    public float contactDirectionThreshold = 45;

    public UnityEvent OnContact;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (targetTag == "" || collision.gameObject.tag == targetTag)
        {
            if (useContactDirection)
            {
                for (int i = 0; i < collision.contactCount; i++)
                {
                    ContactPoint2D contact = collision.GetContact(i);
                    float contactAngle = Mathf.Asin(contact.normal.y) * 180f / Mathf.PI;
                    if (Mathf.Abs(contactAngle - contactDirection) <= contactDirectionThreshold)
                    {
                        OnContact?.Invoke();
                        if (destroyOnContact)
                        {
                            Destroy(gameObject);
                        }
                        return;
                    }
                }
            }
            else
            {
                OnContact?.Invoke();
                if (destroyOnContact)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
