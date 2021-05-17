/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float dammage = 1;
    public float bounceThresholdDegrees = 30;
    public string[] ignoreTags;
    public GameObject explosion;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (string ignoreTag in ignoreTags)
        {
            if (ignoreTag == collision.collider.tag)
            {
                return;
            }
        }

        float maxNormalY = Mathf.Sin(Mathf.PI * 0.5f + bounceThresholdDegrees * Mathf.PI / 180f);
        foreach(ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y <= maxNormalY)
            {
                if (explosion != null)
                {
                    GameObject explosionObject = Instantiate(explosion);
                    explosionObject.transform.position = contact.point;

                    collision.collider.gameObject.BroadcastMessage("OnHit", dammage, SendMessageOptions.DontRequireReceiver);

                    Destroy(gameObject);

                    break;
                }
            }
        }
    }
}
