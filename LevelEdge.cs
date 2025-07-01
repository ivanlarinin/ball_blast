using UnityEngine;

public enum EdgeType
{
    Bottom,
    Left,
    Right
}


public class LevelEdge : MonoBehaviour
{
    [SerializeField] private EdgeType type;
    public EdgeType Type => type;
}
