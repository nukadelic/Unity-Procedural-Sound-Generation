namespace ProceduralSound
{
    using UnityEngine;
    using NaughtyAttributes;
    using System.Collections.Generic;
    using System.Collections;
    using System.Linq;

    [ExecuteInEditMode]
    public class ColliderVolume : MonoBehaviour
    {
        /// Add new collider volume and imidiatly recalculate values, else return existing component
        public static ColliderVolume Attach( GameObject target )
        {
            return target == null ? null :
                   target.GetComponent<ColliderVolume>( )
                ?? target.GetComponentInParent<ColliderVolume>( )
                ?? target.AddComponent<ColliderVolume>( );
        }

        [InfoBox("This component considers only the current transform's local scale values")]

        public bool includeChildCollders = true;

        public bool recalculateOnAwake = true;

        public float Density = 0.1f;

        [Range(0,1)] public float FillRatio = 1.0f;

        public bool IsHollo => FillRatio < 0.05f;

        [ReadOnly] public int CollidersCount = 0;

        [ReadOnly] public float SurfaceArea = 0f;

        [ReadOnly] public float Volume = 0f;

        void Start() { if( recalculateOnAwake ) Recalculate( ); }

        [Button]
        public void Recalculate()
        {
            List<Collider> list = new List<Collider>();

            list = list.Union( GetComponents<Collider>() ).ToList();

            if( includeChildCollders ) list = list.Union( GetComponentsInChildren<Collider>( ) ).ToList();

            CollidersCount = list.Count;

            Volume = SurfaceArea = 0f;

            foreach( var c in list )
            {
                var sv = transform.localScale;
                var s = ( sv.x + sv.y + sv.z ) / 3f;

                Volume += CalcVolume3D( c ) * ( s * s * s ); // x Scale^3
                SurfaceArea += CalcSurfaceArea3D( c ) * ( s * s ); // x Scale^2
            }
        }

        public static float CalcVolume3D( Collider collider )
        {
            if( collider is BoxCollider )
            {
                BoxCollider c = ( BoxCollider ) collider;

                return ( c.size.x * c.size.y * c.size.z );
            }

            if( collider is SphereCollider )
            {
                SphereCollider c = ( SphereCollider ) collider;

                return ( ( 4 / 3 ) * Mathf.PI * ( c.radius * c.radius * c.radius ) );
            }

            if( collider is CapsuleCollider )
            {
                CapsuleCollider c = ( CapsuleCollider ) collider;

                var r = c.radius;
                var a = c.height <= 2f * r ? 0f : c.height - 2f * r;

                return ( Mathf.PI * ( r * r ) * 2f * ( ( 4f / 3f ) * r + a ) );
            }

            if( collider is MeshCollider )
            {
                MeshCollider c = ( MeshCollider ) collider;

                // https://stackoverflow.com/a/1568551/2496170

                var triangles = c.sharedMesh.triangles;
                var vertices = c.sharedMesh.vertices;

                float volume = 0f;

                for(int i = 0; i < triangles.Length; i += 3)
                {
                    Vector3 A = vertices[ triangles [ i ]     ];
                    Vector3 B = vertices[ triangles [ i + 1 ] ];
                    Vector3 C = vertices[ triangles [ i + 2 ] ];

                    var v321 = C.x * B.y * A.z;
                    var v231 = B.x * C.y * A.z;
                    var v312 = C.x * A.y * B.z;
                    var v132 = A.x * C.y * B.z;
                    var v213 = B.x * A.y * C.z;
                    var v123 = A.x * B.y * C.z;

                    var value = ( -v321 + v231 + v312 - v132 - v213 + v123 );

                    volume += ( 1.0f / 6.0f ) * value;
                }

                return Mathf.Abs( volume );
            }

            // TODO: this general approximation looks wrong , check if there is a more accurate method
            return ( collider.bounds.size.x * collider.bounds.size.y * collider.bounds.size.z ) / 3.0f;
        }

        public static float CalcSurfaceArea3D( Collider collider )
        {
            if(collider is BoxCollider)
            {
                BoxCollider c = ( BoxCollider ) collider;

                var XY = 2 * c.size.x * c.size.y;
                var XZ = 2 * c.size.x * c.size.z;
                var ZY = 2 * c.size.z * c.size.y;

                return ( XY + XZ + ZY );
            }

            if(collider is SphereCollider)
            {
                SphereCollider c = ( SphereCollider ) collider;

                return ( 4 * Mathf.PI * c.radius * c.radius );
            }

            if(collider is CapsuleCollider)
            {
                CapsuleCollider c = ( CapsuleCollider ) collider;

                var r = c.radius;
                var a = c.height <= 2f * r ? 0f : c.height - 2f * r;

                return ( Mathf.PI * r * 2f * ( 2f * r + a ) );
            }

            if( collider is MeshCollider )
            {
                MeshCollider c = ( MeshCollider ) collider;

                // https://gamedev.stackexchange.com/a/165647/45452
                
                var triangles = c.sharedMesh.triangles;
                var vertices = c.sharedMesh.vertices;

                double sum = 0.0;

                for(int i = 0; i < triangles.Length; i += 3)
                {
                    Vector3 corner = vertices[ triangles [ i ] ];
                    Vector3 a = vertices[ triangles[ i + 1 ] ] - corner;
                    Vector3 b = vertices[ triangles[i + 2] ] - corner;
                    sum += Vector3.Cross( a, b ).magnitude;
                }

                return ( ( float ) ( sum / 2.0 ) );
            }

            // TODO: this general approximation looks wrong , check if there is a more accurate method
            return collider.bounds.size.sqrMagnitude * 12;
        }
    }
}
