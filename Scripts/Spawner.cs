using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject spawnPrefab;
    
    public void Spawn()
    {
        if (spawnPrefab != null) {
            GameObject obj = Instantiate(spawnPrefab);
            obj.transform.parent = transform.parent;
            obj.transform.position = transform.position;
            obj.transform.rotation = transform.rotation;
        }
    }
}
