/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BurstBehavior : MonoBehaviour
{
    public int count = 3;
    public int countVariation = 0;
    public float intervalSeconds = 0.2f;
    public float intervalVariation = 0f;

    public UnityEvent actions;

    int index = 0;
    float nextTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void UpdateTime()
    {
        float interval = Random.Range(Mathf.Max(0, intervalSeconds - intervalVariation), intervalSeconds + intervalVariation);
        nextTime = Time.time + interval;
    }

    // Update is called once per frame
    void Update()
    {
        if (index > 0 && Time.time >= nextTime)
        {
            UpdateTime();

            actions?.Invoke();

            index--;
        }
    }

    public void Execute()
    {
        int burstCount = Random.Range(count - countVariation, count + countVariation + 1);

        if (burstCount > 0)
        {
            actions?.Invoke();
        }

        UpdateTime();

        index = burstCount - 1;
    }
}
