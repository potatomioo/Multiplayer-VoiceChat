using UnityEngine;
using Photon.Pun;
using Agora.Rtc;

public class PlayerController : MonoBehaviourPunCallbacks
{
    public float moveSpeed = 5f;
    private PhotonView photonView;
    private IRtcEngine rtcEngine;
    private string channelName;

    void Start()
    {
        photonView = GetComponent<PhotonView>();
        channelName = "Channel_" + Random.Range(1000, 9999); // Random channel name for bonus points

        if (photonView.IsMine)
        {
            rtcEngine = RtcEngine.CreateAgoraRtcEngine();
            var logConfig = new LogConfig(null, 1024, LOG_LEVEL.LOG_LEVEL_INFO);
            var context = new RtcEngineContext(
                "8ad8fcec5bb942b98e32d3fd80900307",  // Replace with your Agora App ID
                0,                    // App Certificate (use 0 if not applicable)
                CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION,
                "",                   // Token (leave empty if not used)
                AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_DEFAULT,
                AREA_CODE.AREA_CODE_GLOB,
                logConfig,           // Pass the log configuration
                new Optional<THREAD_PRIORITY_TYPE>(), // Default thread priority
                true,                // Enable audio device management
                true,                // Enable video device management
                true                 // Enable event handling
            );

            rtcEngine.Initialize(context);
            rtcEngine.EnableAudio();
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            Move();
        }
    }

    void Move()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector2 movement = new Vector2(moveHorizontal, moveVertical) * moveSpeed * Time.deltaTime;
        transform.Translate(movement);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (photonView.IsMine && other.CompareTag("Player"))
        {
            rtcEngine.JoinChannel("", channelName, "", 0);
            Debug.Log("Joined channel: " + channelName);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (photonView.IsMine && other.CompareTag("Player"))
        {
            rtcEngine.LeaveChannel();
            Debug.Log("Left channel: " + channelName);
        }
    }

    void OnDestroy()
    {
        if (photonView.IsMine)
        {
            rtcEngine.Dispose();
        }
    }
}
