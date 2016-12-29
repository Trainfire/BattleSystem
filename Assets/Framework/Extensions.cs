using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Framework
{
    static class ListEx
    {
        /// <summary>
        /// Returns true if the index is within range of the list. Always returns false is the list is empty.
        /// </summary>
        public static bool InRange<T>(this IList<T> list, int index)
        {
            if (list.Count == 0)
            {
                return false;
            }
            else
            {
                return index >= 0 && index <= list.Count - 1;
            }
        }
    }

    static class ActionEx
    {
        public static void InvokeSafe(this Action action)
        {
            if (action != null)
                action.Invoke();
        }

        public static void InvokeSafe<T>(this Action<T> action, T arg)
        {
            if (action != null)
                action.Invoke(arg);
        }

        public static void InvokeSafe<T1, T2>(this Action<T1 ,T2> action, T1 arg1, T2 arg2)
        {
            if (action != null)
                action.Invoke(arg1, arg2);
        }
    }

    static class Vector3Ex
    {
        public static Vector2 ToVec2(this Vector3 v)
        {
            return new Vector2(v.x, v.y);
        }
    }

    static class Vector2Ex
    {
        public static Vector3 ToVec3(this Vector2 v)
        {
            return new Vector3(v.x, v.y);
        }
    }

    static class ObjectEx
    {
        public static T FindObjectOfType<T>(Action<T> onFind = null) where T : MonoBehaviour
        {
            var obj = GameObject.FindObjectOfType<T>();
            Assert.IsNotNull(obj, string.Format("A GameObject with the {0} component must exist somewhere in the scene.", typeof(T).FullName));
            if (onFind != null)
                onFind(obj);
            return obj;
        }
    }

    static class GameObjectEx
    {
        public static void SetLayers(this GameObject go, int layer)
        {
            foreach (Transform t in go.transform)
            {
                t.gameObject.layer = layer;
            }
        }

        public static T GetComponent<T>(this GameObject obj, Action<T> onGet)
        {
            var comp = obj.GetComponent<T>();
            if (comp != null)
                onGet(comp);
            return comp;
        }

        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            var comp = obj.GetComponent<T>();
            if (comp != null)
            {
                return comp;
            }
            else
            {
                return obj.gameObject.AddComponent<T>();
            }
        }

        public static List<Transform> GetChildTransforms(this GameObject obj, bool includeInActive = false)
        {
            var children = obj.GetComponentsInChildren<Transform>(includeInActive);
            var childTransforms = new List<Transform>();
            foreach (var child in children)
            {
                if (child.parent == obj.transform)
                {
                    childTransforms.Add(child);
                }
            }
            return childTransforms;
        }

        public static T FindComponentWithTag<T>(string tag) where T : MonoBehaviour
        {
            var gameObject = GameObject.FindGameObjectWithTag(tag);
            if (gameObject == null)
            {
                Debug.LogError("Failed to find a GameObject with tag '" + tag + "'");
            }
            else
            {
                var component = gameObject.GetComponent<T>();
                if (component == null)
                {
                    Debug.LogErrorFormat("Found GameObject with tag '{0}' but it does not have the component '{1}'", tag, typeof(T).Name);
                }
                else
                {
                    return component;
                }
            }
            return null;
        }

        public static List<T> FindComponentsWithTag<T>(string tag) where T : MonoBehaviour
        {
            var components = new List<T>();

            var gameObjects = GameObject.FindGameObjectsWithTag(tag);
            if (gameObjects.Length == 0)
            {
                Debug.LogError("Failed to find GameObjects with tag '" + tag + "'");
            }
            else
            {
                foreach (var gameObject in gameObjects)
                {
                    var component = gameObject.GetComponent<T>();
                    if (component == null)
                    {
                        Debug.LogErrorFormat("Found GameObject with tag '{0}' but it does not have the component '{1}'", tag, typeof(T).Name);
                    }
                    else
                    {
                        components.Add(component);
                    }
                }

            }
            return components;
        }
    }

    public static class UiEx
    {
        public static void Focus(this InputField inputField)
        {
            // Hack to keep focus on inputfield after submitting a command. Classic Unity.
            EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
            inputField.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        public static void ScrollToBottom(this ScrollRect scrollRect)
        {
            scrollRect.normalizedPosition = new Vector2(scrollRect.normalizedPosition.x, 0);
        }

        public static Vector2 WorldToCanvasPoint(this Canvas canvas, Vector3 worldPosition)
        {
            var canvasRect = canvas.transform as RectTransform;

            var viewportPoint = Camera.main.WorldToViewportPoint(worldPosition);

            var screenX = (viewportPoint.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f);
            var screenY = (viewportPoint.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f);

            return new Vector2(screenX, screenY);
        }
    }
}
