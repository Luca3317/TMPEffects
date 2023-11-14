using System.Collections.Generic;
using TMPro;

/*
 * The interface that is to be implemented by all TextEffects.
 * 
 * TODO
 * Define the actual process a bit better
 * 
 * Preprocessor parses string, registers tags using their name, startIndex and endIndex (as length)
 * 
 * Important Point: Do all tags with the same name (ie if you want 3 segments w/ a shake effect in your text) use
 * the same instance of the TextEffect?
 * Ideally yes; much more efficient than creating them on the fly.
 * But: Will have to set the parameters every frame (fine); Cant have them hold state (less fine)
 *      => All the state that really should be relevant might be contained in TMP_Charinfo/Textinfo?
 *      If not might also do custom state/data classes for each segment or character
 *      
 *      ! TODO
 *      Custom data containers per character might be a good idea; on initialization (when is that? Whenever text is updated?)
 *      fill container with info about the character, such as initial color, vertex position, passedDisplayTime = 0 etc.
 *      Will allow you to ie manipulate mesh based on the initial mesh instead of the current one, or change color after character
 *      has been shown for x amount of time.
 * 
 * Another important point: should they even be scriptableObjects? Originally made them that to allow for 
 * different base variable values, so you dont always have to set every attribute. But; can be handled with stylesheet?
 * For now keep them as SOs
 * 
 * Some context object? Ie contain where you are being called from (Update, FixedUpdate, Script) to know what time scaling to apply?
 * 
 * Apply(Character) vs Apply(Span<Character>)
 *      Assuming Character includes segmentposition; no real difference
 *      Applying per character probably simpler conceptually
 *      Any redundant data if saving per character?
 * 
 */

public interface ITMPEffect
{
    /*
     * TODO
     * Pass in character by character (maybe using TMP_CharInfo, maybe custom class)
     * or pass in entire text component with indeces
     */
    public void Apply(ref CharData charData);

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
    public void ResetVariables();


    public void SetParameter<T>(string name, T value);
}
