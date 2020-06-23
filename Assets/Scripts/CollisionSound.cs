namespace ProceduralSound
{
    using UnityEngine;

    public class CollisionSound : MonoBehaviour
    {
        private void OnCollisionEnter( Collision collision )
        {
            Vector3 collision_impulse = collision.impulse;
            Vector3 collision_relativeVelocity = collision.relativeVelocity;

            Rigidbody bodyA = GetComponent<Rigidbody>();    // self 
            Rigidbody bodyB = collision.rigidbody;          // other 

            float bodyA_mass = bodyA?.mass ?? 0f;
            float bodyB_mass = bodyB?.mass ?? 0f;

            float bodyA_angularDrag = bodyA?.angularDrag ?? 0f;
            float bodyB_angularDrag = bodyB?.angularDrag ?? 0f;

            Vector3 bodyA_angularVelocity = bodyA?.angularVelocity ?? Vector3.zero;
            Vector3 bodyB_angularVelocity = bodyB?.angularVelocity ?? Vector3.zero;

            float bodyA_drag = bodyA?.drag ?? 0f;
            float bodyB_drag = bodyB?.drag ?? 0f;

            Vector3 bodyA_inertiaTensor = bodyA?.inertiaTensor ?? Vector3.zero;
            Vector3 bodyB_inertiaTensor = bodyB?.inertiaTensor ?? Vector3.zero;

            var CVA = ColliderVolume.Attach( gameObject );
            var CVB = ColliderVolume.Attach( collision.gameObject );

            float bodyA_volume = CVA?.Volume ?? 0f;
            float bodyB_volume = CVB?.Volume ?? 0f;

            float bodyA_surfaceArea = CVA?.SurfaceArea ?? 0f;
            float bodyB_surfaceArea = CVB?.SurfaceArea ?? 0f;

            //Renderer renderA = GetComponent<Renderer>( ) ?? GetComponentInChildren<Renderer>();
            //Renderer renderB = collision.gameObject.GetComponent<Renderer>( ) ?? collision.gameObject.GetComponentInChildren<Renderer>();
        }

    }
}