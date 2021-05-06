using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{

    Vector3 spawnPosition;
    bool hasSpawnPosition = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Character character = player.GetComponent<Character>();
            character.OnDied += Reload;
        }
    }

    public void Reload()
    {
        StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine()
    {
        yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);

        if (hasSpawnPosition)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                Character character = player.GetComponent<Character>();
                character.OnStart += () => {
                    character?.Respawn(spawnPosition);
                };
                
            }
        }
    }

    public void LoadScene(string name)
    {
        SceneManager.LoadScene(name);

        hasSpawnPosition = false;
    }

    public void SetSpawnPosition(GameObject obj)
    {
        hasSpawnPosition = true;
        spawnPosition = obj.transform.position;
    }
}
