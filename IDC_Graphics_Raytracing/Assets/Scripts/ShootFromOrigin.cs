using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.AnimatedLineRenderer;
using System;

public class ShootFromOrigin : MonoBehaviour {

    private Vector3[] linePositions;
    private AnimatedLineRenderer alr;
    private LineRenderer lr;
    private LightsManager lm;

    // Use this for initialization
    void Awake () {
        lr = gameObject.AddComponent<LineRenderer>();
        alr = gameObject.AddComponent<AnimatedLineRenderer>();
        lm = FindObjectOfType<LightsManager>();
        
        StartCoroutine(setAlr());
    }

    public void TraceRay(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitGO = hit.transform.gameObject;
            Vector3 hitPosition = hit.transform.position;
            Color endColor = hit.transform.gameObject.GetComponent<Renderer>().material.color;

            StartCoroutine(waitAndShootAtPoint(hitPosition, endColor));
            createNewGameObject(hit, hitGO);
        }
        else
        {
            Vector3 rayToNowhere = ray.origin + (ray.direction * 200);
            StartCoroutine(waitAndShootAtPoint(rayToNowhere, Color.grey));
        }
    }

    private static void createNewGameObject(RaycastHit hit, GameObject hitGO)
    {
        GameObject go = new GameObject();

        go.tag = "new";
        go.transform.position = hit.transform.position;
        go.transform.rotation = hit.transform.rotation;

        go.AddComponent<MeshRenderer>();
        go.GetComponent<Renderer>().materials = new Material[] { hitGO.GetComponent<Renderer>().materials[0] };
        go.GetComponent<Renderer>().materials[0] = hitGO.GetComponent<Renderer>().materials[0];

        Ray newRay = new Ray();
        newRay.origin = go.transform.position;
        newRay.direction = hit.normal;

        ShootFromOrigin goShoot = go.AddComponent<ShootFromOrigin>();
        goShoot.shootFromNewPoint(newRay);
    }

    public void shootFromNewPoint(Ray newRay)
    {
        StartCoroutine(sendRaysFromNewHitPoint(newRay));
    }

    public void ResetLR()
    {
        alr.Reset();
    }

    IEnumerator sendRaysFromNewHitPoint(Ray newRay)
    {
        yield return new WaitForSeconds(0.5f);
        //shootLights();
        TraceRay(newRay);
    }

    private void shootLights()
    {
        foreach (GameObject go in lm.lights)
        {
            Vector3 pos = go.transform.position;
            StartCoroutine(waitAndShootAtLight(pos));
        }

    }

    IEnumerator setAlr()
    {
        yield return new WaitForSeconds(0.2f);

        alr.SecondsPerLine = 0.5f;
        alr.StartWidth = 0.5f;
        alr.EndWidth = 0.5f;
        alr.LineRenderer.material = new Material(Shader.Find("Particles/Standard Unlit"));
        alr.StartColor = gameObject.GetComponent<Renderer>().material.color;
    }

    IEnumerator waitAndShootAtPoint(Vector3 pos, Color endColor)
    {
        yield return new WaitForSeconds(0.5f);
        alr.EndColor = endColor;
        alr.Enqueue(transform.position);
        alr.Enqueue(pos);
    }

    IEnumerator waitAndShootAtLight(Vector3 pos)
    {
        yield return new WaitForSeconds(0.1f);
        alr.StartColor = Color.white;
        alr.EndColor = Color.white;
        alr.Enqueue(transform.position);
        alr.Enqueue(pos);
    }


}
