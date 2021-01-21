using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AmmoCount : MonoBehaviour
{
    [SerializeField] TextMeshPro ui;

    public void UpdateText(int roundsRemaing, int magSize) //needs to be called when weapon is first equipped
    {
        ui.text = roundsRemaing.ToString() + "/" + magSize.ToString();
    }


}
