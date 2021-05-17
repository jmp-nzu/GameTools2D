/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/. */
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public string targetTag = "Player";
    public Transform spawnPoint = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == targetTag && SceneControl.singleton != null)
        {
            Character character = collision.gameObject.GetComponent<Character>();
            Inventory inventory = collision.gameObject.GetComponent<Inventory>();

            if (character != null)
            {
                Vector3 spawnPosition = transform.position;
                if (spawnPoint != null)
                {
                    spawnPosition = spawnPoint.position;
                }
                SceneControl.singleton.SetSpawnPosition(character, spawnPosition);
            }

            if (inventory != null)
            {
                SceneControl.singleton.SaveInventoryItems(inventory);
            }
        }
    }
}
