using UnityEngine;

[CreateAssetMenu]
public class MapData : ScriptableObject
{
    [SerializeField] private Vector2 _LimitMin;
    [SerializeField] private Vector2 _LimitMax;

    public Vector2 LimitMin
    {
        get
        {
            return _LimitMin;
        }
    }
    public Vector2 LimitMax
    {
        get
        {
            return _LimitMax;
        }
    }
}
