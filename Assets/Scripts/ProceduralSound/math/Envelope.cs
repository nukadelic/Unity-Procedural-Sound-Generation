
namespace ProceduralSound
{
    using NaughtyAttributes;
    using UnityEngine;

    /* 
    
    https://www.youtube.com/watch?v=n-k0NQ5lcSA
      
    An Envelope shapes the evolutio of the sound over time 
    
    ^ ( Amplitude )
    |
    |
    |     A     |   D   |        S         |     R
    |           |       |                  |
    |         -/|\      |                  |
    |        /  |  \    |                  |
    |       /   |   -   |                  |
    |     -/    |    \--|------------------|
    |    /      |       |                  |\
    |   /       |       |                  |  \
    | -/        |       |                  |    \
    |/          |       |                  |      \
    +================================================= > ( time ) 
    ( Key pressed )                ( Key released )
    
    It controls how sounds develops over the duration of a note 
    from the moment the key is pressed until it is released 

    Four key parameters of the envelope are reffered bt the intial 
    A,D,S,R - together they control how the sound develops over time 

    [ Attack , Decay , Sustain , Release ]

    Time based values : A,D,R : ( time duration value ) 
    Only 1 level based value : S : ( amplitude value ) 

    */

    [System.Serializable] public class Envelope
    {
        /// <summary>
        /// [A]ttack ( Time based ) how long does it take for a sound to get from nothing to full volume
        /// </summary>
        public float Attack { get => _attack; set { _attack = value; BuildCurve( ); } } float _attack;
        /// <summary>
        /// [D]ecay ( Time based ) - controls how long the sound takes to drop down to the sustain value 
        /// </summary>
        public float Decay { get => _decay; set { _decay = value; BuildCurve( ); } } float _decay;
        /// <summary>
        /// [S]ustain ( Level based ) : Limited to [0-1] - as long as the key is pressed down the sound amplitude will be equal to the sustain value 
        /// </summary>
        public float Sustain { get => _sustain; set { _sustain = Mathf.Clamp01( value ); BuildCurve( ); } } float _sustain;
        /// <summary>
        /// Release ( Time based ) - how long does it take for the sound to drop from the sustain value to nothing
        /// </summary>
        public float Release { get => _release; set { _release = value; BuildCurve( ); } } float _release;

        public Envelope( float A = 0.1f, float D = 0.05f, float S = 0.7f, float R = 0.07f )
        {
            _attack = A; _decay = D; _sustain = S; _release = R;

            BuildCurve( );
        }

        public void BuildCurve()
        {
            curve = new AnimationCurve( );
            curve.keys = new Keyframe[ ] {
                new Keyframe( 0, 0 ),
                new Keyframe( _attack, 1 ),
                new Keyframe( _decay + _attack, _sustain ),
                new Keyframe( _release + _decay + _attack, 0 ),
            };
        }

        public AnimationCurve curve;

        [SerializeField, ReadOnly] float _value = 0f;

        public float Value { private set; get; } = 0f;

        float time = 0f;

        public bool pressed = false;

        public float Update( float deltaTime )
        {
            time += deltaTime;

            Value = curve.Evaluate( time );

            if( time > _decay + _attack && pressed )
            {
                time = _decay + _attack;
                Value = _sustain;
            }



            return _value = Value;
        }
    }
}
