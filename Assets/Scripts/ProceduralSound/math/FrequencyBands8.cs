/**

Audio Spectrum 
---------------------------
Sub base :           20 - 60     Hz 
Bass :               60 - 250    Hz 
Low Midrange :       250 - 500   Hz 
Mid Midrange :       500 - 2k    Hz
Upper Midrange :     2 - 4       kHz
Presence :           4 - 6       kHz 
Brilliance :         6 - 20      kHz 
---------------------------

*/
namespace ProceduralSound
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using NaughtyAttributes;

    [System.Serializable]
    public class FrequencyBands8
    {
        public static readonly int SampleCount = 512;
        public static readonly int BandsCount = 8;

        float[] raw             = new float[ BandsCount ];
        float[] smooth          = new float[ BandsCount ];
        float[] acceleration    = new float[ BandsCount ];
        float[] limitsMax       = new float[ BandsCount ];
        float[] limitsMin       = new float[ BandsCount ];

        public float GetBand( int index, bool smooth = false, bool normalized = false )
        {
            if( index < 0 || index >= BandsCount ) throw new System.Exception("Only " + BandsCount + " freq bands exist");

            // Get value 

            if( smooth )
            {
                if( normalized ) return Normalize( index, this.smooth[ index ] );

                else return this.smooth[ index ];
            }

            if( normalized ) return Normalize( index, raw[ index ] );

            return raw[ index ];
        }

        /// <summary 0 to 1 </summary>
        public float Normalize( int index, float value )
        {
            if( index < 0 || index >= BandsCount ) throw new System.Exception("Only " + BandsCount + " freq bands exist");

            // Calculate noamlized value 

            return value / ( limitsMax[ index ] - limitsMin[ index ] ) + limitsMin[ index ];
        }

        public void Sample512( float[] samples )
        {
            if( ( samples?.Length ?? 0 ) != SampleCount ) throw new System.Exception( "Sample512 accepts only array lenght of " + SampleCount );

            int sampleIndex = 0;

            /*
            - for 512 Bands and range up to 22kHz we use : 
            - * 22050 / 512 = 43 Hz per sample
            - * sum = samples count * 43 Hz 
            - 
            - array index | samples | sum      | range 
            * ---------------------------------------------------
            *    0        | 2       | 84 Hz    | 0    - 84 Hz 
            *    1        | 4       | 172 Hz   | 87   - 258 Hz
            *    2        | 8       | 344 Hz   | 259  - 602 Hz 
            *    3        | 16      | 688 Hz   | 603 - 1290 Hz
            *    4        | 32      | 1376 Hz  | 1291 - 2666 Hz
            *    5        | 64      | 2752 Hz  | 2667 - 5418 Hz
            *    6        | 128     | 5504 Hz  | 5419 - 10922 Hz
            *    7        | 256     | 11008 Hz | 10923 - 21930 Hz
            *    
            *    total samples from 0 to 7 = 510 
            */

            for(var i = 0; i < 8; ++i)
            {
                // sample count as power of 2's starting from 2 
                int samplesCount = 2 << i;

                // increase sample count from 510 to 512 
                if(i == 7) samplesCount += 2;

                float avarage = 0;

                for(var j = 0; j < samplesCount; ++j)
                {
                    // Why ? 
                    avarage += samples[ sampleIndex ] * ( sampleIndex + 1 );

                    // Advance to next sample index , will reach 512 
                    sampleIndex++;
                }

                // set raw frequecy band value 
                raw[ i ] = avarage;

                // smooth bands
                if( smooth[ i ] < avarage )
                {
                    smooth[ i ] = avarage;
                    acceleration[ i ] = 0.005f;  
                }
                else
                {
                    smooth[ i ] -= acceleration[ i ];
                    acceleration[ i ] *= 1.2f; // 20% increase 
                }

                // compute limits
                if( avarage < limitsMin[ i ] ) limitsMin[ i ] = avarage;
                if( avarage > limitsMax[ i ] ) limitsMax[ i ] = avarage;
            }
        }
    }

}