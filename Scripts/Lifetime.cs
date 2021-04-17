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
        Destroy(gameObject);
    }
}
