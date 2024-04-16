using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImageFromPrefab : MonoBehaviour
{
    public GameObject prefab;
    public Image image;

    void Start()
    {
        Vector2 size = new Vector2(256, 256);
        
        // make a copy of the prefab, and set its rotation to look at it from the front, 20 degrees over
        GameObject instance = Instantiate(prefab);
        instance = RotateObjectAndChildren(instance, new Vector3(0, 20, 0));
        
        Texture2D preview = RuntimePreviewGenerator.GenerateModelPreview(instance.transform, (int)size.x, (int)size.y);

        /*
        Crop the image to get :
        - half of the width (centered)
        - half of the height (bottom)
        
        
        int width = (int)size.x / 2;
        int height = (int)size.y / 2;
        int x = (int)size.x / 4;
        int y = 0;*/

        if (image != null)
        {
            //Sprite sprite = Sprite.Create(preview, new Rect(x, y, width, height), new Vector2(0.5f, 0.5f));
            Sprite sprite = Sprite.Create(preview, new Rect(0, 0, size.x, size.y), new Vector2(0.5f, 0.5f));
            image.sprite = sprite;
        }
    }

    /*
    function RotateMesh() 
    {
        var qAngle : Quaternion = Quaternion.AngleAxis( rotatedAngleY, Vector3.up );

        for (var vert : int = 0; vert < originalVerts.length; vert ++)
        {
            rotatedVerts[vert] = qAngle * originalVerts[vert];
        }
        
        theMesh.vertices = rotatedVerts;
    }
    */

    GameObject RotateObjectAndChildren(GameObject go, Vector3 rotation)
    {
        // rotate the mesh of the gameobject and all its children. Their rotation should not be changed.
        // remember to call RotatedObject() to rotate the mesh of the gameobject

        Destroy(go);

        // create a new gameobject
        GameObject rotated = new GameObject();
        rotated.transform.position = go.transform.position;
        rotated.transform.rotation = go.transform.rotation;
        rotated.transform.localScale = go.transform.localScale;

        // rotate the mesh of the gameobject
        rotated = RotatedObject(go, rotation);

        // rotate the mesh of the children
        foreach (Transform child in go.transform)
        {
            GameObject rotatedChild = RotateObjectAndChildren(child.gameObject, rotation);
            rotatedChild.transform.parent = rotated.transform;
        }

        return rotated;
    }

    GameObject RotatedObject(GameObject go, Vector3 rotation)
    {
        // rotate the mesh of the gameobject. Its rotation should not be changed.
        
        // create a new gameobject
        GameObject rotated = new GameObject();
        rotated.transform.position = go.transform.position;
        rotated.transform.rotation = go.transform.rotation;
        rotated.transform.localScale = go.transform.localScale;

        // create a new mesh
        Mesh mesh = new Mesh();
        mesh.vertices = go.GetComponent<MeshFilter>().mesh.vertices;
        mesh.triangles = go.GetComponent<MeshFilter>().mesh.triangles;
        mesh.uv = go.GetComponent<MeshFilter>().mesh.uv;
        mesh.normals = go.GetComponent<MeshFilter>().mesh.normals;

        // rotate the mesh
        Quaternion qAngle = Quaternion.Euler(rotation);
        Vector3[] rotatedVerts = new Vector3[mesh.vertices.Length];
        for (int vert = 0; vert < mesh.vertices.Length; vert++)
        {
            rotatedVerts[vert] = qAngle * mesh.vertices[vert];
        }
        mesh.vertices = rotatedVerts;

        // apply the mesh to the rotated gameobject
        rotated.AddComponent<MeshFilter>().mesh = mesh;
        rotated.AddComponent<MeshRenderer>().material = go.GetComponent<MeshRenderer>().material;

        return rotated;
    }
}
