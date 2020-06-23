using UnityEngine;

public class ETF_Controller : MonoBehaviour
{
    public GameObject container;

    public float explodeForce = 6f;
    public float spawnRadius = 7f;

    void Start()
    {
        DebugUI.Init( );

        ResetRB( );
    }

    // Update is called once per frame
    void Update()
    {
        DebugUI.Clear( );
        DebugUI.Layout( DebugUI.UILayout.SpaceS );
        //DebugUI.Header( "Title" );
        DebugUI.Layout( DebugUI.UILayout.BeginHorizontal );
        DebugUI.Button( " Explode ", ExplodeRB );
        DebugUI.Button( " Reset ", ResetRB );
        DebugUI.Layout( DebugUI.UILayout.Flex );
        //DebugUI.Text( "Some label" );
        DebugUI.Layout( DebugUI.UILayout.EndHorizontal );
    }

    void ExplodeRB( )
    {
        foreach( var rb in container.GetComponentsInChildren<Rigidbody>() )
        {
            rb.AddForceAtPosition( 
                Random.rotation * Vector3.one * explodeForce , 
                rb.transform.position, 
                ForceMode.Impulse 
            );
        }
    }

    void ResetRB( )
    {
        var list = container.GetComponentsInChildren<Rigidbody>( );

        for(var i = 0; i < list.Length; ++i )
        {
            var rb = list[ i ];

            //var a = 2 * Mathf.PI * ( i / list.Length );
            //var x = Mathf.Cos( a ) * spawnRadius;
            //var z = Mathf.Sin( a ) * spawnRadius;
            // rb.transform.position = new Vector3( x, 3, z );

            rb.transform.position = Random.insideUnitCircle * spawnRadius;
            var p = rb.transform.position;
            p.y = 3;
            rb.transform.position = p;
            rb.RemoveForces();
        }
    }
}
