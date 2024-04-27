using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetObjectSnapshot : MonoBehaviour
{
    private SnapshotCamera snapshotCamera;
    public GameObject gameObjectToSnapshot;
    public Image image;
    public Color color;

    void Start()
    {
        if (image.sprite != null)
        {
            return;
        }
        
        snapshotCamera = SnapshotCamera.MakeSnapshotCamera(0);

        Color background = color;
        Texture2D snapshot = snapshotCamera.TakePrefabSnapshot(gameObjectToSnapshot, background);

        image.sprite = Sprite.Create(snapshot, new Rect(0, 0, snapshot.width, snapshot.height), new Vector2(0.5f, 0.5f));
    }
}
