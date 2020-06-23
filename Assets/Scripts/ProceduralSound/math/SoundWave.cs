namespace ProceduralSound
{
    using UnityEngine;
    using NaughtyAttributes;

    [System.Serializable]
    public class SoundWave
    {
        [ Tooltip("Current location on the wave")]
        [HideInInspector] public float phase = 0;

        [MinValue(0f)]
        [Tooltip("Tone frequency in Hz")]
        public float frequency = 440.0f;

        [Tooltip("Volume")]
        [Range(0,1)]
        public float gain = 0.5f;

        [Tooltip("Type of wave form / function")]
        public Wave.Type waveType;
    }
}
