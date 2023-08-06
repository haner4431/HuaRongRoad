using System;
using System.Collections;
using System.Collections.Generic;
using LFrame.Core.Tools;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TipsRenderer : MonoBehaviour
{
    public enum RenderStyle
    {
        Line,
        Effect
    }

    [SerializeField] private RenderStyle renderStyle = RenderStyle.Line;
    public GameObject renderEffectPrefab ;
    private List<GameObject>  allRenderEffect;
    private LineRenderer lineRenderer;

    private void Awake()
    {
        
        lineRenderer = GetComponent<LineRenderer>();
        allRenderEffect = new List<GameObject>(9);
        for (int i = 0; i < 9; i++)
        {
            var go = Instantiate(renderEffectPrefab, transform);
            Helper.SetActiveState(go,false);
            allRenderEffect.Add(go);
        }
        lineRenderer.positionCount = 0;
    }

    public void Render(params Vector3[] postions)
    {
        lineRenderer.positionCount =0;
        switch (renderStyle)
        {
            case RenderStyle.Line:
            {
                RenderLine(postions);
                break;
            } 
            case RenderStyle.Effect:
            {
                RenderEffect(postions);
                break;
            }
        }
    }

    private void RenderLine(Vector3[] postions)
    {
        if (lineRenderer != null && postions.Length >1 )
        {
            lineRenderer.positionCount = postions.Length;
            lineRenderer.SetPositions(postions);
        }
       
    }


    private void RenderEffect(Vector3[] postions)
    {
        if (lineRenderer == null )return;
        for (int i = 0; i < allRenderEffect.Count; i++)
        {
            if (i < postions.Length)
            {
                Helper.SetActiveState(allRenderEffect[i],true);
                allRenderEffect[i].transform.position = postions[i];
            }
            else
            {
                Helper.SetActiveState(allRenderEffect[i],false);
            }
        }
    }
}
