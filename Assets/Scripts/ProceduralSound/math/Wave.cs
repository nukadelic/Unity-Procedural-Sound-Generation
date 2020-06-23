namespace ProceduralSound
{ 
    using UnityEngine;
 
    public static class Wave
    {
        public enum Type { Sin, Triangle, Square }

        public static float Calculate( SoundWave x )
        {
            switch( x.waveType )
            {
                case Type.Sin: return Sin( x );
                case Type.Square: return Square( x );
                case Type.Triangle: return Triangle( x );
            }

            return 0;
        }

        public static float Sin( SoundWave x ) => x.gain * Mathf.Sin( x.phase );

        public static float Triangle( SoundWave x ) => x.gain * Mathf.PingPong( x.phase, 1f );

        public static float Square( SoundWave x )
        {
            if ( Sin( x ) >= 0 ) return x.gain * 0.6f;

            return - x.gain * 0.6f;
        }
    }
}


