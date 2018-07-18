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

    public void TraceRay(Ray ray, bool isOrigin)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitGO = hit.transform.gameObject;
            Vector3 hitPosition = hit.transform.position;
            Color endColor = hit.transform.gameObject.GetComponent<Renderer>().material.color;

            StartCoroutine(waitAndShootAtPoint(hitPosition, endColor));
            if (!hitGO.tag.Equals("Light"))
            {
                createNewGameObject(hit, hitGO, isOrigin);
            }
        }
        else
        {
            Vector3 rayToNowhere = ray.origin + (ray.direction * 200);
            StartCoroutine(waitAndShootAtPoint(rayToNowhere, Color.grey));
        }
    }

    private static void createNewGameObject(RaycastHit hit, GameObject hitGO, bool isOrigin)
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
        goShoot.shootFromNewPoint(newRay, isOrigin);
    }

    public void shootFromNewPoint(Ray newRay, bool isOrigin)
    {
        StartCoroutine(sendRaysFromNewHitPoint(newRay, isOrigin));
    }

    public void ResetLR()
    {
        alr.Reset();
    }

    IEnumerator sendRaysFromNewHitPoint(Ray newRay, bool isOrigin)
    {
        yield return new WaitForSeconds(0.5f);
        TraceRay(newRay, false);

        if (isOrigin)
        {
            yield return new WaitForSeconds(1.5f);
            shootLights();
        }
    }

    private void shootLights()
    {
        foreach (GameObject light in lm.lights)
        {
            StartCoroutine(waitAndShootAtLight(light));
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

    IEnumerator waitAndShootAtLight(GameObject lightGO)
    {
        yield return new WaitForSeconds(0.5f);

        GameObject go = new GameObject();
        go.name = "lightGO";
        go.tag = "new";
        go.transform.position = gameObject.transform.position;
        go.transform.rotation = gameObject.transform.rotation;
        LineRenderer lightLr = go.gameObject.AddComponent<LineRenderer>();
        AnimatedLineRenderer lightAlr = go.gameObject.AddComponent<AnimatedLineRenderer>();
        lightAlr.setLineRenderer(lightLr);
        lightAlr.LineRenderer.material = new Material(Shader.Find("Particles/Standard Unlit"));

        lightAlr.StartWidth = alr.StartWidth;
        lightAlr.EndWidth = alr.EndWidth;
        lightAlr.SecondsPerLine = alr.SecondsPerLine;
                
       

        Ray ray = new Ray();
        ray.origin = go.transform.position;
        ray.direction = lightGO.transform.position - go.transform.position;
        ray.direction = ray.direction.normalized;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Color color = Color.gray;
            if (hit.transform.gameObject.tag.Equals("Light"))
            {
                color = hit.transform.gameObject.GetComponent<Renderer>().material.color;
                
            }
            lightAlr.StartColor = color;
            lightAlr.EndColor = color;

            lightAlr.Enqueue(go.transform.position);
            lightAlr.Enqueue(hit.transform.position);
        }
    }


}
