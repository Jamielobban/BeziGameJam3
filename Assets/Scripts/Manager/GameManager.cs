using UnityEngine;
using DG.Tweening;
public class GameManager : MonoBehaviour
{
    public CRTGlitchTester crtGlitch;
    public PlayerController player;
    public Transform playerSpawnPoint;
    public EdgeTileManager tileManager;
    public ScreenRotator screenRotator;

    public void SoftReset()
    {
        //crtGlitch.TestPowerOffEffect();
        screenRotator.ResetRotation();
        tileManager.ResetAllTiles(); // You'll need to implement this
        crtGlitch.SoftResetDie();

        DOVirtual.DelayedCall(0.3f, () =>
        {
            // Reset player
            player.transform.position = playerSpawnPoint.position;
            player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            // Reset tiles


        });
    }
}
