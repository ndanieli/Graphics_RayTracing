using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RTManagerScript : MonoBehaviour {

    public ShootFromOrigin origin;

	public void Shoot(GameObject go)
    {
        Ray ray = new Ray();
        ray.origin = origin.transform.position;
        ray.direction = go.transform.position - origin.transform.position;
        ray.direction = ray.direction.normalized;

        destroyNewGameObject();
        origin.ResetLR();
        origin.TraceRay(ray, true);
    }

    private void destroyNewGameObject()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag("new");

        for (int i = 0; i < gameObjects.Length; i++)
        {
            Destroy(gameObjects[i]);
        }
    }
}
