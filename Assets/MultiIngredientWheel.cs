using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class MultiIngredientWheel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Transform RotatableContainer;
    public GameObject CenterObject;

	public bool hovered = false;

	public void OnPointerEnter(PointerEventData eventData)
	{
		hovered = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		hovered = false;
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int childCount = RotatableContainer.childCount;
		transform.localScale = Vector3.one * (hovered ? 2.25f : 1f);


		var distance = 35;

		for (int i = 0; i < childCount; i++)
        {
            Transform child = RotatableContainer.GetChild(i);
            float angle = (360f / childCount) * i;
			child.localPosition = Quaternion.Euler(0, 0, angle) * Vector3.up * distance;
			child.localScale = Vector3.one * (hovered ? 1.0f : 1f);
		}

	}
}
