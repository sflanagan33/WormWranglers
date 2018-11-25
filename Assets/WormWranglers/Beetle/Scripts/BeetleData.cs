using UnityEngine;

[CreateAssetMenu(fileName = "Beetle Data", menuName = "Beetle Data", order = 1)]
public class BeetleData : ScriptableObject
{
    public Mesh[] models;
    public BeetlePalette[] palettes;
}

[System.Serializable]
public struct BeetlePalette
{
    public Color main;
    public Color shadow;
    public Color highlight;
}