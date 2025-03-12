using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    [SerializeField]
    private GameObject tower;

    private static TowerManager instance;
    public static TowerManager TmInstance => instance;

    private void Awake()
    {
        if (instance != this && instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private Vector3 velocity = Vector3.zero;
    private IEnumerator TowerFallCoroutine()
    {
        Vector3 targetPosition = tower.transform.position + new Vector3(0, -1.5f, 0);
        float duration = 1.5f;
        float timePassed = 0f;

        while (timePassed < duration)
        {
            tower.transform.position = Vector3.SmoothDamp(tower.transform.position, targetPosition, ref velocity, 0.3f);
            timePassed += Time.deltaTime;
            yield return null;
        }

        tower.transform.position = targetPosition;
    }

    public void StartFall()
    {
        StartCoroutine(TowerFallCoroutine());
    }
}
