using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Scriptables/MaterialScriptables")]
public class MaterialScriptables : ScriptableObject
{
    public List<MaterialObjects> colorBasedMaterials;
}

[System.Serializable]
public class MaterialObjects
{
    public Colors busColor;
    public List<Material> materials;
}

