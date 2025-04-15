using System;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class TriggerZone : MonoBehaviour
{
    public event Action<GrableObj> OnAddObj;
    public event Action<GrableObj> OnDelObj;

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out GrableObj obj))
        {
            OnAddObj?.Invoke(obj);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out GrableObj obj))
        {
            OnDelObj?.Invoke(obj);
        }
    }
}