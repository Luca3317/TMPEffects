using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextPreprocessorStack : ITextPreprocessor
{
    private List<ITextPreprocessor> textPreprocessorList;

    public TextPreprocessorStack()
    {
        textPreprocessorList = new List<ITextPreprocessor>();
    }

    public string PreprocessText(string text)
    {
        for (int i = 0; i < textPreprocessorList.Count; i++)
        {
            text = textPreprocessorList[i].PreprocessText(text);
        }

        return text;
    }

    public void AddPreprocessor(ITextPreprocessor textPreprocessor)
    {
        textPreprocessorList.Add(textPreprocessor);
    }

    public void RemovePreprocessor(ITextPreprocessor textPreprocessor)
    {
        textPreprocessorList.Remove(textPreprocessor);
    }
}
