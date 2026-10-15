using UnityEngine;
public class MangoPickup : MonoBehaviour
{
    public MangoSeasonGame game;
    void OnTriggerEnter(Collider other)
    {
        if (other.transform == game.player) game.Collect(this);
    }
    void Update()
    {
        transform.Rotate(0, 35f * Time.deltaTime, 0, Space.World);
        // Also support a player already overlapping the fruit when play begins.
        if (game != null && game.player != null &&
            (game.player.position - transform.position).sqrMagnitude <= 1.3f * 1.3f)
            game.Collect(this);
    }
}
