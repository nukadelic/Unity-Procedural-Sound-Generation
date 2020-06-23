namespace ProceduralSound
{
    using NaughtyAttributes;
    using UnityEngine;

    [RequireComponent(typeof(AudioSource))]
    public class ProcSound : MonoBehaviour
    {
        // 2PI constant 
        static readonly float TwoPi = Mathf.PI * 2f;

        [ReorderableList] public SoundWave[] waves;

        /// Unity audio engine runs on 48k Hz by default 
        float sampling_frequency = 48000f;

        void OnAudioFilterRead( float[ ] data, int channels )
        {
            // Reset values 
            for( var ii = 0; ii < data.Length; ++ii ) data[ ii ] = 0;

            foreach( var wave in waves )
            {
                // Amount of distance the wave will be moving each frame 
                float increment = wave.frequency * TwoPi / sampling_frequency;

                for( var i = 0; i < data.Length; i += channels )
                {
                    wave.phase += increment;

                    data[ i ] += Wave.Calculate( wave );

                    if(channels > 1) data[ i + 1 ] = data[ i ];

                    if(wave.phase > TwoPi) wave.phase = 0f;
                }
            }

            //DebugVars( data, channels );
        }


        //int debug_counter = 0;
        //float debug_timer = 0f;
        //public float debug_fps = 0f; // 46.87412
        //public int debug_dataLength = 0; // 2048
        //public int debug_channels = 0; // 2

        //void DebugVars( float[ ] data, int channels )
        //{
        //    //AudioSettings.GetDSPBufferSize( out int bufferLen, out int numBuffers );
        //    //var rate = AudioSettings.outputSampleRate;
        //    debug_dataLength = data.Length;
        //    debug_channels = channels;
        //    if(debug_timer == 0f) debug_timer = ( float ) AudioSettings.dspTime;
        //    else
        //    {
        //        debug_counter++;
        //        var delta = ( (float) AudioSettings.dspTime ) - debug_timer;
        //        if(delta > 1f)
        //        {
        //            debug_fps = debug_counter / delta;
        //            debug_timer = ( float ) AudioSettings.dspTime;
        //            debug_counter = 0;
        //        }
        //    }
        //}
    }
}