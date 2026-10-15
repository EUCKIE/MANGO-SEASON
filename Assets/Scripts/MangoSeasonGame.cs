using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class MangoSeasonGame : MonoBehaviour
{
    public int Score { get; private set; }
    public int Total = 8;
    public TMP_Text scoreText;
    public TMP_Text messageText;
    public Transform player;
    float messageUntil;
    Rigidbody body;
    void Awake() { body = player.GetComponent<Rigidbody>(); RefreshScore(); }
    void Update()
    {
        if (Time.time > messageUntil) messageText.text = "Collect the golden mangoes. Avoid the red boundaries.";
    }
    void FixedUpdate()
    {
        var k = Keyboard.current;
        if (k == null) return;
        float x = (k.dKey.isPressed || k.rightArrowKey.isPressed ? 1 : 0) - (k.aKey.isPressed || k.leftArrowKey.isPressed ? 1 : 0);
        float z = (k.wKey.isPressed || k.upArrowKey.isPressed ? 1 : 0) - (k.sKey.isPressed || k.downArrowKey.isPressed ? 1 : 0);
        var move = Vector3.ClampMagnitude(new Vector3(x, 0, z), 1);
        body.MovePosition(body.position + move * 6f * Time.fixedDeltaTime);
    }
    public void Collect(MangoPickup mango)
    {
        if (!mango.gameObject.activeSelf) return;
        Score++;
        RefreshScore();
        messageText.text = Score == Total ? "Harvest complete! All mangoes collected." : "+1 MANGO! Keep harvesting.";
        messageUntil = Time.time + 4f;
        var fx = new GameObject("Golden collection burst");
        fx.transform.position = mango.transform.position;
        var ps = fx.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        var main = ps.main;
        main.startColor = new Color(1f, .75f, .08f);
        main.startSize = .16f; main.startSpeed = 2.5f; main.startLifetime = .9f;
        main.duration = 1f; main.loop = false; main.gravityModifier = .4f;
        var emission = ps.emission; emission.rateOverTime = 0;
        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        ps.Play();
        ps.Emit(32);
        Destroy(fx, 2f);
        mango.gameObject.SetActive(false);
    }
    void RefreshScore() { scoreText.text = "MANGOES  " + Score + " / " + Total; }
}
