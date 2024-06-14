using System;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class TriggerZone : MonoBehaviour
{
    public event Action OnChangeStatus;
    public GrableObj CollisionObjt { get; private set; }

    private void Awake()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out GrableObj obj))
        {            
            CollisionObjt = obj;
            OnChangeStatus?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out GrableObj obj))
        {
            CollisionObjt = null;
            OnChangeStatus?.Invoke();
        }
    }
}