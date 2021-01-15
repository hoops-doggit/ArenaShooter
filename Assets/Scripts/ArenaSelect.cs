using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class ArenaSelect : MonoBehaviour
{
    public TMP_Dropdown m_dropdown;
    [SerializeField] Launcher m_launcher;


    private void Start()
    {
        m_dropdown = GetComponent<TMP_Dropdown>();

        List<string> arenas = new List<string>();

        Debug.Log("there are " + SceneManager.sceneCountInBuildSettings + "scenes");

        for (int i = 1; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            var path = SceneUtility.GetScenePathByBuildIndex(i);
            var sceneName = path.Substring(0, path.Length - 6).Substring(path.LastIndexOf('/') + 1);
            arenas.Add(sceneName);
            Debug.Log(sceneName);
        }

        m_dropdown.ClearOptions();
        m_dropdown.AddOptions(arenas);
    }

    public void SendInt()
    {
        Debug.Log("the chosen scene is " + m_dropdown.value);
        m_launcher.arenaToLoad = m_dropdown.value + 1;
    }

}
