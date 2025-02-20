using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace CodeBlack.Runtime.UI
{
    internal class VirtualScreen : GraphicRaycaster
    {
        public Camera screenCamera;

        public GraphicRaycaster screenCaster;

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            Ray ray = eventCamera.ScreenPointToRay(eventData.position); 
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.transform == transform)
                {
                    Vector3 virtualPos = new Vector3(hit.textureCoord.x, hit.textureCoord.y);
                    virtualPos.x *= screenCamera.targetTexture.width;
                    virtualPos.y *= screenCamera.targetTexture.height;

                    eventData.position = virtualPos;
                    screenCaster.Raycast(eventData, resultAppendList);
                }
            }
        }
    }
}
