/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TriggerSequence : MonoBehaviour
{
    public GameObject[] sequence;
    public bool ignoreOrder = false;
    public bool resetOnMistake = true;
    public bool resetOnComplete = false;

    public UnityEvent<GameObject> OnComplete;
    public UnityEvent<GameObject, int> OnCorrect;
    public UnityEvent<GameObject, int> OnMistake;
    public UnityEvent<GameObject> OnReset;

    List<GameObject> triggerObjects = new List<GameObject>();
    
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool CheckOrdered(GameObject sender)
    {
        return sequence[triggerObjects.Count] == sender;
    }

    bool CheckNonOrdered(GameObject sender)
    {
        foreach(GameObject obj in sequence)
        {
            if (obj == sender)
            {
                return true;
            }
        }
        return false;
    }

    public void Trigger(GameObject sender)
    {
        if (triggerObjects.Count < sequence.Length)
        {
            bool matches;
            if (ignoreOrder)
            {
                matches = CheckNonOrdered(sender);
            }
            else {
                matches = CheckOrdered(sender);
            }

            if (matches)
            {
                triggerObjects.Add(sender);
                OnCorrect?.Invoke(gameObject, triggerObjects.Count - 1);
                if (triggerObjects.Count == sequence.Length)
                {
                    OnComplete?.Invoke(gameObject);
                    if (resetOnComplete)
                    {
                        Reset();
                    }
                }
            }
            else
            {
                OnMistake?.Invoke(gameObject, triggerObjects.Count - 1);
                if (resetOnMistake)
                {
                    Reset();
                }
            }
        }
    }

    public void Reset()
    {
        triggerObjects.Clear();
        OnReset?.Invoke(gameObject);
    }
}
