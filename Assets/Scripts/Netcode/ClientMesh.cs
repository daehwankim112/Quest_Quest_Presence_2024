using Unity.Netcode;
using UnityEngine;

public class ClientMesh : NetworkBehaviour
{
    [SerializeField] MeshFilter meshFilter;

    public void SetMesh(Mesh mesh)
    {
        meshFilter.mesh = mesh;
    }
}