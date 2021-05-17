/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerMessages : MonoBehaviour
{
    public enum Event
    {
        TriggerEnter,
        TriggerExit,
        Collision
    }

    public Event triggerEvent = Event.TriggerEnter;
    public string message = "";
    public string targetTag = "Player";
    public bool broadcast = false;

    private void Start()
    {
        
    }

    void Send(GameObject target)
    {
        if (message != "")
        {
            if (targetTag == "" || target.tag == targetTag)
            {
                if (broadcast)
                {
                    target.BroadcastMessage(message, gameObject, SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    target.SendMessage(message, gameObject, SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enabled && triggerEvent == Event.TriggerEnter)
        {
            Send(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (enabled && triggerEvent == Event.TriggerExit)
        {
            Send(collision.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (enabled && triggerEvent == Event.Collision)
        {
            Send(collision.gameObject);
        }
    }
}
