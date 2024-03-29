﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class DrawLine : MonoBehaviour
{
    //Line
    public LineRenderer lineRenderer;

    [SerializeField]
    private Transform pointParent;
    private Transform[] points;
    public Transform[] busstops;


    private Navigation nav;
    private Transform player;

    int busStopIndexinIndex = 0;

    //marcus market destIndex
    int marcusIndex;

    //distance
    public float disFromPlayertoPoint;
    public float disFromBustoDest;




    void Start()
    {
        //point
        points = new Transform[pointParent.childCount];
        int i = 0;
        foreach (Transform child in pointParent)
        {
            points[i] = child;
            i++;
        }

        nav = FindObjectOfType<Navigation>();     
        player = GameObject.FindWithTag("Player").GetComponent<Transform>();

        marcusIndex = 26;
        nav.reDrawLineEvt += new EventHandler(DrawDottedLine);
        nav.reDrawLineEvt += new EventHandler(DrawNavLine);
        //nav.reDrawLineEvt += new EventHandler(SendDistance);
    }


    public void DrawDottedLine(object sender, EventArgs e)
    {
        disFromPlayertoPoint = 0.0f;

        reDrawLineEvtArgs args = e as reDrawLineEvtArgs;
        Vector3 playerPos = player.position;
        int pointIndex = args.nearIndex;
        int busStopIndex = args.nearBusStopIndex;
        Vector3 pointPos = args.nearPointPos;
        Vector3 busPos = args.nearBusStopPos;
        int busStopIndexinIndex = args.busStopIndexinIndex;
        

        //player -> near Point
        disFromPlayertoPoint = DottedLine.DottedLine.Instance.DrawDottedLineFromPlayer(playerPos, pointPos);
            

        //near Point -> next Point -> ... -> bus stop
        if (pointIndex <= busStopIndexinIndex)
        {
            for (int i = pointIndex; i < busStopIndexinIndex; i++)
            {
                int next = i + 1;
                disFromPlayertoPoint +=
                    DottedLine.DottedLine.Instance.DrawDottedLineFromObj(points[i].position, points[next].position);
                
            }
        }
        else
        {
            for (int i = busStopIndexinIndex; i < pointIndex; i++)
            {
                int next = i + 1;
                disFromPlayertoPoint +=
                    DottedLine.DottedLine.Instance.DrawDottedLineFromObj(points[i].position, points[next].position);
            }
        }

        Debug.Log($"disFromPlayertoPoint: {disFromPlayertoPoint}");

        //bus stop -> dest
        

        DottedLine.DottedLine.Instance.Render();


    }

    public void DrawNavLine(object sender, EventArgs e)
    {
        disFromBustoDest = 0.0f;
        reDrawLineEvtArgs args = e as reDrawLineEvtArgs;

        int minIndex = args.nearIndex;
        int busStopIndexinIndex = args.busStopIndexinIndex;
        int busStopIndex = args.nearBusStopIndex;

        lineRenderer.material.SetColor("_Color", Color.red);
        lineRenderer.startWidth = 5f;
        lineRenderer.endWidth = 5f;

        {
            

            int num = 0;
            //set the position
            if (busStopIndexinIndex <= marcusIndex)
            {
                lineRenderer.positionCount = marcusIndex - busStopIndexinIndex + 1;

                for (int i = busStopIndexinIndex; i < marcusIndex + 1; i++)
                {
                    lineRenderer.SetPosition(num++, points[i].position);
                    int next = i + 1;
                    if (i != marcusIndex)
                    {
                        disFromBustoDest += Vector3.Distance(points[i].position, points[next].position);
                    }
                    

                }
            }
            else //지나쳤을 경우
            {
                lineRenderer.positionCount = marcusIndex + (points.Length - busStopIndexinIndex) + 1;

                for (int i = busStopIndexinIndex; i < points.Length; i++)
                {
                    lineRenderer.SetPosition(num++, points[i].position);
                    int next = i + 1;
                    if (i != points.Length - 1)
                    {
                        disFromBustoDest += Vector3.Distance(points[i].position, points[next].position);
                    }

                }

                for (int i = 0; i < marcusIndex + 1; i++)
                {
                    lineRenderer.SetPosition(num++, points[i].position);
                    int next = i + 1;
                    if (i != marcusIndex)
                    {
                        disFromBustoDest += Vector3.Distance(points[i].position, points[next].position);
                    }

                }

            }
            
        }
        
        
        Debug.Log($"disFromBustoDest : {disFromBustoDest}");


    }


}
