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
        "This is just some plain text\r\n\r\n<!show>im</!show> <wave>waving</wave>\r\n<!show>im</!show> <jump>jumping</jump>\r\n<!show>im</!show> <fade>fading</fade>\r\n<!show>im</!show> <shake>shaking</shake>\r\n<!show>im</!show> <pivot>pivoting</pivot>\r\n<!show>im</!show> <funky>funky</funky>\r\n<!show>im</!show> <grow>growing</grow>\r\n<!show>im</!show> <explode>exploding</explode>\r\n<!show>im</!show> <char>changing characters</char>"
    };

    private void Start()
    {
        writer.OnFinishWriter.AddListener(StartCoroutine);
        textIndex = 0;
    }

    void StartCoroutine(TMPWriter writer)
    {
        StartCoroutine(CC());
    }

    IEnumerator CC()
    {
        yield return new WaitForSeconds(3);

        writer.Hide(0, writer.TextComponent.textInfo.characterCount, false);

        yield return new WaitForSeconds(3);

        writer.SetText(strings[textIndex++]);
        textIndex %= strings.Length;
    }
}
