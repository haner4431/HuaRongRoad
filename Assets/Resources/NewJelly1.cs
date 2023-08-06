using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewJelly1 : MonoBehaviour
{
    Material newJellyMat;
    public Vector3 lastPostion;
    public float stiffness = 1;
    public float damping = 0.75f;
    public float mass = 1;
    public Vector3 velocity=new Vector3(0,0,0);
    public Vector3 Force;
    public Vector3 abstractPos;

    private void Awake()
    {
        newJellyMat = GetComponent<MeshRenderer>().material;
        lastPostion = transform.position;
        abstractPos = transform.position;
    }

    private void FixedUpdate()
    {
        newJellyMat.SetFloat("_SizeY", GetComponent<MeshRenderer>().bounds.max.y);

        Force = (lastPostion- abstractPos) * stiffness;
        velocity = (velocity + Force / mass) * damping;
        abstractPos += velocity;
        newJellyMat.SetVector("_AbstracPos", transform.InverseTransformPoint( abstractPos));
        lastPostion = transform.position;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(abstractPos, 0.01f);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(abstractPos, velocity + abstractPos);
    }
}
