using System.Collections;
using System.Collections.Generic;
using MyBase.ApplicationEvent;
using UnityEngine;

public class Spawner : MonoBehaviour {

    public enum GizmoType { Never, SelectedOnly, Always }

    public Transform parent;
    public Boid prefab;
    public float spawnRadius = 10;
    public int spawnCount = 10;
    public Color colour;
    public GizmoType showSpawnRegion;

    void Awake () 
    {
        Spawn();

        BoidManager.BoidManagerAction += TestEvent;
    }

    private void TestEvent(bool test) {
        ApplicationEventManager.On("ev1", (x)=>
        {
            Debug.Log("BOID TEST EV " + test +x);
        }, this);
    }

    void Spawn()
    {
        for (int i = 0; i < spawnCount; i++) {
            Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
            Boid boid = Instantiate(prefab);
            boid.transform.SetParent(parent);
            boid.transform.position = pos;
            boid.transform.forward = Random.insideUnitSphere;

            boid.SetColour (colour);
        }
    }

    private void OnDrawGizmos () 
    {
        if (showSpawnRegion == GizmoType.Always) 
        {
            DrawGizmos ();
        }
    }

    private void OnDrawGizmosSelected () 
    {
        if (showSpawnRegion == GizmoType.SelectedOnly) 
        {
            DrawGizmos();
        }
    }

    private void DrawGizmos () {

        Gizmos.color = new Color (colour.r, colour.g, colour.b, 0.3f);
        Gizmos.DrawSphere (transform.position, spawnRadius);
    }

}