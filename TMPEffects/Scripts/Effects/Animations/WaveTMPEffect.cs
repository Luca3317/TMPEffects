using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "new WaveTMProEffect", menuName = "TMPEffects/Effects/Wave")]
public class WaveTMPEffect : TMPAnimation
{
    [SerializeField] private float initialSpeed;
    [SerializeField] private float initialFrequency;
    [SerializeField] private float initialAmplitude;

    private float frequency;
    private float amplitude;
    private float speed;

    public override void Animate(ref CharData cData, AnimationContext context)
    {
        float scale = cData.pointSize / 36f;

        float xPos = (cData.currentMesh.vertex_TL.position.x + cData.currentMesh.vertex_TR.position.x) / 2;
        Vector3 currentCenter = Vector3.zero;
        for (int i = 0; i < 4; i++)
        {
            currentCenter += cData.currentMesh.GetPosition(i);
        }


        for (int i = 0; i < 4; i++)
        {
            float yOffset = amplitude * (Mathf.Sin((context.useScaledTime ? Time.time : Time.unscaledTime) * speed + (xPos / (200 * (context.scaleAnimations ? scale : 1)))/*  cData.index*/ * frequency + Mathf.PI / 2) + 1) * (context.scaleAnimations ? scale : 1);
            cData.currentMesh.SetPosition(i, cData.initialMesh.GetPosition(i) + Vector3.up * yOffset);
        }
        //float scale = cData.pointSize / 36f;

        //for (int i = 0; i < 4; i++)
        //{
        //    float xPos = (cData.initialMesh.vertex_TL.position.x + cData.initialMesh.vertex_TR.position.x) / 2;
        //    float yOffset = amplitude * (Mathf.Sin( (context.useScaledTime ? Time.time : Time.unscaledTime) * speed + (xPos / (200 * (context.scaleAnimations ? scale : 1)))/*  cData.index*/ * frequency + Mathf.PI / 2) + 1) * (context.scaleAnimations ? scale : 1);
        //    cData.currentMesh.SetPosition(i, cData.initialMesh.GetPosition(i) + Vector3.up * yOffset);
        //}
    }

    public override void SetParameters(Dictionary<string, string> parameters)
    {
        if (parameters == null) return;

        foreach (var kvp in parameters)
        {
            switch (kvp.Key)
            {
                case "s":
                case "sp":
                case "speed": speed = float.Parse(kvp.Value); break;

                case "f":
                case "fq":
                case "frequency": frequency = float.Parse(kvp.Value); break;

                case "a":
                case "amp":
                case "amplitude": amplitude = float.Parse(kvp.Value); break;
            }
        }
    }

    public override void ResetVariables()
    {
        frequency = initialFrequency;
        amplitude = initialAmplitude;
        speed = initialSpeed;
    }

    public override bool ValidateParameters(Dictionary<string, string> parameters)
    {
        if (parameters == null)
            return true;

        foreach (var kvp in parameters)
        {
            switch (kvp.Key)
            {
                case "s":
                case "sp":
                case "speed":
                case "f":
                case "fq":
                case "frequency":
                case "a":
                case "amp":
                case "amplitude": if (!float.TryParse(kvp.Value, out _)) return false; break;
            }
        }

        return true;
    }

    public override void SetParameter<T>(string name, T value)
    {

    }
}
