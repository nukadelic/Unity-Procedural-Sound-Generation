using UnityEngine;
using UnityEngine.Events;

public class RandomTransformOnStart : MonoBehaviour
{
    public Vector3 minScale = Vector3.one * 0.5f;
    public Vector3 maxScale = Vector3.one * 2.5f;

    public bool uniformXZ = false;

    public bool setRandomRotation = true;

    public UnityEvent OnComplete;

    void Start()
    {
        var sx = ( maxScale.x - minScale.x ) * Random.value + minScale.x;
        var sy = ( maxScale.y - minScale.y ) * Random.value + minScale.y;
        var sz = ( maxScale.z - minScale.z ) * Random.value + minScale.z;
        
        if( uniformXZ ) sz = sx;

        transform.localScale = new Vector3( sx,sy,sz );

        if( setRandomRotation ) transform.rotation = Random.rotation;

        OnComplete?.Invoke();
    }
}
