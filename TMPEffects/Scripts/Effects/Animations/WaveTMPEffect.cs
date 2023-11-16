using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[TMPEffect(tag: "wave")]
[CreateAssetMenu(fileName = "new WaveTMProEffect", menuName = "TMPEffects/Effects/Wave")]
public class WaveTMPEffect : TMPEffect
{
    [SerializeField] private float initialSpeed;
    [SerializeField] private float initialFrequency;
    [SerializeField] private float initialAmplitude;

    private float frequency;
    private float amplitude;
    private float speed;

    public override void Apply(ref CharData cData)
    {
        for (int i = 0; i < 4; i++)
        {
            float yOffset = amplitude * (Mathf.Sin(Time.time * speed + cData.index * frequency + Mathf.PI / 2) + 1);
            cData.currentMesh.SetPosition(i, cData.initialMesh.GetPosition(i) + Vector3.up * yOffset);
        }
    }

    public override void SetParameters(Dictionary<string, string> parameters)
    {
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
