using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardFaceOpt : MonoBehaviour
{
    
    public Sprite faceTexture;

    private static readonly int MainTex = Shader.PropertyToID("_MainTex");
    private Renderer _renderer;
    private MaterialPropertyBlock _propBlock;

    void Awake()
    {
        _propBlock = new MaterialPropertyBlock();
        _renderer = GetComponent<Renderer>();
        
        _renderer.GetPropertyBlock(_propBlock);
        
        _propBlock.SetTexture(MainTex, faceTexture.texture);
        
        
        // Apply the edited values to the renderer.
        _renderer.SetPropertyBlock(_propBlock);

    }
}
