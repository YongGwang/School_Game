using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fading : MonoBehaviour
{
    public Image whiteFade;

    private void Start()
    {
        whiteFade.canvasRenderer.SetAlpha(1.0f);

        fadeiIn();
    }
    
    void fadeiIn()
    {
        whiteFade.CrossFadeAlpha(0, 2, false);
    }

}
