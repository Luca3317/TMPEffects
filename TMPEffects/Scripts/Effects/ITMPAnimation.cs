using System.Collections.Generic;

namespace TMPEffects
{
    public interface ITMPAnimation
    {
        /*
         * TODO
         * Pass in character by character (maybe using TMP_CharInfo, maybe custom class)
         * or pass in entire text component with indeces
         */
        public void Animate(ref CharData charData, AnimationContext context);

        // TODO maybe have both Apply(Character) and Apply(String, int start, int length)
        // Will make some calculations simpler + more efficient
        // Alterantively: ResetParameters => Prepare/Reset/ResetContext + more data in CharData
        //public void Apply(System.ReadOnlySpan<CharData> charData);

        // TODO Custom Parameter class? Also; differentiate between value and attributes?
        // Maybe SetValue and SetAttributes: Tag is defined in attribute, so cant be referenced within effect
        // (Reflection too expensive)
        // Alt: Just use "" for value isntead of tagname
        // Also, in imoplementations: either need a fallback for every variable, ie initialAmplitude and currentAmplitude, and reset this before calling setparameters
        // Or make this just set some backing dictionary and then get those values in apply
        // Probably prefer a ResetVariables method with standard values
        public void SetParameters(Dictionary<string, string> parameters);
        public bool ValidateParameters(Dictionary<string, string> parameters);
        public void ResetParameters();
    }
}