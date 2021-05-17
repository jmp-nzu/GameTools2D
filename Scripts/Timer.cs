/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    public float seconds = 10f;
    public bool oneShot = true;
    public UnityEvent actions;

    float time = 0;

    public float RemainingTime
    {
        get
        {
            return seconds - time;
        }
    }

    public void Reset()
    {
        time = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time >= seconds)
        {
            actions.Invoke();

            if (oneShot)
            {
                time = 0;
                enabled = false;
            }
            else
            {
                time = time % seconds;
            }
        }
    }
}
