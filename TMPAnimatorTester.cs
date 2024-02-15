//using System.Collections;
//using System.Collections.Generic;
//using System.Data.Common;
//using TMPEffects.Components;
//using UnityEngine;

//public class TMPAnimatorTester : MonoBehaviour
//{
//    public TMPAnimatorComponent animator;

//    void Start()
//    {
//        animator.SetUpdateFrom(UpdateFrom.Update);
//        animator.StartAnimating();
//    }

//    void Update()
//    {
//        if (Input.GetKey(KeyCode.LeftShift))
//        {
//            if (Input.GetKeyDown(KeyCode.W))
//            {
//                if (!animator.IsAnimating)
//                    animator.StartAnimating();
//                else animator.StopAnimating();
//            }
//            if (Input.GetKeyDown(KeyCode.U))
//            {
//                animator.SetUpdateFrom(UpdateFrom.Update);
//            }
//            else if (Input.GetKeyDown(KeyCode.L))
//            {
//                animator.SetUpdateFrom(UpdateFrom.LateUpdate);
//            }
//            else if (Input.GetKeyDown(KeyCode.F))
//            {
//                animator.SetUpdateFrom(UpdateFrom.FixedUpdate);
//            }
//            else if (Input.GetKeyDown(KeyCode.S))
//            {
//                animator.SetUpdateFrom(UpdateFrom.Script);
//            }


//            if (Input.GetKey(KeyCode.Space))
//            {
//                animator.UpdateAnimations(Time.deltaTime);
//            }
//            if (Input.GetKeyDown(KeyCode.R))
//            {
//                animator.ResetAnimations();
//            }

//            if (Input.GetKeyDown(KeyCode.J))
//            {
//                animator.TryInsertTag("<wave>", 6,40);
//            }
//        }
//    }
//}
