using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    [Header("��Ч����")]
    public AudioClip soundEffect;      // Ҫ���ŵ���Ч
    public float volume = 1.0f;        // ������С (0.0 �� 1.0)

    [Header("��������")]
    public string targetTag = "Item";  // Ҫ��������Ʒ��ǩ
    public bool playOnce = true;       // �Ƿ�ֻ����һ��

    private AudioSource audioSource;
    private bool hasPlayed = false;

    void Start()
    {
        // ��ȡ�򴴽�AudioSource���
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ����AudioSource����
        audioSource.playOnAwake = false;
        audioSource.volume = volume;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // �����ײ����ı�ǩ
        if (other.CompareTag(targetTag))
        {
            // �������Ϊֻ����һ�����Ѿ����Ź����򷵻�
            if (playOnce && hasPlayed)
                return;

            // ������Ч
            PlaySound();

            // ���Ϊ�Ѳ���
            hasPlayed = true;

            Debug.Log($"������Ч: {soundEffect.name} (��ǩ: {other.tag})");
        }
    }

    // ������Ч�ķ���
    public void PlaySound()
    {
        if (soundEffect != null && audioSource != null)
        {
            audioSource.PlayOneShot(soundEffect, volume);
        }
        else
        {
            Debug.LogWarning("��Ч��AudioSourceδ���ã�");
        }
    }

    // �ֶ��������ţ����Դ������ű����ã�
    public void ManualPlaySound()
    {
        PlaySound();
    }

    // ���ò���״̬�������ٴβ��ţ�
    public void ResetSound()
    {
        hasPlayed = false;
    }

    // �����µ���Ч
    public void SetSoundEffect(AudioClip newSound)
    {
        soundEffect = newSound;
    }

    // �����µ�Ŀ���ǩ
    public void SetTargetTag(string newTag)
    {
        targetTag = newTag;
    }
}