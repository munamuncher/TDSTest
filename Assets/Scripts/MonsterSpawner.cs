using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] spawnPoints;
    [SerializeField]
    private GameObject zombie;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            int ran = Random.Range(0, spawnPoints.Length);
            GameObject Monster = Instantiate(zombie, spawnPoints[ran].transform.position, Quaternion.identity);
            Monster.GetComponent<MonsterMovementAI>().ReceiveLayer(ran + 8);
        }
    }

}
