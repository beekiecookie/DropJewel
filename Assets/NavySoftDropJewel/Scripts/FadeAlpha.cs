using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeAlpha : MonoBehaviour
{
    [SerializeField] private AnimationCurve fadeCurve;
    SpriteRenderer spriteRenderer;
    Color _color;
    float _timer = 0f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // do this in awake, it has an impact on performances in Update
        _color = spriteRenderer.color;
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        _color.a = fadeCurve.Evaluate(_timer);
        spriteRenderer.color = _color;
        if (_timer >= 1) _timer = 0;
    }
}
