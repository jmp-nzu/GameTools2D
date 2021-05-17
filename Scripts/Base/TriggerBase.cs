/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerBase : MonoBehaviour
{
    public string targetTag;
    List<GameObject> objectsInTrigger = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected abstract void ObjectEntered(GameObject gameObject);
    protected abstract void ObjectExited(GameObject gameObject);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        bool newObject = !objectsInTrigger.Contains(collision.gameObject);

        if (enabled && (targetTag == "" || collision.gameObject.tag == targetTag) && newObject)
        {
            ObjectEntered(gameObject);
        }

        objectsInTrigger.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        objectsInTrigger.Remove(collision.gameObject);
        bool objectExited = !objectsInTrigger.Contains(collision.gameObject);

        if (enabled && (targetTag == "" || collision.gameObject.tag == targetTag) && objectExited)
        {
            ObjectExited(collision.gameObject);
        }
    }
}
