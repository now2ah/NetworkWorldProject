using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppInitiator : MonoBehaviour
{
    [SerializeField] private List<GameObject> managerList;

    private void Start()
    {
        StartCoroutine(LoadManagersCoroutine());
    }

    IEnumerator LoadManagersCoroutine()
    {
        foreach (GameObject manager in managerList)
        {
            Instantiate(manager);
            yield return null;
        }
        SceneManager.LoadScene(1);
    }
}
