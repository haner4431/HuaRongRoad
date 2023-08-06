using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewJelly : MonoBehaviour
{
    Material newJellyMat;
    public Vector3 lastPostion;

    private void Awake()
    {
        newJellyMat = GetComponent<MeshRenderer>().material;
        lastPostion = transform.position;
    }

    private void FixedUpdate()
    {
        newJellyMat.SetFloat("_SizeY", GetComponent<MeshRenderer>().bounds.max.y);
        newJellyMat.SetVector("_LastCenter", new Vector4(lastPostion.x, lastPostion.y, lastPostion.z, 1));
        lastPostion = transform.position;
    }
}
