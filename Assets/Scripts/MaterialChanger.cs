using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    [SerializeField] private List<Material> materials = new List<Material>();

    private Renderer _renderer;
    #region MONO
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    #endregion

    public void ChangeMaterial(int index)
    {
        if (index>=0 && index < materials.Count)
        {
            _renderer.material = materials[index];
        }
    }
}
