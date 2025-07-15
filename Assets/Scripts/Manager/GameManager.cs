using UnityEngine;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    public CRTGlitchTester crtGlitch;
    public PlayerController player;
    public Transform playerSpawnPoint;
    public EdgeTileManager tileManager;

    public WorldRotator worldRotator;

    private AudioSource audio1;


    void Start()
    {
        crtGlitch = FindFirstObjectByType<CRTGlitchTester>();
        player = FindFirstObjectByType<PlayerController>();
        playerSpawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        tileManager = FindFirstObjectByType<EdgeTileManager>();
        worldRotator = FindFirstObjectByType<WorldRotator>();
        audio1 = GetComponentInChildren<AudioSource>();
    }
    public void SoftReset()
    {
        //crtGlitch.TestPowerOffEffect();
        crtGlitch.SoftResetDie();
        worldRotator.ResetRotation();
        audio1.Play();
        DOVirtual.DelayedCall(0.3f, () =>
        {
            tileManager.ResetAllTiles(); 

            player.transform.position = playerSpawnPoint.position;
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            player.isHittable = true;



        });
    }
}
