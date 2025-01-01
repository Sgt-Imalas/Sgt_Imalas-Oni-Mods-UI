using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class collidertester : MonoBehaviour
{
    PolygonCollider2D collider;
    SpriteRenderer sprite;
    public Vector3 worldPosition;
    public TextMesh text;

    // Update is called once per frame
    void Update()
    {
        if(sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        if(collider == null)
        {
            collider = GetComponent<PolygonCollider2D>();
        }

        Vector3 screenPosition = Input.mousePosition;
        screenPosition.z = Camera.main.transform.position.y - transform.position.y;

        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        if(text != null)
        {
            text.text = worldPosition.ToString() + " "  + Input.mousePosition.ToString();
        }

        if(collider.OverlapPoint(worldPosition))
        {
            sprite.color = Color.green;
        }
        else
        {
            sprite.color = Color.red;
        }
    }
}
