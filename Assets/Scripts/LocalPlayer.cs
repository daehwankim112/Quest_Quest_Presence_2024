using NuiN.NExtensions;
using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    [SerializeField] GameObject giantPlayer;
    [SerializeField] GameObject smallPlayer;

    [SerializeField] float spawnHeight = 10f;
    
    void Start()
    {
        SetPlayerAsGiant();
    }

    void OnEnable()
    {
        GameEvents.OnRecievedSceneMeshFromHost += SetPlayerAsSmall;
        GameEvents.OnLobbyHosted += SetPlayerAsGiant;
        GameEvents.OnRoomScaled += ScaleGiant;
    }
    void OnDisable()
    {
        GameEvents.OnRecievedSceneMeshFromHost -= SetPlayerAsSmall;
        GameEvents.OnLobbyHosted -= SetPlayerAsGiant;
        GameEvents.OnRoomScaled -= ScaleGiant;
    }

    void SetPlayerAsGiant()
    {
        giantPlayer.transform.position = Vector3.zero;
        
        giantPlayer.SetActive(true);
        smallPlayer.SetActive(false);
    }
    
    void SetPlayerAsSmall(Vector3 spawnPosition)
    {
        smallPlayer.transform.position = spawnPosition.Add(y: spawnHeight);
        
        giantPlayer.SetActive(false);
        smallPlayer.SetActive(true);
    }

    void ScaleGiant()
    {
        giantPlayer.transform.localScale = RoomEnvironmentInitializer.RoomScale;
    }
}
