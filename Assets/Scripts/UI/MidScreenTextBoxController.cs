using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class MidScreenTextBoxController : MonoBehaviour
{

    [SerializeField] PhotonView PV;
    [SerializeField] PlayerController player;
    TextMeshPro textObject;
    [SerializeField] GameObject reticle;

    // Start is called before the first frame update
    void Start()
    {
            textObject = GetComponent<TextMeshPro>();

        if (player.GetPlayerManager().kills < 1)
        {
            string startText = "First to " + ModeDeathMatch.Instance.GetScoreLimit() + " points wins!";
            textObject.text = startText;
            Invoke("EraseText", 3);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateText(string text)
    {

    }

    public void EraseText()
    {
        textObject.text = string.Empty;
        reticle.SetActive(true);
    }
}
