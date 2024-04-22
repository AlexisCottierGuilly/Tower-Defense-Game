using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Notifier
{
    public string text;
    public float duration;
}

public class NotificationManager : MonoBehaviour
{
    public GameObject notificationPreview;
    public List<Notifier> waitingNotifications = new List<Notifier>();

    private float timeFromNotification;
    private float notificationDuration;
    private TextMeshProUGUI text;
    private Animator animator;

    private float durationMultiplier = 2f;

    void Start()
    {
        text = notificationPreview.GetComponent<TextMeshProUGUI>();
        animator = notificationPreview.GetComponent<Animator>();
    }

    public void ShowNotification(string text, float duration=2.5f)
    {
        Notifier notifier = new Notifier();
        notifier.text = text;
        notifier.duration = duration;
        waitingNotifications.Add(notifier);
    }

    void Notify(Notifier notification)
    {
        animator.speed = 1f / notification.duration * 2f / durationMultiplier;
        text.text = notification.text;

        animator.SetTrigger("Notify");
    }

    void Update()
    {
        timeFromNotification += Time.deltaTime;

        if (timeFromNotification >= notificationDuration)
        {
            if (waitingNotifications.Count > 0)
            {
                Notifier notification = waitingNotifications[0];
                waitingNotifications.RemoveAt(0);

                Notify(notification);
                notificationDuration = notification.duration * durationMultiplier;
                timeFromNotification = 0f;
            }
            else
            {
                timeFromNotification = 0f;
                notificationDuration = 0f;
            }
        }
    }
}
