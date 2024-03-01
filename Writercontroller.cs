using System.Collections;
using System.Collections.Generic;
using TMPEffects.Components;
using UnityEngine;

public class Writercontroller : MonoBehaviour
{
    [SerializeField] TMPWriter writer;

    int textIndex;
    public readonly string[] strings = new string[]
    {
        "This is the very first text after the switch!\nIll keep this one short.",
        "Second text\nIll have to write a little more here. \nActually no i dont at all.",
        "",
        "",
        "",
        "",
        "",
        ""
    };

    private void Start()
    {
        writer.OnFinishWriter.AddListener(StartCoroutine);
        textIndex = 0;
    }

    void StartCoroutine()
    {
        StartCoroutine(CC());
    }

    IEnumerator CC()
    {
        yield return new WaitForSeconds(3);

        writer.Hide(0, writer.TextComponent.textInfo.characterCount, false);

        yield return new WaitForSeconds(3);

        writer.SetText(strings[textIndex++]);
    }
}
