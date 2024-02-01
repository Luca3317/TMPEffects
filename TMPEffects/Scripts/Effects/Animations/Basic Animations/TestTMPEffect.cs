using UnityEngine;

namespace TMPEffects.Animations
{
    [CreateAssetMenu(fileName = "new TestTMProEffect", menuName = "TMPEffects/Animations/Test")]
    public class TestTMProEffect : TMPAnimationParameterless

    {
        [SerializeField] Color32 color;

        [SerializeField] private float _speed = 10;
        [SerializeField] private float _frequency = 10;
        [SerializeField] private Color32 lerpColor;

        public override void Animate(ref CharData cData, IAnimationContext context)
        {
            for (int i = 0; i < 4; i++)
            {
                Color32 c = Color.red;
                cData.mesh.SetColor(i, c);
            }

            cData.mesh.SetColor(0, Color.blue);
            cData.mesh.SetColor(2, Color.blue);
        }

        public override void ResetParameters()
        { }
    }
}