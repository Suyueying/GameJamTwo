using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    [Header("音效设置")]
    public AudioClip soundEffect;      // 要播放的音效
    public float volume = 1.0f;        // 音量大小 (0.0 到 1.0)

    [Header("触发设置")]
    public string targetTag = "Item";  // 要触发的物品标签
    public bool playOnce = true;       // 是否只播放一次

    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        // 获取或创建AudioSource组件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // 设置AudioSource属性
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查碰撞对象的标签
        if (other.CompareTag(targetTag))
        {
            // 如果设置为只播放一次且已经播放过，则返回
            if (playOnce && hasPlayed)
                return;

            // 播放音效
            PlaySound();

            // 标记为已播放
            hasPlayed = true;

            Debug.Log($"播放音效: {soundEffect.name} (标签: {other.tag})");
        }
    }

    // 播放音效的方法
    public void PlaySound()
    {
        if (soundEffect != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundEffect, volume);
        }
        else
        {
            Debug.LogWarning("音效或AudioSource未设置！");
        }
    }

    // 手动触发播放（可以从其他脚本调用）
    public void ManualPlaySound()
    {
        PlaySound();
    }

    // 重置播放状态（允许再次播放）
    public void ResetSound()
    {
        hasPlayed = false;
    }

    // 设置新的音效
    public void SetSoundEffect(AudioClip newSound)
    {
        soundEffect = newSound;
    }

    // 设置新的目标标签
    public void SetTargetTag(string newTag)
    {
        targetTag = newTag;
    }
}