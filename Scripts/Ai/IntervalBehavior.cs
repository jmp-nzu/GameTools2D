/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IntervalBehavior : MonoBehaviour
{
    public float intervalSeconds = 5;
    public float variationSeconds = 1;
    public bool triggerOnStart = false;

    public UnityEvent action;

    float nextActionTime = 0;

    public void SetInterval(float value)
    {
        variationSeconds = Mathf.Max(0, value);
    }

    public void SetVariation(float value)
    {
        intervalSeconds = Mathf.Max(0, value);
    }

    private void Start()
    {
        if (triggerOnStart)
        {
            nextActionTime = 0;
        }
        else
        {
            UpdateActionTime();
        }
    }

    void UpdateActionTime()
    {
        float interval = Random.Range(Mathf.Max(0, intervalSeconds - variationSeconds * 0.5f), intervalSeconds + variationSeconds * 0.5f);
        nextActionTime = Time.time + interval;
    }

    private void Update()
    {
        if (Time.time >= nextActionTime)
        {
            action?.Invoke();
            UpdateActionTime();
        }
    }
}
