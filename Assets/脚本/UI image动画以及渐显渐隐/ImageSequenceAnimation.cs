using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageSequenceAnimation : MonoBehaviour
{
    [Header("动画设置")]
    [Tooltip("序列帧图片数组")]
    public Sprite[] frames;
    [Tooltip("动画播放速度（每秒帧数）")]
    [Range(1, 60)] public float frameRate = 12f;
    [Tooltip("是否循环播放")]
    public bool loop = true;
    [Tooltip("是否在开始时自动播放")]
    public bool playOnStart = true;
    [Tooltip("是否在播放完成后销毁")]
    public bool destroyOnComplete = false;

    private Image imageComponent;
    private int currentFrame = 0;
    private bool isPlaying = false;
    private float frameTimer = 0f;

    private void Awake()
    {
        imageComponent = GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogError("ImageSequenceAnimation: 未找到 Image 组件！");
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        if (playOnStart)
        {
            Play();
        }
    }

    private void Update()
    {
        if (!isPlaying || frames == null || frames.Length == 0) return;

        frameTimer += Time.deltaTime;
        float frameInterval = 1f / frameRate;

        if (frameTimer >= frameInterval)
        {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;

            // 更新图片
            imageComponent.sprite = frames[currentFrame];

            // 检查是否播放完成
            if (currentFrame == frames.Length - 1)
            {
                if (!loop)
                {
                    isPlaying = false;
                    if (destroyOnComplete)
                    {
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    // 开始播放动画
    public void Play()
    {
        if (frames == null || frames.Length == 0)
        {
            Debug.LogWarning("ImageSequenceAnimation: 没有设置序列帧图片！");
            return;
        }

        isPlaying = true;
        currentFrame = 0;
        frameTimer = 0f;
        imageComponent.sprite = frames[0];
    }

    // 停止播放动画
    public void Stop()
    {
        isPlaying = false;
    }

    // 暂停播放动画
    public void Pause()
    {
        isPlaying = false;
    }

    // 继续播放动画
    public void Resume()
    {
        isPlaying = true;
    }

    // 设置动画帧
    public void SetFrames(Sprite[] newFrames)
    {
        frames = newFrames;
        if (isPlaying)
        {
            currentFrame = 0;
            frameTimer = 0f;
            imageComponent.sprite = frames[0];
        }
    }

    // 设置播放速度
    public void SetFrameRate(float newFrameRate)
    {
        frameRate = Mathf.Clamp(newFrameRate, 1f, 60f);
    }
} 