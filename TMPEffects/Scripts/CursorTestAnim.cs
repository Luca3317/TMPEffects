using System.Collections;
using System.Collections.Generic;
using TMPEffects;
using UnityEngine;

[CreateAssetMenu(fileName = "cursor aniu", menuName = "TMPEffects/Cursor anim")]
public class CursorTestAnim : TMPAnimationParameterless
{
    Transform obj;
    float stopDist = 20;
    public override void Animate(ref CharData cData, ref IAnimationContext context)
    {
        if (obj == null) return;
        Vector3 pos = obj.position;
        for (int i = 0; i < 4; i++)
        {
            Vector3 dir = cData.mesh.GetPosition(i) - pos;

            float magnitude = dir.magnitude;
            if (magnitude > stopDist) continue;
            float dist = Mathf.Clamp(1f / magnitude, -0.15f, 0.15f);
            cData.mesh.SetPosition(i, cData.mesh.initial.GetPosition(i) + dir.normalized * dist);
            //cData.currentMesh.SetUV(i, cData.initialMesh.GetUV0(i) + new Vector2(obj.position.x, obj.position.y)/100);
            //cData.currentMesh.SetUV2(i, cData.initialMesh.GetUV2(i) + new Vector2(obj.position.x, obj.position.y)/100);
        }

        //if (obj == null) return;
        //Vector3 pos = obj.position;
        //for (int i = 0; i < 4; i++)
        //{
        //    Vector3 dir =  cData.currentMesh.GetPosition(i) - pos;

        //    float magnitude = dir.magnitude;
        //    if (magnitude > stopDist) continue;
        //    float dist = Mathf.Clamp(1f / magnitude, -0.15f, 0.15f);
        //    cData.currentMesh.SetPosition(i, cData.currentMesh.GetPosition(i) + dir.normalized * dist);
        //}
    }

    public override void ResetParameters()
    {
    }

    public void AddObject(Transform obj)
    {
        this.obj = obj;
    }
}
