using Snek.Utilities;
using UnityEngine;

[UseSnekInspector]
[RequireComponent(typeof(RectTransform))]
public class LoadingCircle : SnekMonoBehaviour
{
    [Tooltip("Degrees per second")]
    [Min(0f)]
    [SerializeField] private float _spinSpeed = 2f;

    private void Update()
    {
        transform.Rotate(Vector3.back, _spinSpeed * Time.deltaTime);
    }
}
