using System.Collections.Generic;
using TMPEffects.AutoParameters.Attributes;
using TMPEffects.CharacterData;
using TMPEffects.Parameters;
using UnityEngine;

namespace TMPEffects.TMPAnimations.Animations
{
    [AutoParameters]
    [CreateAssetMenu(fileName = "new SketchyAnimation",
        menuName = "TMPEffects/Animations/Basic Animations/Built-in/Sketchy")]
    public partial class SketchyAnimation : TMPAnimation
    {
        [AutoParameter("delay", "d"), SerializeField]
        [Tooltip("The delay between each change, in seconds.\nAliases: delay, d")]
        private float delayTime;

        [AutoParameter("minoffset", "minoff"), SerializeField]
        [Tooltip("The min offset from the original position.\nAliases: minoffset, minoff")]
        private Vector3 minOffset;

        [AutoParameter("maxoffset", "maxoff"), SerializeField]
        [Tooltip("The max offset from the original position.\nAliases: maxoffset, maxoff")]
        private Vector3 maxOffset;

        [AutoParameter("minrotation", "minrot"), SerializeField]
        [Tooltip("The min rotation, in euler angles.\nAliases: minrotation, minrot")]
        private Vector3 minRotation;

        [AutoParameter("maxrotation", "maxrot"), SerializeField]
        [Tooltip("The max rotation, in euler angles.\nAliases: maxrotation, maxrot")]
        private Vector3 maxRotation;

        [AutoParameter("minscale", "minscl"), SerializeField] 
            [Tooltip("The min scale.\nAliases: minscale, minscl")]
        private Vector3 minScale;

        [AutoParameter("maxscale", "maxscl"), SerializeField] 
            [Tooltip("The max scale.\nAliases: maxscale, maxscl")]
        private Vector3 maxScale;
        
        [AutoParameter("mincolorshift", "minclrshift", "minclr"), SerializeField] 
        [Tooltip("The min color shift, RGB.\nAliases: mincolorshift, minclrshift, minclr")]
        private Vector3 minColorShift;

        [AutoParameter("maxcolorshift", "maxclrshift", "maxclr"), SerializeField] 
            [Tooltip("The max color shift, RGB.\nAliases: maxcolorshift, maxclrshift, maxclr")]
        private Vector3 maxColorShift;

        private partial void Animate(CharData cData, AnimData data, IAnimationContext context)
        {
            if (!data.lastUpdate.TryGetValue(cData.info.index, out float lastUpdate) || 
                context.AnimatorContext.PassedTime - lastUpdate > data.delayTime ||
                !data.modDatas.TryGetValue(cData.info.index, out var modData))
            {
                modData = new ModData();

                modData.rotation = new Vector3(
                    UnityEngine.Random.Range(data.minRotation.x, data.maxRotation.x),
                    UnityEngine.Random.Range(data.minRotation.y, data.maxRotation.y),
                    UnityEngine.Random.Range(data.minRotation.z, data.maxRotation.z));

                modData.scale = new Vector3(
                    UnityEngine.Random.Range(data.minScale.x, data.maxScale.x),
                    UnityEngine.Random.Range(data.minScale.y, data.maxScale.y),
                    UnityEngine.Random.Range(data.minScale.z, data.maxScale.z));

                modData.offset = new Vector3(
                    UnityEngine.Random.Range(data.minOffset.x, data.maxOffset.x),
                    UnityEngine.Random.Range(data.minOffset.y, data.maxOffset.y),
                    UnityEngine.Random.Range(data.minOffset.z, data.maxOffset.z));

                modData.colorshift = new Vector3(
                    UnityEngine.Random.Range(data.minColorShift.x, data.maxColorShift.x),
                    UnityEngine.Random.Range(data.minColorShift.y, data.maxColorShift.y),
                    UnityEngine.Random.Range(data.minColorShift.z, data.maxColorShift.z));

                data.modDatas[cData.info.index] = modData;
                data.lastUpdate[cData.info.index] = context.AnimatorContext.PassedTime;
            }
            
            if (modData.offset != Vector3.zero) cData.SetPosition(cData.InitialPosition + modData.offset);
            if (modData.scale != Vector3.one) cData.SetScale(modData.scale);
            if (modData.rotation != Vector3.zero) cData.AddRotation(modData.rotation, cData.InitialPosition);
            if (modData.colorshift != Vector3.zero)
            {
                context.AnimatorContext.Modifiers.CalculateVertexColors(cData, context.AnimatorContext);
                for (int i = 0; i < 4; i++)
                {
                    Color col = context.AnimatorContext.Modifiers.VertexColor(i);
                    col.r += modData.colorshift.x;
                    col.g += modData.colorshift.y;
                    col.b += modData.colorshift.z;
                    cData.mesh.SetColor(i, col);
                }
            }
        }

        [AutoParametersStorage]
        private partial class AnimData
        {
            public Dictionary<int, ModData> modDatas = new Dictionary<int, ModData>();
            public Dictionary<int, float> lastUpdate = new Dictionary<int, float>();
        }

        private struct ModData
        {
            public Vector3 rotation;
            public Vector3 scale;
            public Vector3 offset;
            public Vector3 colorshift;
        }
    }
}