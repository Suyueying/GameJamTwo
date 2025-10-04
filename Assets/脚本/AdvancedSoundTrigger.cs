using UnityEngine;
using System.Collections;

public class AdvancedSoundTrigger : MonoBehaviour
{
    [Header("音效设置")]
    public AudioClip soundClip;          // 要播放的音效
    public float volume = 1.0f;          // 音量大小
    public bool loop = false;            // 是否循环播放
    public bool playOnEnter = true;      // 进入时播放
    public bool stopOnExit = false;      // 离开时停止
    public float fadeInTime = 0.0f;      // 淡入时间（秒）
    public float fadeOutTime = 0.0f;     // 淡出时间（秒）

    [Header("触发设置")]
    public string playerTag = "Player";  // 玩家标签
    public bool requireKeyPress = false; // 是否需要按键触发
    public KeyCode triggerKey = KeyCode.E; // 触发按键

    [Header("冷却时间")]
    public bool useCooldown = false;     // 是否使用冷却时间
    public float cooldownDuration = 2.0f; // 冷却时间（秒）

    private AudioSource audioSource;
    private bool isPlayerInside = false;
    private bool isOnCooldown = false;
    private float originalVolume;

    void Start()
    {
        // 确保有AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 配置AudioSource
        audioSource.clip = soundClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;

        originalVolume = volume;
    }

    void Update()
    {
        // 如果需要按键触发且玩家在区域内
        if (requireKeyPress && isPlayerInside && Input.GetKeyDown(triggerKey) && !isOnCooldown)
        {
            PlaySound();

            // 如果使用冷却时间
            if (useCooldown)
            {
                StartCoroutine(Cooldown());
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的对象是否是玩家
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = true;

            // 如果需要进入时播放音效且不需要按键触发
            if (playOnEnter && !requireKeyPress && soundClip != null && !isOnCooldown)
            {
                PlaySound();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // 检查离开的对象是否是玩家
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = false;

            // 如果需要离开时停止音效
            if (stopOnExit && audioSource.isPlaying)
            {
                if (fadeOutTime > 0)
                {
                    StartCoroutine(FadeOut());
                }
                else
                {
                    audioSource.Stop();
                }
            }
        }
    }

    // 播放音效
    public void PlaySound()
    {
        if (soundClip != null && !isOnCooldown)
        {
            if (fadeInTime > 0)
            {
                StartCoroutine(FadeIn());
            }
            else
            {
                if (loop)
                {
                    audioSource.Play();
                }
                else
                {
                    audioSource.PlayOneShot(soundClip, volume);
                }
            }

            // 如果使用冷却时间
            if (useCooldown)
            {
                StartCoroutine(Cooldown());
            }
        }
    }

    // 停止音效
    public void StopSound()
    {
        if (audioSource.isPlaying)
        {
            if (fadeOutTime > 0)
            {
                StartCoroutine(FadeOut());
            }
            else
            {
                audioSource.Stop();
            }
        }
    }

    // 淡入效果
    IEnumerator FadeIn()
    {
        audioSource.volume = 0;
        audioSource.Play();

        float timer = 0;
        while (timer < fadeInTime)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(0, originalVolume, timer / fadeInTime);
            yield return null;
        }

        audioSource.volume = originalVolume;
    }

    // 淡出效果
    IEnumerator FadeOut()
    {
        float startVolume = audioSource.volume;
        float timer = 0;

        while (timer < fadeOutTime)
        {
            timer += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0, timer / fadeOutTime);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = originalVolume;
    }

    // 冷却时间协程
    IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
    }

    // 检查玩家是否在触发区域内
    public bool IsPlayerInside()
    {
        return isPlayerInside;
    }

    // 设置音量
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
        originalVolume = volume;
    }
}