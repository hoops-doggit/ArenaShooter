using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilController : MonoBehaviour
{
    [Header("Recoil Settings")]
    public float rotationSpeed = 6;
    public float returnSpeed = 25;
    [Space()]

    //[Header("HipFire")]
   //public Vector3 RecoilRotation = new Vector3(2, 2, 2);

    private Vector3 currentRotation;
    private Vector3 rot;

    public void DoRecoil(Vector3 recoilRotation)
    {
        currentRotation += new Vector3(-recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), Random.Range(-recoilRotation.z, recoilRotation.z));
    }


    void FixedUpdate()
    {
        currentRotation = Vector3.Lerp(currentRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        rot = Vector3.Slerp(rot, currentRotation, rotationSpeed * Time.fixedDeltaTime);
        transform.rotation = Quaternion.Euler(rot);
    }
}
