using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DebugUI : MonoBehaviour
{
    static DebugUI instance;

    private void Awake( ) 
    {
        if(instance != null) throw new Exception( "Debug UI already exists" );
        else
        {
            instance = this;
            DontDestroyOnLoad( instance );
        }
    }

    public static void Init()
    {
        if(instance != null) return;
        var go = new GameObject( );
        go.AddComponent<DebugUI>( );
        go.name = "DebugUI";
    }

    // ------------------- 
    // ------------------- 
    // ------------------- 

    public static void Clear( )
    {
        items = new List<UIData>( );
    }

    public enum UILayout
    {
        Text, Button, Header,
        BeginHorizontal, EndHorizontal,
        BeginVertical, EndVertical,
        SpaceS, SpaceM, SpaceL, Flex
    }

    public class UIData
    {
        public UILayout layout;
        public string text;
        public bool disabled = false;
        public Action callback;
    }

    public static List<UIData> items = new List<UIData>();

    public static void Text( string text )
    {
        UIData data = Layout( UILayout.Text );
        data.text = text;
    }

    public static UIData Button( string label, Action callback )
    {
        UIData data = Layout( UILayout.Button );
        data.callback = callback;
        data.text = label;
        return data;
    }

    public static UIData Layout( UILayout layout )
    {
        var data = new UIData() { layout = layout };
        items.Add( data );
        return data;
    }

    public static void Header( string text )
    {
        UIData data = Layout( UILayout.Header );
        data.text = text;
    }


    // -----------------

    Rect screen;
    float scale = 1f;

    void OnGUI( )
    {
        scale = Screen.dpi / 96;
        screen = new Rect( 0, 0, Screen.width, Screen.height );

        InitStyles( );

        if( Input.touchCount > 0 ) HandleTouchScroll( );

        var oScroll = new GUILayoutOption[] {
            GUILayout.ExpandWidth( true ),
            GUILayout.ExpandHeight( true ),
            GUILayout.MaxWidth( screen.width ),
            GUILayout.MaxHeight( screen.height )
        };

        using(var scope = new GUILayout.ScrollViewScope( scroll, sScroll, oScroll ))
        {
            scroll = scope.scrollPosition;

            foreach( var item in items )
            {
                switch( item.layout )
                {
                    case UILayout.SpaceS: GUILayout.Space( 5 * scale ); break;
                    case UILayout.SpaceM: GUILayout.Space( 10 * scale ); break;
                    case UILayout.SpaceL: GUILayout.Space( 20 * scale ); break;
                    case UILayout.Flex: GUILayout.FlexibleSpace( ); break;
                    case UILayout.BeginHorizontal: GUILayout.BeginHorizontal( ); break;
                    case UILayout.EndHorizontal: GUILayout.EndHorizontal( ); break;
                    case UILayout.BeginVertical: GUILayout.BeginVertical( ); break;
                    case UILayout.EndVertical: GUILayout.EndVertical( ); break;
                    case UILayout.Text: GUILayout.Label( item.text, sLabel ); break;
                    case UILayout.Button:
                        var label = item.text;
                        if( item.disabled ) label = " --[x]-- " + label + " --[x]-- ";
                        if( GUILayout.Button( label, sBtn ) && ! item.disabled ) 
                            item.callback.Invoke( ); 
                    break;
                    case UILayout.Header:
                        using(new GUILayout.VerticalScope( ))
                        {
                            GUILayout.Space( 10 * scale );
                            GUILayout.Label( item.text, sTextHuge );
                            GUILayout.Space( 10 * scale );
                        }
                    break;
                }
            }
        }
    }

    // -- STYLE -- 

    bool sInit = false;
    GUIStyle sBtn = null;
    GUIStyle sLabel = null;
    GUIStyle sScroll = null;
    GUIStyle sTextHuge = null;

    void InitStyles()
    {
        if( sInit ) return; sInit = true;

        sBtn = new GUIStyle( GUI.skin.button );
        sBtn.fontSize = Mathf.FloorToInt( 12 * scale );
        sBtn.wordWrap = true;
        sBtn.normal.background = Texture2D.grayTexture;
        sBtn.border = new RectOffset( 1, 1, 1, 1 );
        var tex2D = new Texture2D(3, 3);
        for(var x = 0; x < 3; ++x) for(var y = 0; y < 3; ++y) tex2D.SetPixel( x, y, Color.white );
        tex2D.SetPixel( sBtn.border.left, sBtn.border.top, Color.grey ); tex2D.filterMode = FilterMode.Point; tex2D.Apply( );
        sBtn.hover.background = sBtn.active.background = sBtn.focused.background = tex2D;

        sLabel = new GUIStyle( GUI.skin.label );
        sLabel.fontSize = sBtn.fontSize;

        sScroll = new GUIStyle( GUI.skin.scrollView );
        int pad = Mathf.FloorToInt(10 * scale);
        sScroll.padding = new RectOffset( pad, Mathf.FloorToInt( 2 * scale ), pad, pad );
        
        var scrollBarTex = new Texture2D(1, 1);
        scrollBarTex.SetPixel( 0, 0, new Color( 0.1f, 0.1f, 0.1f ) );
        scrollBarTex.Apply( );

        GUI.skin.verticalScrollbarThumb.fixedWidth = 14f * scale;
        GUI.skin.verticalScrollbar.fixedWidth = 14f * scale;
        GUI.skin.verticalScrollbarThumb.normal.background = scrollBarTex;

        sTextHuge = new GUIStyle( GUI.skin.label );
        sTextHuge.fontStyle = FontStyle.Bold;
        sTextHuge.fontSize = Mathf.FloorToInt( 24 * scale );
        sTextHuge.alignment = TextAnchor.MiddleCenter;
    }

    // -- TOUCH CONTROLS -- 

    bool scrolling = false;
    Vector2 scroll = Vector2.zero;
    Vector2 point = Vector2.zero;

    IEnumerator StopTouch( )
    {
        yield return new WaitForSeconds( 0.15f );
        scrolling = false;
    }

    void HandleTouchScroll( )
    {
        var touch = Input.GetTouch( 0 );

        switch(touch.phase)
        {
            case TouchPhase.Began:
                point = touch.position;
                scrolling = true;
                break;
            case TouchPhase.Moved:
                scroll.y += touch.deltaPosition.y / 4f;
                scrolling = true;
                break;
            case TouchPhase.Canceled:
            case TouchPhase.Ended:
                bool click = (touch.position - point).magnitude < 5f * scale;
                if(click) scrolling = false;
                else StartCoroutine( StopTouch( ) );
                break;
        }
    }
}
