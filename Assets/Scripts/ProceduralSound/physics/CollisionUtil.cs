using UnityEngine;

namespace ProceduralSound
{
    public static class CollisionUtil
    {
        /// <summary>   
        /// Total sum of the distance between the colliders at the contact point.   
        /// Physics engine keeps track of the distances between all the potentially interpenetrating colliders and once the distance is less than the sum of the contact offsets in a given collider pair, contacts are immediately generated and OnCollisionEnter event is sent. That means that while the contact pair is still active, the distance between the colliders can be greater than zero, equal to zero, or even less than zero when the colliders are still apart, touching, or overlapping respectively.
        /// </summary>
        public static float GetSeparationTotal( this Collision self )
        {
            float sum = 0f;

            for( var i = 0; i < self.contactCount; ++i )

                sum += self.GetContact( i ).separation;

            return sum;
        }

        /// <summary>   
        /// Avarage point of all contacts </summary>
        public static Vector3 GetAvarageContactPoint( this Collision self )
        {
            Vector3 sum = Vector3.zero;

            for( var i = 0; i < self.contactCount; ++i )
                sum += self.GetContact( i ).point;

            return sum / self.contactCount;
        }

        /// <summary>   
        /// Avarage normalized "normal" value of all contact points </summary>
        public static Vector3 GetAvarageContactNormal( this Collision self )
        {
            Vector3 sum = Vector3.zero;

            for( var i = 0; i < self.contactCount; ++i )
                sum += self.GetContact( i ).normal;

            return ( sum / self.contactCount ).normalized;
        }
    }
}
