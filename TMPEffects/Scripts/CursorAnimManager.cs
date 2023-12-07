using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorAnimManager : MonoBehaviour
{
    [SerializeField] CursorTestAnim anim;
    private void Awake()
    {
        anim?.AddObject(transform);
    }

    private void OnDestroy()
    {
        anim?.AddObject(null);
    }




}
