using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorldInteraction : MonoBehaviour
{
	NavMeshAgent playerAgent;

	void Start()
	{
		playerAgent = GetComponent<NavMeshAgent>();
	}

    void Update()
    {
        if (Input.GetMouseButton(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        {
            GetInteraction();
        }
    }
    void GetInteraction()
    {
        RaycastHit interactionInfo;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out interactionInfo, Mathf.Infinity))
        {
            GameObject interactedObject = interactionInfo.collider.gameObject;

            if (interactedObject.tag == "Interactable Object")
            {
                
            }
            else
            {
				playerAgent.destination = interactionInfo.point;
            }
        }
    }
}
