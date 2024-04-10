using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace HappyKit
{
    public static class Input
    {
        public static Vector2 PointerPosToCanvasPos(RectTransform canvas, Vector2 pointerPos)
        {
            Vector2 clickedPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, pointerPos, Camera.main, out clickedPos);
            return clickedPos;
        }
        public static Vector2 PointerPosToCanvasPos(RectTransform canvas, Vector2 pointerPos, Camera camera)
        {
            Vector2 clickedPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, pointerPos, camera, out clickedPos);
            return clickedPos;
        }
        public static bool IsMouseOverObject(RectTransform UITransform)
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = UnityEngine.Input.mousePosition;

            List<RaycastResult> raycastResultList = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
            foreach (var raycast in raycastResultList)
            {
                if (raycast.gameObject.GetComponent<RectTransform>() == UITransform)
                    return true;
            }
            return false;
        }
        public static List<RectTransform> GetUIsUnderMouse()
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = UnityEngine.Input.mousePosition;

            List<RaycastResult> raycastResultList = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResultList);

            List<RectTransform> uiList = new List<RectTransform>();
            foreach (var raycast in raycastResultList)
            {
                RectTransform rt = raycast.gameObject.GetComponent<RectTransform>();
                if (rt != null)
                    uiList.Add(rt);
            }
            return uiList;
        }

    }
}