using UnityEngine;

/// <summary>
/// Trigger zone yang mengaktifkan berbagai jenis trap.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TrapTrigger : MonoBehaviour
{
    [Header("Trap Type")]
    [Tooltip("Batu yang jatuh dari atas")]
    public FallingRock[] fallingRocks;
    
    [Tooltip("Obstacle yang naik dari bawah")]
    public RisingObstacle[] risingObstacles;
    
    [Tooltip("Obstacle yang bergerak horizontal")]
    public MovingObstacle[] movingObstacles;
    
    [Tooltip("Bola yang menggelinding")]
    public RollingBall[] rollingBalls;
    
    private bool hasTriggered = false;

    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (hasTriggered) return;
        
        hasTriggered = true;
        
        // Aktifkan batu jatuh
        if (fallingRocks != null)
        {
            foreach (FallingRock rock in fallingRocks)
            {
                if (rock != null) rock.StartFalling();
            }
        }
        
        // Aktifkan obstacle naik
        if (risingObstacles != null)
        {
            foreach (RisingObstacle obstacle in risingObstacles)
            {
                if (obstacle != null) obstacle.StartRising();
            }
        }
        
        // Aktifkan obstacle bergerak horizontal
        if (movingObstacles != null)
        {
            foreach (MovingObstacle obstacle in movingObstacles)
            {
                if (obstacle != null) obstacle.StartMoving();
            }
        }
        
        // Aktifkan bola menggelinding
        if (rollingBalls != null)
        {
            foreach (RollingBall ball in rollingBalls)
            {
                if (ball != null) ball.StartRolling();
            }
        }
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}
