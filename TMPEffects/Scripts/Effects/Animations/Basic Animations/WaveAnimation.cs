using System.Collections.Generic;
using UnityEngine;
using TMPEffects.TextProcessing;

namespace TMPEffects.Animations
{
    [CreateAssetMenu(fileName = "new WaveAnimation", menuName = "TMPEffects/Animations/Wave")]
    public class WaveAnimation : TMPAnimation
    {
        [SerializeField] private float speed;
        [SerializeField] private float frequency;
        [SerializeField] private float amplitude;

        private float currentFrequency;
        private float currentAmplitude;
        private float currentSpeed;

        public override void Animate(ref CharData cData, ref IAnimationContext context)
        {
            float xPos = (cData.mesh.initial.vertex_TL.position.x + cData.mesh.initial.vertex_TR.position.x) / 2;
            float yOffset = currentAmplitude * (Mathf.Sin((context.animatorContext.passedTime) * currentSpeed +/* cData.index*/ (xPos / (cData.info.referenceScale /*(cData.ascender - cData.descender)*/ / 36f)) / 200 * currentFrequency + Mathf.PI / 2) + 1) * (/*context.scaleAnimations ? scale :*/ 1);
            cData.SetPosition(cData.info.initialPosition + Vector3.up * yOffset);
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
                    case "speed": ParsingUtility.StringToFloat(kvp.Value, out currentSpeed); break;

                    case "f":
                    case "fq":
                    case "frequency": ParsingUtility.StringToFloat(kvp.Value, out currentFrequency); break;

                    case "a":
                    case "amp":
                    case "amplitude": ParsingUtility.StringToFloat(kvp.Value, out currentAmplitude); break;
                }
            }
        }

        public override void ResetParameters()
        {
            currentFrequency = frequency;
            currentAmplitude = amplitude;
            currentSpeed = speed;
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
                    case "amplitude":
                        if (!ParsingUtility.StringToFloat(kvp.Value, out _)) return false;
                        break;
                }
            }

            return true;
        }
    }


    //[CreateAssetMenu(fileName = "new WaveAnimation", menuName = "TMPEffects/Animations/Wave")]
    //public class WaveAnimation : TMPAnimation
    //{
    //    [SerializeField] private float speed;
    //    [SerializeField] private float frequency;
    //    [SerializeField] private float amplitude;

    //    private float currentFrequency;
    //    private float currentAmplitude;
    //    private float currentSpeed;

    //    public override void Animate(ref CharData cData, ref IAnimationContext context)
    //    {
    //        float xPos = (cData.mesh.initial.vertex_TL.position.x + cData.mesh.initial.vertex_TR.position.x) / 2;
    //        float yOffset = currentAmplitude * (Mathf.Sin((context.AnimatorContext.passedTime) * currentSpeed +/* cData.index*/ (xPos / (cData.info.referenceScale /*(cData.ascender - cData.descender)*/ / 36f)) / 200 * currentFrequency + Mathf.PI / 2) + 1) * (/*context.scaleAnimations ? scale :*/ 1);
    //        cData.position = cData.info.initialPosition + Vector3.up * yOffset;

    //        //float xPos = (cData.initialMesh.vertex_TL.position.x + cData.initialMesh.vertex_TR.position.x) / 2;
    //        //float yOffset = currentAmplitude * (Mathf.Sin((context.AnimatorContext.passedTime) * currentSpeed +/* cData.index*/ (xPos / (cData.initialScale /*(cData.ascender - cData.descender)*/ / 36f)) / 200 * currentFrequency + Mathf.PI / 2) + 1) * (/*context.scaleAnimations ? scale :*/ 1);
    //        //for (int i = 0; i < 4; i++)
    //        //{
    //        //    cData.currentMesh.SetPosition(i, cData.initialMesh.GetPosition(i) + Vector3.up * yOffset);
    //        //}
    //    }

    //    public override void SetParameters(Dictionary<string, string> parameters)
    //    {
    //        if (parameters == null) return;

    //        foreach (var kvp in parameters)
    //        {
    //            switch (kvp.Key)
    //            {
    //                case "s":
    //                case "sp":
    //                case "speed": ParsingUtility.StringToFloat(kvp.Value, out currentSpeed); break;

    //                case "f":
    //                case "fq":
    //                case "frequency": ParsingUtility.StringToFloat(kvp.Value, out currentFrequency); break;

    //                case "a":
    //                case "amp":
    //                case "amplitude": ParsingUtility.StringToFloat(kvp.Value, out currentAmplitude); break;
    //            }
    //        }
    //    }

    //    public override void ResetParameters()
    //    {
    //        currentFrequency = frequency;
    //        currentAmplitude = amplitude;
    //        currentSpeed = speed;
    //    }

    //    public override bool ValidateParameters(Dictionary<string, string> parameters)
    //    {
    //        if (parameters == null)
    //            return true;

    //        foreach (var kvp in parameters)
    //        {
    //            switch (kvp.Key)
    //            {
    //                case "s":
    //                case "sp":
    //                case "speed":
    //                case "f":
    //                case "fq":
    //                case "frequency":
    //                case "a":
    //                case "amp":
    //                case "amplitude": if (!ParsingUtility.StringToFloat(kvp.Value, out _)) return false; break;
    //            }
    //        }

    //        return true;
    //    }
    //}
}