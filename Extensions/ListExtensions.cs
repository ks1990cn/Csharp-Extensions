using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Extensions
{
    public static class IListExtensions
    {

        /// <summary>
        /// Simple Extension Method for IList.
        /// Checks and sees if the item is already in the IList.
        /// If the item is in the IList, returns false,
        /// otherwise, it adds the item to the IList and returns true.
        /// </summary>
        /// <typeparam name="T">The type of the IList</typeparam>
        /// <param name="iList">The IList to add to</param>
        /// <param name="item">The item to try to add to the IList if it does not already exist in the IList</param>
        /// <returns>True for successfully adding the item to the IList. False otherwise</returns>
        public static bool TryAdd<T>(this IList<T> iList, T item)
        {
            if (!iList.Contains(item))
            {
                iList.Add(item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Swap two elements of a list.
        /// </summary>
        /// <param name="list"></param>
        /// <param name="indexA"></param>
        /// <param name="indexB"></param>
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        public static void MatchCount<T>(this IList<T> list, IList<T> against)
        {
            int myCount = list.Count;
            int matchingCount = against.Count;

            if (myCount < matchingCount)
            {
                for (int i = myCount; i < matchingCount; ++i)
                {
                    list.Add(against[i]);
                }
            }
            else if (myCount > matchingCount)
            {
                for (int i = matchingCount - 1; i >= myCount; --i)
                {
                    list.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Clone an IList (Does not clone members if they are references).
        /// </summary>
        /// <typeparam name="T">List Type</typeparam>
        /// <param name="listToClone"></param>
        /// <returns></returns>
        public static IList<T> Clone<T>(this IList<T> listToClone)
        {
            return listToClone.ToList();
        }

        public static System.Collections.IList Clone2(this System.Collections.IList listToClone)
        {

            if (listToClone is List<Guid?>)
            {
                return (listToClone as List<Guid?>).ToList();
            }
            if (listToClone is List<string>)
            {
                return (listToClone as List<string>).ToList();
            }
            if (listToClone is List<double?>)
            {
                return (listToClone as List<double?>).ToList();
            }
            if (listToClone is List<int?>)
            {
                return (listToClone as List<int?>).ToList();
            }
            return listToClone;
        }
    }
}
