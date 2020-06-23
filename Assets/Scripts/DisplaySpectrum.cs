namespace ProceduralSound
{
    using UnityEngine;
    using NaughtyAttributes;
    using ProceduralSound;

    [RequireComponent( typeof( AudioSource ) )]
    public class DisplaySpectrum : MonoBehaviour
    {
        public bool IsPlaying => Application.isPlaying;
        
        FrequencyBands8 bands = null;
        AudioSource audioSource = null;
        GameObject quad = null;
        Material mat = null;
        Texture2D tex = null;

        int w = 1024;
        int h = FrequencyBands8.SampleCount;

        bool inited = false;

        public FFTWindow plotWindow = FFTWindow.BlackmanHarris;

        //[Header("Visual Preview")]
        //[Tooltip("Move the display spectrum to a new 'local' position value")]
        //public bool translate = false;
        //[ShowIf("translate")]
        //public Vector3 translateValue = Vector3.zero;
        //public float scalePreview = 1f;

        private void Awake( )
        {
            if(inited) return; inited = true;

            tex = new Texture2D( w, h, TextureFormat.RGB24, false, true );
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.SetPixels32( new Color32[ w * h ] );
            tex.Apply( );

            mat = new Material( Shader.Find( "Unlit/Texture" ) );
            mat.SetTexture( "_MainTex", tex );

            quad = GameObject.CreatePrimitive( PrimitiveType.Quad );
            quad.GetComponent<Renderer>( ).material = mat;
            quad.transform.localPosition = Vector3.zero;
            quad.transform.parent = transform;
            quad.gameObject.hideFlags = HideFlags.HideInHierarchy;
            quad.transform.localScale = new Vector3( w / h, 1 );
        }


        void Start( ) 
        {
            audioSource = GetComponent<AudioSource>( );

            bands = new FrequencyBands8();
        }

        [ ReadOnly ] public float minSample;
        [ ReadOnly ] public float maxSample;

        int i = 0;
        float[] samples;

        void Update( )
        {
            samples = new float[ FrequencyBands8.SampleCount ];

            audioSource.GetSpectrumData( samples, 0, plotWindow );

            bands.Sample512( samples );

            minSample = float.MaxValue;
            maxSample = float.MinValue;

            var x = i++ % ( w - 256 );

            var xNext = ( x + 1 ) % ( w - 256 );

            for(var y = 0; y < h; ++y)
            {
                float s = samples[ y ];

                float vR = 1 - Mathf.Pow( ( 1 - s ), ( 100 ) );
                float vG = s * 2;
                float vB = ( s - 0.5f ) * 2f;

                var r = s <= 0.03f ? vR / 0.03f : 0f;
                var g = s > 0.5f ? 0f : vG / 0.4f;
                var b = s > 0.5f ? vB : 0f;

                tex.SetPixel( x, y, new Color( r, g, b ) );

                if(s < minSample) minSample = s;
                if(s > maxSample) maxSample = s;

                tex.SetPixel( xNext, y, Color.grey ); // Draw full height cursor 
            }

            Color c = Color.white;

            // Draw Freq bands on the right hand side of the texture 

            int band = 0;

            tex.SetPixels( w - 256, 0, 256, h, new Color[ 256 * h ] );

            for(x = w - 256; x < w; x += 32 ) // 256 / 8 = 32
            {
                var b = bands.GetBand( band ++ , true, true );

                c = c == Color.white ? Color.grey : Color.white;

                for(var xi = 0; xi < 32; xi++)
                {
                    var hi = Mathf.Clamp( h * b, 0, h );

                    for(var yi = 0; yi < hi ; yi ++ ) 
                    {
                        tex.SetPixel( x + xi, yi, c );
                    }
                }
            }

            tex.Apply( );

            if(Camera.main != null)
            {
                var vec = quad.transform.position - Camera.main.transform.position;
                quad.transform.rotation = Quaternion.LookRotation( vec, quad.transform.up );
            }

            //quad.transform.localPosition = translate ? translateValue : Vector3.zero;
            //quad.transform.localScale = new Vector3( w / h, 1 ) * scalePreview;
        }
    }
}