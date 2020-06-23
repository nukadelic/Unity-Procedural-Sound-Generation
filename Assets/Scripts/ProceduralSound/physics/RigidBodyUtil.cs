using UnityEngine;

public static class RigidBodyUtil
{
    public static void RemoveForces( this Rigidbody rb )
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.inertiaTensor = Vector3.one;
        rb.inertiaTensorRotation = Quaternion.identity;
        rb.angularDrag = 0;
        rb.angularVelocity = Vector3.zero;
    }
}
