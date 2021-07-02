using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class MainMono : MainBase
{
    private List<Transform> goes;

    public override void CreateObject(Material mat, int count, Mesh mesh)
    {
        goes = new List<Transform>(count);
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = UnityEngine.Random.insideUnitSphere * 40;

            var go = new GameObject();
            var mr = go.AddComponent<MeshRenderer>();
            mr.material = mat;
            mr.receiveShadows = true;
            mr.shadowCastingMode = ShadowCastingMode.On;

            var mf = go.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            
            go.transform.position = pos;
            go.transform.rotation = Quaternion.identity;

            goes.Add(go.transform);
        }

        GameObject.Destroy(cube);
    }

    public override void Update()
    {
        foreach (var go in goes)
        {
            Quaternion q = go.rotation;
            var ang = q.eulerAngles;
            ang.y = ang.y + Time.deltaTime;
            go.rotation = Quaternion.Euler(ang);

            Vector3 pos = UnityEngine.Random.insideUnitSphere * 40;
            go.position = pos;
        }
    }
}