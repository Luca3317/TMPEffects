using System;
using System.Collections;
using System.Collections.Generic;
using TMPEffects.CharacterData;
using TMPEffects.Components;
using UnityEngine;

// TODO Change name or change tmpmeshmodifiers name
[RequireComponent(typeof(TMPAnimator))]
[ExecuteInEditMode, ExecuteAlways]
public class TMPMeshModifier : MonoBehaviour
{
    [System.NonSerialized] private CharDataModifiers _modifiers;
    [System.NonSerialized] private TMPAnimator _animator;

    // private void OnEnable()
    // {
    //     if (_animator == null)
    //         _animator = GetComponent<TMPAnimator>();
    //
    //     _modifiers = new CharDataModifiers();
    //     _animator.OnCharacterAnimated += OnCharacterAnimated;
    // }
    //
    // private void OnDisable()
    // {
    //     _animator.OnCharacterAnimated -= OnCharacterAnimated;
    // }

    private void OnCharacterAnimated(CharData cdata)
    {
        // Debug.Log("before " + cdata.CharacterModifiers.PositionDelta + " w/ " + _modifiers.CharacterModifiers.PositionDelta);
        cdata.MeshModifiers.Combine(_modifiers.MeshModifiers);
        cdata._CharacterModifiers.Combine(_modifiers.CharacterModifiers);
        // Debug.Log("after " + cdata.CharacterModifiers.PositionDelta);
    }

    public void SetModifiers(CharDataModifiers modifiers)
    {
        _modifiers = modifiers;
    }


}
