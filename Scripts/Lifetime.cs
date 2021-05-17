/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    public float duration = 1f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EndLifetime());
    }

    IEnumerator EndLifetime()
    {
        yield return new WaitForSeconds(duration);
        EndNow();
    }

    public void EndNow()
    {
        Destroy(gameObject);
    }
}
