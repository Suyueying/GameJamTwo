using UnityEngine;
using System.Collections;

public class AdvancedSoundTrigger : MonoBehaviour
{
    [Header("��Ч����")]
    public AudioClip soundClip;          // Ҫ���ŵ���Ч
    public float volume = 1.0f;          // ������С
    public bool loop = false;            // �Ƿ�ѭ������
    public bool playOnEnter = true;      // ����ʱ����
    public bool stopOnExit = false;      // �뿪ʱֹͣ
    public float fadeInTime = 0.0f;      // ����ʱ�䣨�룩
    public float fadeOutTime = 0.0f;     // ����ʱ�䣨�룩

    [Header("��������")]
    public string playerTag = "Player";  // ��ұ�ǩ
    public bool requireKeyPress = false; // �Ƿ���Ҫ��������
    public KeyCode triggerKey = KeyCode.E; // ��������

    [Header("��ȴʱ��")]
    public bool useCooldown = false;     // �Ƿ�ʹ����ȴʱ��
    public float cooldownDuration = 2.0f; // ��ȴʱ�䣨�룩

    private AudioSource audioSource;
    private bool isPlayerInside = false;
    private bool isOnCooldown = false;
    private float originalVolume;

    void Start()
    {
        // ȷ����AudioSource���
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // ����AudioSource
        audioSource.clip = soundClip;
        audioSource.volume = volume;
        audioSource.loop = loop;
        audioSource.playOnAwake = false;

        originalVolume = volume;
    }

    void Update()
    {
        // �����Ҫ���������������������
        if (requireKeyPress && isPlayerInside && Input.GetKeyDown(triggerKey) && !isOnCooldown)
        {
            PlaySound();

            // ���ʹ����ȴʱ��
            if (useCooldown)
            {
                StartCoroutine(Cooldown());
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // ������Ķ����Ƿ������
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = true;

            // �����Ҫ����ʱ������Ч�Ҳ���Ҫ��������
            if (playOnEnter && !requireKeyPress && soundClip != null && !isOnCooldown)
            {
                PlaySound();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // ����뿪�Ķ����Ƿ������
        if (other.CompareTag(playerTag))
        {
            isPlayerInside = false;

            // �����Ҫ�뿪ʱֹͣ��Ч
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

    // ������Ч
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

            // ���ʹ����ȴʱ��
            if (useCooldown)
            {
                StartCoroutine(Cooldown());
            }
        }
    }

    // ֹͣ��Ч
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

    // ����Ч��
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

    // ����Ч��
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

    // ��ȴʱ��Э��
    IEnumerator Cooldown()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldownDuration);
        isOnCooldown = false;
    }

    // �������Ƿ��ڴ���������
    public bool IsPlayerInside()
    {
        return isPlayerInside;
    }

    // ��������
    public void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
        audioSource.volume = volume;
        originalVolume = volume;
    }
}