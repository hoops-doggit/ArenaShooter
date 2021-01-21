using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BouncyUi : MonoBehaviourPunCallbacks
{
    public Transform scalePivot;

    bool doBounce = false;
    float amount;
    float duration = 0;
    float curDuration = 0;

    public void BounceUI(float _duration, float _amount)
    {
        duration = _duration;
        amount = _amount;
        curDuration = 0;
        doBounce = true;
        Debug.Log("do bounce");
    }

    void Update()
    {
        if (doBounce)
        {

            scalePivot.localScale = Vector3.Lerp(Vector3.one * amount, Vector3.one, curDuration / duration);
            curDuration += Time.deltaTime;
            if (curDuration > duration)
            {
                scalePivot.localScale = Vector3.one;
                doBounce = false;
            }
        }
    }
}
