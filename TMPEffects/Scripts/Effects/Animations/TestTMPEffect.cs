using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new TestTMProEffect", menuName = "TMPEffects/Effects/Test")]
public class TestTMProEffect : TMPAnimationParameterless

{
    [SerializeField] Color32 color;

    [SerializeField] private float _speed = 10;
    [SerializeField] private float _frequency = 10;
    [SerializeField] private Color32 lerpColor;

    public override void Animate(ref CharData cData, AnimationContext context)
    {
        for (int i = 0; i < 4; i++)
        {
            Color32 c = Color.red;
            cData.currentMesh.SetColor(i, c);
        }

        //for (int i = 0; i < 4; i++)
        //{
        //    Color32 c = new Color32
        //    (
        //        (byte)Mathf.Lerp(0, 255, Mathf.PingPong(Time.time, 1)),
        //        (byte)Mathf.Lerp(0, 255, Mathf.PingPong(Time.time, 1)),
        //        (byte)Mathf.Lerp(0, 255, Mathf.PingPong(Time.time, 1)),
        //        (byte)Mathf.Lerp(0, 255, Mathf.PingPong(Time.time, 1))
        //    );

        //    cData.currentMesh.SetColor(i, c);
        //}
    }

    public override void ResetVariables()
    { }
}