using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public static class Utility
{
    public static void Invoke(this MonoBehaviour mb, Action f, float delay)
    {
        mb.StartCoroutine(InvokeRoutine(f, delay));
    }
 
    private static IEnumerator InvokeRoutine(System.Action f, float delay)
    {
        yield return new WaitForSeconds(delay);
        f();
    }
}

namespace Characters
{

    public abstract class MovementComponent : MonoBehaviour
    {
        public UnityEvent MovementFinished;       
        public abstract Task StartMoving(Vector3 Start, Quaternion StartRotation, Vector3 Target);
        
    }

}
