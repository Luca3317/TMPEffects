using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using UnityEngine;

// TODO Change name or change tmpmeshmodifiers name
[RequireComponent(typeof(TMPAnimator))]
public class TMPMeshModifier : MonoBehaviour
{
    // private CharDataModifiers _modifiers;
    private TMPAnimator _animator;

    private void OnEnable()
    {
        if (_animator == null)
            _animator = GetComponent<TMPAnimator>();

        // _modifiers = new CharDataModifiers();
        _animator.OnCharacterAnimated += OnCharacterAnimated;
    }

    private void OnDisable()
    {
        _animator.OnCharacterAnimated -= OnCharacterAnimated;
    }

    private void OnCharacterAnimated(CharData cdata)
    {
        // SmthThatAppliesModifiers applier = new SmthThatAppliesModifiers();
        // applier.ApplyToCurrentCharData(cdata, _modifiers);
    }

    // public void SetModifiers(CharDataModifiers modifiers)
    // {
    //     _modifiers = modifiers;
    // }
}
