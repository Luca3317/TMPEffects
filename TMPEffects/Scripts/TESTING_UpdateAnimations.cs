using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TESTING_UpdateAnimations : MonoBehaviour
{
    [SerializeField] TMPAnimatorFinal animator;
    [SerializeField] TMP_Text text;

    public bool breaks;
    void Start()
    {
        animator.SetUpdateFrom(UpdateFrom.Update);
        StartCoroutine(ChangeAfter());
    }

    IEnumerator ChangeAfter()
    {
        yield return new WaitForSeconds(3);
        if (breaks) Debug.Break();
        text.SetText("my new text! this will be <test>red</test>, this will <wave>move</wave>, and this will do <test><wave>both! Awesome!</wave></test>! This last sentence wont do anythign");
        yield return null;
    }

}
