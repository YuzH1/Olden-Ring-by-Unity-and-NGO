using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SG
{
    public static class SingletonUtility
    {
        public static bool TrySetInstance<T>(T candidate, ref T currentInstance) where T : MonoBehaviour
        {
            if (currentInstance == null || currentInstance == candidate)
            {
                currentInstance = candidate;
                return true;
            }

#if UNITY_EDITOR
            ReselectDuplicate(candidate.gameObject, currentInstance.gameObject);
#endif

            candidate.gameObject.SetActive(false);
            Object.Destroy(candidate.gameObject);
            return false;
        }

#if UNITY_EDITOR
        private static void ReselectDuplicate(GameObject duplicateObject, GameObject survivingObject)
        {
            Object[] currentSelection = Selection.objects;

            if (currentSelection == null || currentSelection.Length == 0)
            {
                return;
            }

            List<Object> updatedSelection = new List<Object>(currentSelection.Length);
            bool removedDuplicate = false;

            for (int index = 0; index < currentSelection.Length; index++)
            {
                Object selectedObject = currentSelection[index];

                // Filter out stale destroyed references before reassigning Selection.
                if (selectedObject == null)
                {
                    continue;
                }

                if (selectedObject == duplicateObject)
                {
                    removedDuplicate = true;

                    if (!updatedSelection.Contains(survivingObject))
                    {
                        updatedSelection.Add(survivingObject);
                    }

                    continue;
                }

                updatedSelection.Add(selectedObject);
            }

            if (!removedDuplicate)
            {
                return;
            }

            if (survivingObject != null && !updatedSelection.Contains(survivingObject))
            {
                updatedSelection.Add(survivingObject);
            }

            Selection.objects = updatedSelection.ToArray();
            Selection.activeGameObject = survivingObject;
        }
#endif
    }
}