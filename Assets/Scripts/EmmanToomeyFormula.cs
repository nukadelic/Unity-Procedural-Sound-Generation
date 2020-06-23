namespace ProceduralSound
{
    using NaughtyAttributes;
    using UnityEngine;
    using System.Linq;
    using System.Collections.Generic;

    /**
     * https://emmantoomey.files.wordpress.com/2016/01/report.pdf
     * Procedurally Generated Audio from Real-Time Collision Detection [ Design Document ]
     * Emman Toomey∗ - 40090268@napier.ac.uk - Edinburgh Napier University - Physics-Based Animation (SET09119)
     * 
     * 5.4  System Calculations (2) :
     * y(t)=y_0e^{(-rt/2m)}cos(t\sqrt{k/m-(r/2m)^2})
     */

    [DisallowMultipleComponent]
    [RequireComponent( typeof( AudioSource ) )]
    public class EmmanToomeyFormula : MonoBehaviour
    {
        // 2PI constant 
        static readonly float TwoPi = Mathf.PI * 2f;

        // e = 2.71828182845904523536028747135266249775724709369995
        public static readonly float E = 2.718281828459045f;

        [Range(0,1)]
        public float Volume = 0.4f;
        [Tooltip("'y0' in Meters")]
        public float y0 = 1f;
        [Tooltip("'m' in Grams")]
        public float Mass = 1f;
        [Tooltip("'r' in N/m/s")] // aka Damping Ratio
        public float Damping = 0.002f;
        [Tooltip("'k' in N/m")]
        public float Stiffness = 1000f;
        [Tooltip("t += freq in Hz")]
        public float Frequency = 440;
        [ReadOnly, Tooltip("'t' in Seconds (current wave phase)")]
        public float Phase = 0;

        float VolumeMultiplier = 1f;

        public float Evaluate( float time )
        {
            // $y(t)=y_0e^{(-rt/2m)}cos(t\sqrt{k/m-(r/2m)^2})$
            var m2 = Mass * 2f;
            var ePow = Mathf.Pow( E, ( -Damping * time / m2 ) );
            var sqrt = Mathf.Sqrt( Stiffness/Mass - Mathf.Pow( Damping / m2, 2 ) );
            var cosT = Mathf.Cos( time * sqrt );
            return y0 * ePow * cosT;
        }

        bool isPlaying = false;
        public bool IsPlaying => isPlaying;

        float startTime = 0f;

        [Button] public void Play( bool force_restart = false )
        {
            if( ! Application.isPlaying || ( isPlaying && ! force_restart ) ) return;

            isPlaying = true;
            Phase = 0;
            startTime = 0f;
        }

        /// Unity audio engine runs on 48k Hz by default 
        float sampling_frequency = 48000f;

        void OnAudioFilterRead( float[ ] data, int channels )
        {
            if( ! isPlaying ) return;

            if( startTime == 0 ) startTime = ( float ) AudioSettings.dspTime;

            // Amount of distance the wave will be moving each frame 
            float increment = Frequency * TwoPi / sampling_frequency;

            float value = 0f;

            for(var i = 0; i < data.Length; i += channels)
            {
                Phase += increment;

                value = Evaluate( Phase ) * Volume * VolumeMultiplier;

                if( Mathf.Abs( value ) > MaxVal ) MaxVal = Mathf.Abs( value );

                data[ i ] = value;

                if(channels > 1) data[ i + 1 ] = data[ i ];
            }

            if( Mathf.Abs( value ) < 1e-4f ) isPlaying = false;
        }

        [ReadOnly]
        public float MaxVal = 0;

        [Tooltip("Requires collider to be attached")]
        public bool playOnCollision = true;

        public bool adjustParamsOnCollision = true;

        ColliderVolume selfVol;
        Rigidbody body;

        Collision lastCollision;

        void OnDrawGizmos( )
        {
            if( ( lastCollision?.contactCount ?? 0 ) < 1 ) return;

            foreach( var c in lastCollision.contacts )
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere( c.point, 0.1f );
            }
        }

        public void OnCollisionEnter( Collision collision )
        {
            lastCollision = collision;

            selfVol = selfVol ?? ColliderVolume.Attach( gameObject );
            body = body ?? GetComponent<Rigidbody>() ?? GetComponentInParent<Rigidbody>();

            //y0 = 0;
            //Stiffness = 0;
            //Damping = 0;
            //Frequency = 0;

            // -- Rigid Body -- 

            var bD = body.drag;
            var bAD = body.angularDrag;
            var bM = body.mass;

            // -- Physics Material -- 

            // How bouncy is the surface? A value of 0 will not bounce. A value of 1 will bounce without any loss of energy.
            float mB = collision.collider.material.bounciness;
            // The friction used when already moving. This value is usually between 0 and 1.
            float mF = collision.collider.material.dynamicFriction;
            // The friction coefficient used when an object is lying on a surface.
            float mSF = collision.collider.material.staticFriction;

            // -- Volume / Area -- 

            // Combined Volume of all the colliders attached
            float oV = selfVol.Volume;
            // Combined Surface area of all the colliders attached 
            float oSA = selfVol.SurfaceArea;

            // -- Collision variables --

            // The total impulse applied to this contact pair to resolve the collision.
            Vector3 cI = collision.impulse;
            // The relative linear velocity of the two colliding objects
            Vector3 cV = collision.relativeVelocity;
            // Total sum of the distance between the colliders at the contact point.
            float cST = collision.GetSeparationTotal(); 
            // Avarage point of all contacts
            Vector3 cP = collision.GetAvarageContactPoint();
            // Avarage normalized "normal" value of all contact points
            Vector3 cN = collision.GetAvarageContactNormal();

            // -- Normalize --

            var velocity_normalized = cV.magnitude / 350f; // 50 on avarage
            var volume_normalized = oV / 27f; // For a a unit calpsule the avarage value is 10 
            var surfaceArea_normalized = oSA / 27f; // For a unit capsule the avarage value is 10
            var impulseMagnitude_normalize = cI.magnitude / 150f; // strong impule magnitude is around 30 

            //VolumeMultiplier = impulseMagnitude_normalize;

            //var contactPoint_normalized = cST;

            Damping = 0.012f - ( 0.01f * impulseMagnitude_normalize );

            values[ globalIndex ] = Mathf.Abs( cI.magnitude ); 

            //Mass = bM / 60f;

            // Viscous damping force is a formulation of the damping phenomena, in which the source of damping force is modeled as a function of the volume, shape, and velocity of an object traversing through a real fluid with viscosity.[1] 

            //Damping = 

            Play( true );
        }

        private void Awake( )
        {
            globalIndex = count ++ ;
            values.Add( 0f );
        }

        [InfoBox("yeet")]
        [ReadOnly] public float avarage = 0f;
        [ReadOnly] public float[] allValues;

        private void Update( ) 
        {
            allValues = values.ToArray( );
            avarage = values.Sum() / values.Count;
        }

        int globalIndex = 0;
        static int count = 0;
        public static List<float> values = new List<float>();
    }
}