using System.Collections.Generic;
using UnityEngine;

namespace Snek.Utilities
{
    public static class SnekListExtensions
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            if(list == null)
            {
                Debug.LogError("Cannot shuffle a List which is not initialized (Null).");

                return;
            }

            for (int i = list.Count - 1; i > 0; i--)
            {
                int randomIndex = Random.Range(0, i + 1);

                list.SwapListElements(randomIndex, i);
            }
        }

        public static void SwapListElements<T>(this IList<T> list, int currentIndex, int targetIndex)
        {
            T movedElement = list[currentIndex];

            list[currentIndex] = list[targetIndex];
            list[targetIndex] = movedElement;
        }

        public static bool HasIndex<T>(this IList<T> list, int index)
        {
            if (list == null)
            {
                Debug.LogError("Cannot look for index in a List which is not initialized (Null).");

                return false;
            }

            return index >= 0 && index < list.Count;
        }

        public static int GetRandomIndex<T>(this IList<T> list)
        {
            if (list == null)
            {
                Debug.LogError("Cannot look for index in a List which is not initialized (Null).");

                return -1;
            }

            if (list.Count == 0)
            {
                Debug.LogError("Cannot look for index in a List which is empty.");

                return -1;
            }

            return Random.Range(0, list.Count);
        }
    }
}
