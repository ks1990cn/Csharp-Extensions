using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class CollectionExtensions
    {
        /// <summary>
        /// Converts a type into a DataTable compatible type
        /// </summary>
        /// <param name="type">The type to convert</param>
        /// <returns>DataTable compatible type</returns>
        public static Type MarshallToTableType(Type type)
        {
            // Convert nullable types to their non-nullable version
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
            }

            if (type.IsEnum)
            {
                type = type.GetEnumUnderlyingType();
            }

            if (type == typeof(Int32)
                || type == typeof(Guid)
                || type == typeof(String)
                || type == typeof(Double)
                || type == typeof(DateTime)
                || type == typeof(Single)
                || type == typeof(Boolean)
                || type == typeof(Char)
                || type == typeof(Decimal)
                || type == typeof(Int64)
                || type == typeof(TimeSpan)
                || type == typeof(UInt32)
                || type == typeof(UInt64)
                || type == typeof(UInt32)
                || type == typeof(Int16)
                || type == typeof(SByte)
                || type == typeof(UInt16)
                || type == typeof(List<string>)
                || type == typeof(List<int?>)
                || type == typeof(List<Guid?>)
                || type == typeof(List<double?>)
                || type == typeof(List<DateTime?>))
            {
                return type;
            }
            else
            {
                return typeof(object);
            }
        }

        /// <summary>
        /// Performs the given action for each item in the sequence.
        /// </summary>
        /// <typeparam name="T">Type of item being iterated on.</typeparam>
        /// <param name="items">Sequence of items being operated on.</param>
        /// <param name="action">The action to be performed.</param>
        /// <returns>The original sequence.</returns>
        public static IEnumerable<T> Each<T>(this IEnumerable<T> items, Action<T> action)
        {
            if (items == null) return null;
            foreach (T item in items)
            {
                T capturedItem = item; // Necessary to prevent captured variable bug.
                action(capturedItem);
            }

            return items;
        }

        /// <summary>
        /// Performs the given action for each item in the list.
        /// </summary>
        /// <typeparam name="T">Type of item being iterated on.</typeparam>
        /// <param name="items">List of items being operated on.</param>
        /// <param name="action">The action to be performed.</param>
        /// <returns>The original sequence.</returns>
        public static IList<T> Each<T>(this IList<T> items, Action<T> action)
        {
            if (items == null)
                return items;

            for (int i = 0; i < items.Count; i++)
            {
                T capturedItem = items[i]; // Necessary to prevent captured variable bug.
                action(capturedItem);
            }

            return items;
        }

        public static double StandardDeviation(this IList<double> items)
        {
            double ret = 0;

            if (items.Count() > 0)
            {
                //std = sqrt(E[X^2]-E[X]^2)
                //E[X^2]
                double term1 = items.Average(x => Math.Pow(x, 2));

                //E[X]^2
                double term2 = Math.Pow(items.Average(), 2);

                ret = Math.Sqrt(term1 - term2);
            }
            return ret;
        }

        /// <summary>
        /// Performs the given action for each item in the array.
        /// </summary>
        /// <typeparam name="T">Type of item being iterated on.</typeparam>
        /// <param name="items">Array of items being operated on.</param>
        /// <param name="action">The action to be performed.</param>
        /// <returns>The original sequence.</returns>
        public static T[] Each<T>(this T[] items, Action<T> action)
        {

            for (int i = 0; i < items.Length; i++)
            {
                T capturedItem = items[i]; // Necessary to prevent captured variable bug.
                action(capturedItem);
            }

            return items;
        }

        /// <summary>
        /// Performs the given action for each item in the sequence.
        /// </summary>
        /// <typeparam name="T">Type of item being iterated on.</typeparam>
        /// <param name="items">Sequence of items being operated on.</param>
        /// <param name="action">The action to be performed, which may also use the index variable.</param>
        /// <returns>The original sequence.</returns>
        public static IEnumerable<T> Each<T>(this IEnumerable<T> items, Action<T, int> action)
        {
            int i = 0;
            foreach (T item in items)
            {
                T capturedItem = item; // Necessary to prevent captured variable bug.
                action(capturedItem, i++);
            }

            return items;
        }

        /// <summary>
        /// Performs the given action for each item in the list.
        /// </summary>
        /// <typeparam name="T">Type of item being iterated on.</typeparam>
        /// <param name="items">List of items being operated on.</param>
        /// <param name="action">The action to be performed, which may also use the index variable.</param>
        /// <returns>The original sequence.</returns>
        public static IList<T> Each<T>(this IList<T> items, Action<T, int> action)
        {

            for (int i = 0; i < items.Count; i++)
            {
                T capturedItem = items[i]; // Necessary to prevent captured variable bug.
                action(capturedItem, i);
            }

            return items;
        }

        /// <summary>
        /// Performs the given action for each item in the array.
        /// </summary>
        /// <typeparam name="T">Type of item being iterated on.</typeparam>
        /// <param name="items">Array of items being operated on.</param>
        /// <param name="action">The action to be performed, which may also use the index variable.</param>
        /// <returns>The original sequence.</returns>
        public static T[] Each<T>(this T[] items, Action<T, int> action)
        {

            for (int i = 0; i < items.Length; i++)
            {
                T capturedItem = items[i]; // Necessary to prevent captured variable bug.
                action(capturedItem, i);
            }

            return items;
        }

        /// <summary>
        /// Returns each combination of item in the original sequence and item in the selected sequence as a flat set.
        /// </summary>
        /// <param name="items">The items for which to select </param>
        /// <param name="sequenceSelector">A function that selects a sequence based on an item and its index in the sequence.</param>
        public static IEnumerable<TResult> SelectManyIndexed<TItem, TResult>(this IEnumerable<TItem> items, Func<TItem, int, IEnumerable<TResult>> sequenceSelector)
        {
            Func<Tuple<TItem, int>, IEnumerable<TResult>> wrappedSequenceSelector = x => sequenceSelector(x.Item1, x.Item2);

            var ret = items
                .Select((x, i) => Tuple.Create(x, i))
                .SelectMany(wrappedSequenceSelector);

            return ret;
        }

        /// <summary>
        /// Returns each combination of item in the original sequence and item in the selected sequence as a flat set.
        /// </summary>
        /// <param name="items">The items for which to select </param>
        /// <param name="sequenceSelector">A function that selects a sequence based on an item and its index in the sequence.</param>
        /// <param name="resultSelector">A function that selects a result item from an item, an item in the selected sequence, and the item's index.</param>
        public static IEnumerable<TResult> SelectManyIndexed<TItem, TSequence, TResult>(this IEnumerable<TItem> items, Func<TItem, int, IEnumerable<TSequence>> sequenceSelector,
            Func<TItem, TSequence, int, TResult> resultSelector)
        {
            Func<Tuple<TItem, int>, IEnumerable<TSequence>> wrappedSequenceSelector = x => sequenceSelector(x.Item1, x.Item2);
            Func<Tuple<TItem, int>, TSequence, TResult> wrappedResultSelector = (itemIndexTuple, sequenceItem) =>
                resultSelector(itemIndexTuple.Item1, sequenceItem, itemIndexTuple.Item2);


            var ret = items
                .Select((x, i) => Tuple.Create(x, i))
                .SelectMany(wrappedSequenceSelector, wrappedResultSelector);

            return ret;
        }

        #region Each By Type
        /// <summary>
        /// Performs actions on each element of the sequence based on their type.
        /// Elements matched by types specified first will NOT also have matching actions performed
        /// for types specified later.
        /// An action for a given type may be declared null if no action should be taken for that type.
        /// </summary>
        /// <typeparam name="TItem">The type of the sequence.</typeparam>
        /// <typeparam name="T1">The type for which the 1st action will be performed.</typeparam>
        /// <typeparam name="T2">The type for which the 2nd action will be performed.</typeparam>
        /// <typeparam name="T3">The type for which the 3rd action will be performed.</typeparam>
        /// <typeparam name="T4">The type for which the 4th action will be performed.</typeparam>
        /// <typeparam name="T5">The type for which the 5th action will be performed.</typeparam>
        /// <typeparam name="T6">The type for which the 6th action will be performed.</typeparam>
        /// <typeparam name="T7">The type for which the 7th action will be performed.</typeparam>
        /// <typeparam name="T8">The type for which the 8th action will be performed.</typeparam>
        /// <param name="items">The sequence of items</param>
        /// <param name="defaultAction">The action to be performed if the element does not match any of the specific types given or is null.</param>
        /// <param name="action1">The action that will be performed for the 1st specific type given.</param>
        /// <param name="action2">The action that will be performed for the 2nd specific type given.</param>
        /// <param name="action3">The action that will be performed for the 3rd specific type given.</param>
        /// <param name="action4">The action that will be performed for the 4th specific type given.</param>
        /// <param name="action5">The action that will be performed for the 5th specific type given.</param>
        /// <param name="action6">The action that will be performed for the 6th specific type given.</param>
        /// <param name="action7">The action that will be performed for the 7th specific type given.</param>
        /// <param name="action8">The action that will be performed for the 8th specific type given.</param>
        /// <returns>The base sequence of items</returns>
        public static IEnumerable<TItem> EachByType<TItem, T1, T2, T3, T4, T5, T6, T7, T8>(this IEnumerable<TItem> items,
            Action<TItem> defaultAction,
            Action<T1> action1,
            Action<T2> action2,
            Action<T3> action3,
            Action<T4> action4,
            Action<T5> action5,
            Action<T6> action6,
            Action<T7> action7,
            Action<T8> action8)
            where T1 : TItem
            where T2 : TItem
            where T3 : TItem
            where T4 : TItem
            where T5 : TItem
            where T6 : TItem
            where T7 : TItem
            where T8 : TItem
        {
            foreach (TItem item in items)
            {
                if (item == null)
                {
                    if (defaultAction != null) defaultAction(item);
                    continue;
                }

                if (item is T1)
                {
                    if (action1 != null) action1((T1)item);
                }
                else if (item is T2)
                {
                    if (action2 != null) action2((T2)item);
                }
                else if (item is T3)
                {
                    if (action3 != null) action3((T3)item);
                }
                else if (item is T4)
                {
                    if (action4 != null) action4((T4)item);
                }
                else if (item is T5)
                {
                    if (action5 != null) action5((T5)item);
                }
                else if (item is T6)
                {
                    if (action6 != null) action6((T6)item);
                }
                else if (item is T7)
                {
                    if (action7 != null) action7((T7)item);
                }
                else if (item is T8)
                {
                    if (action8 != null) action8((T8)item);
                }
                else
                {
                    if (defaultAction != null) defaultAction(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Performs actions on each element of the sequence based on their type.
        /// Elements matched by types specified first will NOT also have matching actions performed
        /// for types specified later.
        /// An action for a given type may be declared null if no action should be taken for that type.
        /// </summary>
        /// <typeparam name="TItem">The type of the sequence.</typeparam>
        /// <typeparam name="T1">The type for which the 1st action will be performed.</typeparam>
        /// <typeparam name="T2">The type for which the 2nd action will be performed.</typeparam>
        /// <typeparam name="T3">The type for which the 3rd action will be performed.</typeparam>
        /// <typeparam name="T4">The type for which the 4th action will be performed.</typeparam>
        /// <typeparam name="T5">The type for which the 5th action will be performed.</typeparam>
        /// <typeparam name="T6">The type for which the 6th action will be performed.</typeparam>
        /// <typeparam name="T7">The type for which the 7th action will be performed.</typeparam>
        /// <param name="items">The sequence of items</param>
        /// <param name="defaultAction">The action to be performed if the element does not match any of the specific types given or is null.</param>
        /// <param name="action1">The action that will be performed for the 1st specific type given.</param>
        /// <param name="action2">The action that will be performed for the 2nd specific type given.</param>
        /// <param name="action3">The action that will be performed for the 3rd specific type given.</param>
        /// <param name="action4">The action that will be performed for the 4th specific type given.</param>
        /// <param name="action5">The action that will be performed for the 5th specific type given.</param>
        /// <param name="action6">The action that will be performed for the 6th specific type given.</param>
        /// <param name="action7">The action that will be performed for the 7th specific type given.</param>
        /// <returns>The base sequence of items</returns>
        public static IEnumerable<TItem> EachByType<TItem, T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<TItem> items,
            Action<TItem> defaultAction,
            Action<T1> action1,
            Action<T2> action2,
            Action<T3> action3,
            Action<T4> action4,
            Action<T5> action5,
            Action<T6> action6,
            Action<T7> action7)
            where T1 : TItem
            where T2 : TItem
            where T3 : TItem
            where T4 : TItem
            where T5 : TItem
            where T6 : TItem
            where T7 : TItem
        {
            foreach (TItem item in items)
            {
                if (item == null)
                {
                    if (defaultAction != null) defaultAction(item);
                    continue;
                }

                if (item is T1)
                {
                    if (action1 != null) action1((T1)item);
                }
                else if (item is T2)
                {
                    if (action2 != null) action2((T2)item);
                }
                else if (item is T3)
                {
                    if (action3 != null) action3((T3)item);
                }
                else if (item is T4)
                {
                    if (action4 != null) action4((T4)item);
                }
                else if (item is T5)
                {
                    if (action5 != null) action5((T5)item);
                }
                else if (item is T6)
                {
                    if (action6 != null) action6((T6)item);
                }
                else if (item is T7)
                {
                    if (action7 != null) action7((T7)item);
                }
                else
                {
                    if (defaultAction != null) defaultAction(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Performs actions on each element of the sequence based on their type.
        /// Elements matched by types specified first will NOT also have matching actions performed
        /// for types specified later.
        /// An action for a given type may be declared null if no action should be taken for that type.
        /// </summary>
        /// <typeparam name="TItem">The type of the sequence.</typeparam>
        /// <typeparam name="T1">The type for which the 1st action will be performed.</typeparam>
        /// <typeparam name="T2">The type for which the 2nd action will be performed.</typeparam>
        /// <typeparam name="T3">The type for which the 3rd action will be performed.</typeparam>
        /// <typeparam name="T4">The type for which the 4th action will be performed.</typeparam>
        /// <typeparam name="T5">The type for which the 5th action will be performed.</typeparam>
        /// <typeparam name="T6">The type for which the 6th action will be performed.</typeparam>
        /// <param name="items">The sequence of items</param>
        /// <param name="defaultAction">The action to be performed if the element does not match any of the specific types given or is null.</param>
        /// <param name="action1">The action that will be performed for the 1st specific type given.</param>
        /// <param name="action2">The action that will be performed for the 2nd specific type given.</param>
        /// <param name="action3">The action that will be performed for the 3rd specific type given.</param>
        /// <param name="action4">The action that will be performed for the 4th specific type given.</param>
        /// <param name="action5">The action that will be performed for the 5th specific type given.</param>
        /// <param name="action6">The action that will be performed for the 6th specific type given.</param>
        /// <returns>The base sequence of items</returns>
        public static IEnumerable<TItem> EachByType<TItem, T1, T2, T3, T4, T5, T6>(this IEnumerable<TItem> items,
            Action<TItem> defaultAction,
            Action<T1> action1,
            Action<T2> action2,
            Action<T3> action3,
            Action<T4> action4,
            Action<T5> action5,
            Action<T6> action6)
            where T1 : TItem
            where T2 : TItem
            where T3 : TItem
            where T4 : TItem
            where T5 : TItem
            where T6 : TItem
        {
            foreach (TItem item in items)
            {
                if (item == null)
                {
                    if (defaultAction != null) defaultAction(item);
                    continue;
                }

                if (item is T1)
                {
                    if (action1 != null) action1((T1)item);
                }
                else if (item is T2)
                {
                    if (action2 != null) action2((T2)item);
                }
                else if (item is T3)
                {
                    if (action3 != null) action3((T3)item);
                }
                else if (item is T4)
                {
                    if (action4 != null) action4((T4)item);
                }
                else if (item is T5)
                {
                    if (action5 != null) action5((T5)item);
                }
                else if (item is T6)
                {
                    if (action6 != null) action6((T6)item);
                }
                else
                {
                    if (defaultAction != null) defaultAction(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Performs actions on each element of the sequence based on their type.
        /// Elements matched by types specified first will NOT also have matching actions performed
        /// for types specified later.
        /// An action for a given type may be declared null if no action should be taken for that type.
        /// </summary>
        /// <typeparam name="TItem">The type of the sequence.</typeparam>
        /// <typeparam name="T1">The type for which the 1st action will be performed.</typeparam>
        /// <typeparam name="T2">The type for which the 2nd action will be performed.</typeparam>
        /// <typeparam name="T3">The type for which the 3rd action will be performed.</typeparam>
        /// <typeparam name="T4">The type for which the 4th action will be performed.</typeparam>
        /// <typeparam name="T5">The type for which the 5th action will be performed.</typeparam>
        /// <param name="items">The sequence of items</param>
        /// <param name="defaultAction">The action to be performed if the element does not match any of the specific types given or is null.</param>
        /// <param name="action1">The action that will be performed for the 1st specific type given.</param>
        /// <param name="action2">The action that will be performed for the 2nd specific type given.</param>
        /// <param name="action3">The action that will be performed for the 3rd specific type given.</param>
        /// <param name="action4">The action that will be performed for the 4th specific type given.</param>
        /// <param name="action5">The action that will be performed for the 5th specific type given.</param>
        /// <returns>The base sequence of items</returns>
        public static IEnumerable<TItem> EachByType<TItem, T1, T2, T3, T4, T5>(this IEnumerable<TItem> items,
            Action<TItem> defaultAction,
            Action<T1> action1,
            Action<T2> action2,
            Action<T3> action3,
            Action<T4> action4,
            Action<T5> action5)
            where T1 : TItem
            where T2 : TItem
            where T3 : TItem
            where T4 : TItem
            where T5 : TItem
        {
            foreach (TItem item in items)
            {
                if (item == null)
                {
                    if (defaultAction != null) defaultAction(item);
                    continue;
                }

                if (item is T1)
                {
                    if (action1 != null) action1((T1)item);
                }
                else if (item is T2)
                {
                    if (action2 != null) action2((T2)item);
                }
                else if (item is T3)
                {
                    if (action3 != null) action3((T3)item);
                }
                else if (item is T4)
                {
                    if (action4 != null) action4((T4)item);
                }
                else if (item is T5)
                {
                    if (action5 != null) action5((T5)item);
                }
                else
                {
                    if (defaultAction != null) defaultAction(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Performs actions on each element of the sequence based on their type.
        /// Elements matched by types specified first will NOT also have matching actions performed
        /// for types specified later.
        /// An action for a given type may be declared null if no action should be taken for that type.
        /// </summary>
        /// <typeparam name="TItem">The type of the sequence.</typeparam>
        /// <typeparam name="T1">The type for which the 1st action will be performed.</typeparam>
        /// <typeparam name="T2">The type for which the 2nd action will be performed.</typeparam>
        /// <typeparam name="T3">The type for which the 3rd action will be performed.</typeparam>
        /// <typeparam name="T4">The type for which the 4th action will be performed.</typeparam>
        /// <param name="items">The sequence of items</param>
        /// <param name="defaultAction">The action to be performed if the element does not match any of the specific types given or is null.</param>
        /// <param name="action1">The action that will be performed for the 1st specific type given.</param>
        /// <param name="action2">The action that will be performed for the 2nd specific type given.</param>
        /// <param name="action3">The action that will be performed for the 3rd specific type given.</param>
        /// <param name="action4">The action that will be performed for the 4th specific type given.</param>
        /// <returns>The base sequence of items</returns>
        public static IEnumerable<TItem> EachByType<TItem, T1, T2, T3, T4>(this IEnumerable<TItem> items,
            Action<TItem> defaultAction,
            Action<T1> action1,
            Action<T2> action2,
            Action<T3> action3,
            Action<T4> action4)
            where T1 : TItem
            where T2 : TItem
            where T3 : TItem
            where T4 : TItem
        {
            foreach (TItem item in items)
            {
                if (item == null)
                {
                    if (defaultAction != null) defaultAction(item);
                    continue;
                }

                if (item is T1)
                {
                    if (action1 != null) action1((T1)item);
                }
                else if (item is T2)
                {
                    if (action2 != null) action2((T2)item);
                }
                else if (item is T3)
                {
                    if (action3 != null) action3((T3)item);
                }
                else if (item is T4)
                {
                    if (action4 != null) action4((T4)item);
                }
                else
                {
                    if (defaultAction != null) defaultAction(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Performs actions on each element of the sequence based on their type.
        /// Elements matched by types specified first will NOT also have matching actions performed
        /// for types specified later.
        /// An action for a given type may be declared null if no action should be taken for that type.
        /// </summary>
        /// <typeparam name="TItem">The type of the sequence.</typeparam>
        /// <typeparam name="T1">The type for which the 1st action will be performed.</typeparam>
        /// <typeparam name="T2">The type for which the 2nd action will be performed.</typeparam>
        /// <typeparam name="T3">The type for which the 3rd action will be performed.</typeparam>
        /// <param name="items">The sequence of items</param>
        /// <param name="defaultAction">The action to be performed if the element does not match any of the specific types given or is null.</param>
        /// <param name="action1">The action that will be performed for the 1st specific type given.</param>
        /// <param name="action2">The action that will be performed for the 2nd specific type given.</param>
        /// <param name="action3">The action that will be performed for the 3rd specific type given.</param>
        /// <returns>The base sequence of items</returns>
        public static IEnumerable<TItem> EachByType<TItem, T1, T2, T3>(this IEnumerable<TItem> items,
            Action<TItem> defaultAction,
            Action<T1> action1,
            Action<T2> action2,
            Action<T3> action3)
            where T1 : TItem
            where T2 : TItem
            where T3 : TItem
        {
            foreach (TItem item in items)
            {
                if (item == null)
                {
                    if (defaultAction != null) defaultAction(item);
                    continue;
                }

                if (item is T1)
                {
                    if (action1 != null) action1((T1)item);
                }
                else if (item is T2)
                {
                    if (action2 != null) action2((T2)item);
                }
                else if (item is T3)
                {
                    if (action3 != null) action3((T3)item);
                }
                else
                {
                    if (defaultAction != null) defaultAction(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Performs actions on each element of the sequence based on their type.
        /// Elements matched by types specified first will NOT also have matching actions performed
        /// for types specified later.
        /// An action for a given type may be declared null if no action should be taken for that type.
        /// </summary>
        /// <typeparam name="TItem">The type of the sequence.</typeparam>
        /// <typeparam name="T1">The type for which the 1st action will be performed.</typeparam>
        /// <typeparam name="T2">The type for which the 2nd action will be performed.</typeparam>
        /// <param name="items">The sequence of items</param>
        /// <param name="defaultAction">The action to be performed if the element does not match any of the specific types given or is null.</param>
        /// <param name="action1">The action that will be performed for the 1st specific type given.</param>
        /// <param name="action2">The action that will be performed for the 2nd specific type given.</param>
        /// <returns>The base sequence of items</returns>
        public static IEnumerable<TItem> EachByType<TItem, T1, T2>(this IEnumerable<TItem> items,
            Action<TItem> defaultAction,
            Action<T1> action1,
            Action<T2> action2)
            where T1 : TItem
            where T2 : TItem
        {
            foreach (TItem item in items)
            {
                if (item == null)
                {
                    if (defaultAction != null) defaultAction(item);
                    continue;
                }

                if (item is T1)
                {
                    if (action1 != null) action1((T1)item);
                }
                else if (item is T2)
                {
                    if (action2 != null) action2((T2)item);
                }
                else
                {
                    if (defaultAction != null) defaultAction(item);
                }
            }

            return items;
        }

        /// <summary>
        /// Performs actions on each element of the sequence based on their type.
        /// Elements matched by types specified first will NOT also have matching actions performed
        /// for types specified later.
        /// An action for a given type may be declared null if no action should be taken for that type.
        /// </summary>
        /// <typeparam name="TItem">The type of the sequence.</typeparam>
        /// <typeparam name="T1">The type for which the 1st action will be performed.</typeparam>
        /// <param name="items">The sequence of items</param>
        /// <param name="defaultAction">The action to be performed if the element does not match any of the specific types given or is null.</param>
        /// <param name="action1">The action that will be performed for the 1st specific type given.</param>
        /// <returns>The base sequence of items</returns>
        public static IEnumerable<TItem> EachByType<TItem, T1>(this IEnumerable<TItem> items,
            Action<TItem> defaultAction,
            Action<T1> action1)
            where T1 : TItem
        {
            foreach (TItem item in items)
            {
                if (item == null)
                {
                    if (defaultAction != null) defaultAction(item);
                    continue;
                }

                if (item is T1)
                {
                    if (action1 != null) action1((T1)item);
                }
                else
                {
                    if (defaultAction != null) defaultAction(item);
                }
            }

            return items;
        }
        #endregion

        #region Lambda equality comparer versions of standard LINQ Methods
  


      

        /// <summary>
        /// Returns the elements of the first sequence that are not present in the second.
        /// </summary>
        /// <typeparam name="TFirst">The type of elements in the first sequence.</typeparam>
        /// <typeparam name="TSecond">The type of elements in the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key used for comparing elements.</typeparam>
        /// <param name="first">An IEnumerable&lt;TFirst&gt; whose elements that are not
        /// also in second will be returned.</param>
        /// <param name="second">An IEnumerable&lt;TSecond&gt; whose elements that also occur
        /// in the first sequence will cause those elements to be removed from the returned sequence.</param>
        /// <param name="firstKeySelector">A funcion selecting a comparison object for each item in the first sequence.</param>
        /// <param name="secondKeySelector">A funcion selecting a comparison object for each item in the second sequence.</param>
        /// <returns>A sequence of elements </returns>
        public static IEnumerable<TFirst> ExceptBy<TFirst, TSecond, TKey>(this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TKey> firstKeySelector,
            Func<TSecond, TKey> secondKeySelector)
        {
            if (firstKeySelector == null) { throw new ArgumentNullException("sourceKeySelector", "The source key selection function cannot be null."); }
            if (secondKeySelector == null) { throw new ArgumentNullException("otherKeySelector", "The other key selection function cannot be null."); }

            var itemInSecond = second
                .Select(x => secondKeySelector(x))
                .ToHashset();

            return first
                .Where(x => !itemInSecond.Contains(firstKeySelector(x)));
        }

        /// <summary>
        /// Returns the elements of the first sequence that are not present in the second.
        /// </summary>
        /// <typeparam name="TFirst">The type of elements in the first sequence.</typeparam>
        /// <typeparam name="TSecond">The type of elements in the second sequence.</typeparam>
        /// <typeparam name="TKey">The type of the key used for comparing elements.</typeparam>
        /// <param name="first">An IEnumerable&lt;TFirst&gt; whose elements that are not
        /// also in second will be returned.</param>
        /// <param name="second">An IEnumerable&lt;TSecond&gt; whose elements that also occur
        /// in the first sequence will cause those elements to be removed from the returned sequence.</param>
        /// <param name="firstKeySelector">A funcion selecting a comparison object for each item in the first sequence.</param>
        /// <param name="secondKeySelector">A funcion selecting a comparison object for each item in the second sequence.</param>
        /// <param name="equalityComparer">An equality comparer that will be used to compare the keys</param>
        /// <returns>A sequence of elements </returns>
        public static IEnumerable<TFirst> ExceptBy<TFirst, TSecond, TKey>(this IEnumerable<TFirst> first,
            IEnumerable<TSecond> second,
            Func<TFirst, TKey> firstKeySelector,
            Func<TSecond, TKey> secondKeySelector,
            IEqualityComparer<TKey> equalityComparer)
        {
            if (firstKeySelector == null) throw new ArgumentNullException("sourceKeySelector", "The source key selection function cannot be null.");
            if (secondKeySelector == null) throw new ArgumentNullException("otherKeySelector", "The other key selection function cannot be null.");
            if (equalityComparer == null) throw new ArgumentNullException("equalityComparer", "The equality comparer cannot be null");

            var itemInSecond = second
                .Select(x => secondKeySelector(x))
                .ToHashset();

            return first
                .Where(x => !itemInSecond.Contains(firstKeySelector(x), equalityComparer));
        }


       

        /// <summary>
        /// Filters a sequence of items by those contained in another sequence.
        /// Equality is checked using the key generated by the key selector function.
        /// </summary>
        /// <typeparam name="TItem">The type of item that will be filtered.</typeparam>
        /// <typeparam name="TKey">The type of the key used for equality comparison.</typeparam>
        /// <param name="first">The items to filter.</param>
        /// <param name="second">The items that will be checked to see if the first item is contained in.</param>
        /// <param name="keySelector">A function selecting the equality comparison key from the items.</param>
        /// <returns>A sequence containing the elements in the first sequence that are contained in the second sequence.</returns>
        public static IEnumerable<TItem> ContainedIn<TItem, TKey>(this IEnumerable<TItem> first, IEnumerable<TItem> second, Func<TItem, TKey> keySelector)
        {
            if (keySelector == null) throw new ArgumentNullException("keySelector", "The key selection function can not be null.");

            var secondKeys = new HashSet<TKey>();
            foreach (var item in second)
            {
                secondKeys.AddIfMissing(keySelector(item));
            }

            var targetItems = first
                .Where(x => secondKeys.Contains(keySelector(x)));

            return targetItems;
        }

        /// <summary>
        /// Filters a sequence of items by those contained in another sequence.
        /// The types of the two item sequences do not need to be the same, as equality is checked using the key generated by the key selector functions.
        /// </summary>
        /// <typeparam name="TFirst">The type of item that will be filtered.</typeparam>
        /// <typeparam name="TSecond">The type of item that will be filtered against.</typeparam>
        /// <typeparam name="TKey">The type of the key used for equality comparison.</typeparam>
        /// <param name="first">The items to filter.</param>
        /// <param name="second">The items that will be checked to see if the first item is contained in.</param>
        /// <param name="firstKeySelector">A function selecting the equality comparison key from the items in the first sequence.</param>
        /// <param name="secondKeySelector">A function selecting the equality comparison key from the items in the second sequence.</param>
        /// <returns>A sequence containing the elements in the first sequence that are contained in the second sequence.</returns>
        public static IEnumerable<TFirst> ContainedIn<TFirst, TSecond, TKey>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TKey> firstKeySelector, Func<TSecond, TKey> secondKeySelector)
        {
            if (firstKeySelector == null) throw new ArgumentNullException("firstKeySelector", "The first key selection function can not be null.");
            if (secondKeySelector == null) throw new ArgumentNullException("secondKeySelector", "The second key selection function can not be null.");

            var secondKeys = new HashSet<TKey>();
            foreach (var item in second)
            {
                secondKeys.AddIfMissing(secondKeySelector(item));
            }

            var targetItems = first
                .Where(x => secondKeys.Contains(firstKeySelector(x)));

            return targetItems;
        }

        /// <summary>
        /// Filters a sequence of items by those contained in another sequence.
        /// The types of the two item sequences do not need to be the same, as equality is checked using the key generated by the key selector functions.
        /// </summary>
        /// <typeparam name="TFirst">The type of item that will be filtered.</typeparam>
        /// <typeparam name="TSecond">The type of item that will be filtered against.</typeparam>
        /// <typeparam name="TKey">The type of the key used for equality comparison.</typeparam>
        /// <param name="first">The items to filter.</param>
        /// <param name="second">The items that will be checked to see if the first item is contained in.</param>
        /// <param name="firstKeySelector">A function selecting the equality comparison key from the items in the first sequence.</param>
        /// <param name="secondKeySelector">A function selecting the equality comparison key from the items in the second sequence.</param>
        /// <param name="keyComparer">A comparer used to determine key equality.</param>
        /// <returns>A sequence containing the elements in the first sequence that are contained in the second sequence.</returns>
        public static IEnumerable<TFirst> ContainedIn<TFirst, TSecond, TKey>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, Func<TFirst, TKey> firstKeySelector, Func<TSecond, TKey> secondKeySelector, IEqualityComparer<TKey> keyComparer)
        {
            if (firstKeySelector == null) throw new ArgumentNullException("firstKeySelector", "The first key selection function can not be null.");
            if (secondKeySelector == null) throw new ArgumentNullException("secondKeySelector", "The second key selection function can not be null.");

            var secondKeys = new HashSet<TKey>(keyComparer);
            foreach (var item in second)
            {
                secondKeys.AddIfMissing(secondKeySelector(item));
            }

            var targetItems = first
                .Where(x => secondKeys.Contains(firstKeySelector(x)));

            return targetItems;
        }

      
      

        

        /// <summary>
        /// Returns the items that are contained in a collection more than once.
        /// </summary>
        /// <typeparam name="T">The data type of the items</typeparam>
        /// <param name="items">The collection of items</param>
        /// <param name="itemComparer">[Optional] Comparer used to determine the equality of two items.
        ///                        If not specified or null, the default comparer for the data type is used.</param>
        /// <returns>A collection containing those items that appear more than once in the input collection.
        public static IEnumerable<T> Duplicates<T>(this IEnumerable<T> items, IEqualityComparer<T> itemComparer = null)
        {
            HashSet<T> hashSet = new HashSet<T>(itemComparer);

            return items.Where(item => !hashSet.Add(item));
        }

        /// <summary>
        /// Groups a collection of items by their dependencies on other items
        /// (i.e. for each item in the resulting collection of item groups, all
        /// the dependencies of that item are contained one of the preceeding groups).
        /// </summary>
        /// <typeparam name="T">The data type of the items</typeparam>
        /// <param name="items">The items to be grouped</param>
        /// <param name="getDependencies">A function that, given an item, returns the collection of 
        ///                               items on which the given item depends, all of which must
        ///                               also be contained in "items".</param>
        /// <param name="comparer">[Optional] Comparer used to determine the equality of two items.
        ///                        If not specified or null, the default comparer for the data type is used.</param>
        /// <returns>A list of item groups, ordered from least (no dependencies) to most dependent</returns>
        public static List<List<T>> GroupByDependencies<T>(
                                                        this IEnumerable<T> items,
                                                        Func<T, IEnumerable<T>> getDependencies,
                                                        IEqualityComparer<T> comparer = null)
        {
            var groups = new List<List<T>>();
            var visited = new Dictionary<T, int>(comparer);

            foreach (T item in items)
            {
                Visit(item, getDependencies, groups, visited);
            }

            return groups;
        }

        private static int Visit<T>(T item,
                                    Func<T, IEnumerable<T>> getDependencies,
                                    List<List<T>> groups,
                                    IDictionary<T, int> visited)
        {
            const int InProgress = -1;

            int level;

            // Have we already visited this item?
            if (visited.TryGetValue(item, out level))
            {
                // Yes. Check for circular dependency.
                if (level == InProgress)
                {
                    throw new ArgumentException("Circular dependency found.");
                }
            }
            else
            {
                // No. Visit each node in the dependencies' subtrees
                visited[item] = level = InProgress;

                // Get the item's dependencies
                IEnumerable<T> dependencies = getDependencies(item);

                if (dependencies != null)
                {
                    // Get their max depth
                    foreach (T dependency in dependencies)
                    {
                        level = Math.Max(level, Visit(dependency, getDependencies, groups, visited));
                    }
                }

                // This item's depth is one greater
                visited[item] = ++level;

                // Add enough lists to accomodate this level
                while (groups.Count <= level)
                {
                    groups.Add(new List<T>());
                }

                // Add this item to its level's group
                groups[level].Add(item);
            }

            return level;
        }

        #region MoreLinq
        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection.
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return source.MaxBy(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Returns the maximal element of the given sequence, based on
        /// the given projection and the specified comparer for projected values. 
        /// </summary>
        /// <remarks>
        /// If more than one element has the maximal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current maximal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The maximal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/> 
        /// or <paramref name="comparer"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        public static TSource MaxBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            if (comparer == null) throw new ArgumentNullException("comparer");
            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var max = sourceIterator.Current;
                var maxKey = selector(max);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, maxKey) > 0)
                    {
                        max = candidate;
                        maxKey = candidateProjected;
                    }
                }
                return max;
            }
        }

        /// <summary>
        /// Returns the minimal element of the given sequence, based on
        /// the given projection. If the sequence is empty, then it will throw an exception.
        /// </summary>
        /// <remarks>
        /// If more than one element has the minimal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current minimal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <returns>The minimal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector)
        {
            return source.MinBy(selector, Comparer<TKey>.Default);
        }

        /// <summary>
        /// Returns the minimal element of the given sequence, based on
        /// the given projection and the specified comparer for projected values. If the sequence is empty, then it will throw an exception.
        /// </summary>
        /// <remarks>
        /// If more than one element has the minimal projected value, the first
        /// one encountered will be returned. This overload uses the default comparer
        /// for the projected type. This operator uses immediate execution, but
        /// only buffers a single result (the current minimal element).
        /// </remarks>
        /// <typeparam name="TSource">Type of the source sequence</typeparam>
        /// <typeparam name="TKey">Type of the projected element</typeparam>
        /// <param name="source">Source sequence</param>
        /// <param name="selector">Selector to use to pick the results to compare</param>
        /// <param name="comparer">Comparer to use to compare projected values</param>
        /// <returns>The minimal element, according to the projection.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="selector"/> 
        /// or <paramref name="comparer"/> is null</exception>
        /// <exception cref="InvalidOperationException"><paramref name="source"/> is empty</exception>
        public static TSource MinBy<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> selector, IComparer<TKey> comparer)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (selector == null) throw new ArgumentNullException("selector");
            if (comparer == null) throw new ArgumentNullException("comparer");
            using (var sourceIterator = source.GetEnumerator())
            {
                if (!sourceIterator.MoveNext())
                {
                    throw new InvalidOperationException("Sequence contains no elements");
                }
                var min = sourceIterator.Current;
                var minKey = selector(min);
                while (sourceIterator.MoveNext())
                {
                    var candidate = sourceIterator.Current;
                    var candidateProjected = selector(candidate);
                    if (comparer.Compare(candidateProjected, minKey) < 0)
                    {
                        min = candidate;
                        minKey = candidateProjected;
                    }
                }
                return min;
            }
        }

        /// <summary>
        /// Executes the given action on each element in the source sequence
        /// and yields it.
        /// </summary>
        /// <remarks>
        /// The returned sequence is essentially a duplicate of
        /// the original, but with the extra action being executed while the
        /// sequence is evaluated. The action is always taken before the element
        /// is yielded, so any changes made by the action will be visible in the
        /// returned sequence. This operator uses deferred execution and streams it results.
        /// </remarks>
        /// <typeparam name="T">The type of the elements in the sequence</typeparam>
        /// <param name="source">The sequence of elements</param>
        /// <param name="action">The action to execute on each element</param>
        public static IEnumerable<T> Pipe<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");
            return PipeImpl(source, action);
        }

        private static IEnumerable<T> PipeImpl<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var element in source)
            {
                action(element);
                yield return element;
            }
        }

        /// <summary>
        /// Asserts that all elements of a sequence meet a given condition
        /// otherwise throws an <see cref="Exception"/> object.
        /// </summary>
        /// <typeparam name="TSource">Type of elements in <paramref name="source"/> sequence.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <param name="predicate">Function that asserts an element of the <paramref name="source"/> sequence for a condition.</param>
        /// <returns>
        /// Returns the original sequence.
        /// </returns>
        /// <exception cref="InvalidOperationException">The input sequence
        /// contains an element that does not meet the condition being 
        /// asserted.</exception>
        /// <remarks>
        /// This operator uses deferred execution and streams its results.
        /// </remarks>
        public static IEnumerable<TSource> Assert<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
        {
            return Assert(source, predicate, null);
        }

        /// <summary>
        /// Asserts that all elements of a sequence meet a given condition
        /// otherwise throws an <see cref="Exception"/> object.
        /// </summary>
        /// <typeparam name="TSource">Type of elements in <paramref name="source"/> sequence.</typeparam>
        /// <param name="source">Source sequence.</param>
        /// <param name="predicate">Function that asserts an element of the input sequence for a condition.</param>
        /// <param name="errorSelector">Function that returns the <see cref="Exception"/> object to throw.</param>
        /// <returns>
        /// Returns the original sequence.
        /// </returns>
        /// <remarks>
        /// This operator uses deferred execution and streams its results.
        /// </remarks>
        public static IEnumerable<TSource> Assert<TSource>(this IEnumerable<TSource> source,
            Func<TSource, bool> predicate, Func<TSource, Exception> errorSelector)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (predicate == null) throw new ArgumentNullException("predicate");

            return AssertImpl(source, predicate, errorSelector ?? delegate { return null; });
        }

        private static IEnumerable<TSource> AssertImpl<TSource>(IEnumerable<TSource> source,
            Func<TSource, bool> predicate, Func<TSource, Exception> errorSelector)
        {
            foreach (var element in source)
            {
                var success = predicate(element);
                if (!success)
                    throw errorSelector(element) ?? new InvalidOperationException("Sequence contains an invalid item.");
                yield return element;
            }
        }
        #endregion

        #endregion

        /// <summary>
        /// Returns an ObservableCollection of items
        /// </summary>
        /// <typeparam name="T">The data type of the items</typeparam>
        /// <param name="items">The items to include in the ObservableCollection</param>
        /// <returns>The ObservableCollection containing the items</returns>
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> items)
        {
            return items == null ? null : new ObservableCollection<T>(items);
        }

        /// <summary>
        /// Returns a hash set of items.
        /// </summary>
        /// <param name="items">The items to include in the set. Must be unique.</param>
        public static HashSet<T> ToHashset<T>(this IEnumerable<T> items)
        {
            HashSet<T> retval = new HashSet<T>(items);
            return retval;
        }

        /// <summary>
        /// Returns a hash set of items.
        /// </summary>
        /// <param name="items">The items to include in the set. Must be unique.</param>
        /// <param name="comparer">An equality comparer that will be used to determine whether an item is contained in the set.</param>
        public static HashSet<T> ToHashset<T>(this IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            HashSet<T> retval = new HashSet<T>(items, comparer);
            return retval;
        }

        /// <summary>
        /// Returns a hash set of items and ensures uniqueness.
        /// </summary>
        /// <param name="items">The items to include in the set. Must be unique.</param>
        public static HashSet<T> ToDistinctHashSet<T>(this IEnumerable<T> items)
        {

            HashSet<T> retval = new HashSet<T>();
            foreach (var item in items)
            {
                retval.Add(item);
            }
            return retval;
        }

        /// <summary>
        /// Returns a hash set of items and ensures uniqueness.
        /// </summary>
        /// <param name="items">The items to include in the set. Must be unique.</param>
        /// <param name="comparer">An equality comparer that will be used to determine whether an item is contained in the set.</param>
        public static HashSet<T> ToDistinctHashSet<T>(this IEnumerable<T> items, IEqualityComparer<T> comparer)
        {
            HashSet<T> retval = new HashSet<T>(comparer);
            foreach (var item in items)
            {
                if (!retval.Contains(item)) retval.Add(item);
            }
            return retval;
        }

        /// <summary>
        /// Returns the elements of the specified sequence or, if the sequence is empty, a collection
        /// containing a single item specified by the default item selector.
        /// </summary>
        /// <typeparam name="T">The type of the elements of source.</typeparam>
        /// <param name="source">The type of the elements of source.</param>
        /// <param name="defaultItemSelector">A function returning the default item.</param>
        /// <returns>An IEnumerable&lt;T&gt; that contains defaultValue if source is empty; otherwise, source.</returns>
        /// <exception cref="System.ArgumentNullException" ></exception>
        public static IEnumerable<T> DefaultIfEmptyExt<T>(this IEnumerable<T> source, Func<T> defaultItemSelector)
        {
            if (defaultItemSelector == null) { throw new ArgumentNullException("defaultItemSelector", "The default item selection function cannot be null."); }
            T defaultItem = defaultItemSelector();
            return source.DefaultIfEmpty(defaultItem);
        }

        /// <summary>
        /// Randomly return an element from an IEnumerable. If the IEnumerable is empty, then return a default value of the type T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="randomNumberGenerator"></param>
        /// <returns></returns>
        public static T RandomElementOrDefault<T>(this IEnumerable<T> source, Random randomNumberGenerator)
        {
            T current = default(T);
            int count = 0;
            foreach (T element in source)
            {
                count++;
                if (randomNumberGenerator.Next(count) == 0)
                {
                    current = element;
                }
            }

            return current;
        }

        /// <summary>
        /// Takes An IEnumerable &gt;T&lt; and returns a Dictionary&gt;K,V&lt; based on the passed keySelector and valueSelector
        /// This will, however allow duplicate keys. It simply overrides the previous values with the new values
        /// </summary>
        /// <typeparam name="TSource">The Type of the Enumerable to send to Dictionary</typeparam>
        /// <typeparam name="K">The type of the Key to return</typeparam>
        /// <typeparam name="V">The Type of the Value to return</typeparam>
        /// <param name="source">The Enumerable to send ToDictionary</param>
        /// <param name="keySelector">The Func to select the keys</param>
        /// <param name="valueSelector">The Func to select the values</param>
        /// <returns>A Dictionary&gt;K,V&lt; that is created from <paramref name="source"/></returns>
        /// <exception cref="System.ArgumentNullException"/>
        public static Dictionary<K, V> ToOverridingDictionary<TSource, K, V>(this IEnumerable<TSource> source, Func<TSource, K> keySelector, Func<TSource, V> valueSelector)
        {
            if (source == null) { throw new ArgumentNullException("source", "Expected parameter source for ToOverridingDictionary"); }
            if (keySelector == null) { throw new ArgumentNullException("keySelector", "Expected parameter keySelector for ToOverridingDictionary"); }
            if (valueSelector == null) { throw new ArgumentNullException("valueSelector", "Expected parameter valueSelector for ToOverridingDictionary"); }

            Dictionary<K, V> output = new Dictionary<K, V>();
            foreach (TSource item in source)
            {
                output[keySelector(item)] = valueSelector(item);
            }

            return output;
        }

        /// <summary>
        /// Takes An IEnumerable &gt;T&lt; and returns a Dictionary&gt;K,V&lt; based on the passed keySelector and valueSelector
        /// This will, however allow duplicate keys. It simply ignores the new values upon key collision
        /// </summary>
        /// <typeparam name="TSource">The Type of the Enumerable to send to Dictionary</typeparam>
        /// <typeparam name="K">The type of the Key to return</typeparam>
        /// <typeparam name="V">The Type of the Value to return</typeparam>
        /// <param name="source">The Enumerable to send ToDictionary</param>
        /// <param name="keySelector">The Func to select the keys</param>
        /// <param name="valueSelector">The Func to select the values</param>
        /// <returns>A Dictionary&gt;K,V&lt; that is created from <paramref name="source"/></returns>
        /// <exception cref="System.ArgumentNullException"/>
        public static Dictionary<K, V> ToFirstDictionary<TSource, K, V>(this IEnumerable<TSource> source, Func<TSource, K> keySelector, Func<TSource, V> valueSelector)
        {
            return source.ToFirstDictionary(keySelector, valueSelector, null);
        }

        /// <summary>
        /// Takes An IEnumerable &gt;T&lt; and returns a Dictionary&gt;K,V&lt; based on the passed keySelector and valueSelector
        /// This will, however allow duplicate keys. It simply ignores the new values upon key collision
        /// </summary>
        /// <typeparam name="TSource">The Type of the Enumerable to send to Dictionary</typeparam>
        /// <typeparam name="K">The type of the Key to return</typeparam>
        /// <typeparam name="V">The Type of the Value to return</typeparam>
        /// <param name="source">The Enumerable to send ToDictionary</param>
        /// <param name="keySelector">The Func to select the keys</param>
        /// <param name="valueSelector">The Func to select the values</param>
        /// <param name="keyComparer">A comparer used to determine key equality for the dictionary and for key uniqueness.</param>
        /// <returns>A Dictionary&gt;K,V&lt; that is created from <paramref name="source"/></returns>
        /// <exception cref="System.ArgumentNullException"/>
        public static Dictionary<K, V> ToFirstDictionary<TSource, K, V>(this IEnumerable<TSource> source, Func<TSource, K> keySelector, Func<TSource, V> valueSelector,
            IEqualityComparer<K> keyComparer)
        {
            if (source == null) { throw new ArgumentNullException("source", "Expected parameter source for ToFirstDictionary"); }
            if (keySelector == null) { throw new ArgumentNullException("keySelector", "Expected parameter keySelector for ToFirstDictionary"); }
            if (valueSelector == null) { throw new ArgumentNullException("valueSelector", "Expected parameter valueSelector for ToFirstDictionary"); }

            Dictionary<K, V> output = null;
            if (keyComparer != null)
            {
                output = new Dictionary<K, V>(keyComparer);
            }
            else
            {
                output = new Dictionary<K, V>();
            }
            foreach (TSource item in source)
            {
                K key = keySelector(item);
                if (key == null) continue;
                if (!output.ContainsKey(key))
                {
                    output.Add(key, valueSelector(item));
                }
            }

            return output;
        }

        /// <summary>
        /// Adds a single new item to the end of a sequence of items.
        /// </summary>
        /// <typeparam name="T">The type of item in the sequence.</typeparam>
        /// <param name="items">The original sequence of items.</param>
        /// <param name="newItem">The item to be added.</param>
        /// <returns>The combined sequence of items.</returns>
        public static IEnumerable<T> ConcatSingle<T>(this IEnumerable<T> items, T newItem)
        {
            foreach (var item in items)
            {
                yield return item;
            }
            yield return newItem;
        }

        /// <summary>
        /// Adds a single new item to the front of a sequence of items.
        /// </summary>
        /// <typeparam name="T">The type of item in the sequence.</typeparam>
        /// <param name="items">The original sequence of items.</param>
        /// <param name="newItem">The item to be added.</param>
        /// <returns>The combined sequence of items.</returns>
        public static IEnumerable<T> PrependSingle<T>(this IEnumerable<T> items, T newItem)
        {
            yield return newItem;
            foreach (var item in items)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Adds a sequence of items to the front of an existing sequence of items.
        /// </summary>
        /// <typeparam name="T">The type of item in the sequence.</typeparam>
        /// <param name="items">The original sequence of items.</param>
        /// <param name="newItems">The new sequence to be added.</param>
        /// <returns>The combined sequence of items.</returns>
        public static IEnumerable<T> PrependSingle<T>(this IEnumerable<T> items, IEnumerable<T> newItems)
        {
            foreach (var item in newItems)
            {
                yield return item;
            }
            foreach (var item in items)
            {
                yield return item;
            }
        }

        /// <summary>
        /// Adds the item if it is not already present in the hash set.
        /// </summary>
        /// <param name="items">The hash set to add to.</param>
        /// <param name="itemToAdd">The item to add.</param>
        public static void AddIfMissing<T>(this HashSet<T> items, T itemToAdd)
        {
            if (!items.Contains(itemToAdd))
            {
                items.Add(itemToAdd);
            }
        }

        /// <summary>
        /// Checks if an item is present in a hash set, and if so, removes it.
        /// </summary>
        /// <param name="items">The hash set to remove from.</param>
        /// <param name="itemToAdd">The item to remove.</param>
        public static void RemoveIfPresent<T>(this HashSet<T> items, T itemToRemove)
        {
            if (items.Contains(itemToRemove))
            {
                items.Remove(itemToRemove);
            }
        }

        /// <summary>
        /// Synchronizes the items in a collection to match another set of items.
        /// The goal of this method is to make as few changes as possible.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="currentItems">The collection being updated.</param>
        /// <param name="newItems">The new items.</param>
        public static void SynchronizeWith<T>(this IList<T> currentItems, IEnumerable<T> newItems)
            where T : class
        {
            // TODO implement sorting?
            // This will not properly handle the case where the new items set contains a reordering of the current items.

            var newItemsSet = newItems.ToHashset();

            currentItems
                .Where(x => !newItemsSet.Contains(x))
                .ToArray()
                .Each(x => currentItems.Remove(x));

            var existingItems = currentItems
                .ToHashset();

            newItems
                .Each((x, i) =>
                {
                    if (i == currentItems.Count)
                    {
                        currentItems.Add(x);
                    }
                    else if (!currentItems.Contains(x))
                    {
                        currentItems.Insert(i, x);
                    }
                    else if (currentItems[i] != x)
                    {
                        currentItems[i] = x;
                    }
                });
        }

        /// <summary>
        /// Returns elements of a seqence in an infinite repeating loop.
        /// </summary>
        /// <param name="items">The sequence of items to be repeated.</param>
        public static IEnumerable<T> Repeat<T>(this IEnumerable<T> items)
        {
            using (var en = items.GetEnumerator())
            {
                // If our sequence is empty, just return it
                if (!en.MoveNext()) yield break;

                while (true)
                {
                    yield return en.Current;
                    if (!en.MoveNext())
                    {
                        en.Reset();
                        en.MoveNext();
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the weighted average of the given property of the item.
        /// </summary>
        /// <param name="items">The items to average.</param>
        /// <param name="valueSelector">A function that gets the value to be averaged for each item.</param>
        /// <param name="weightSelector">A function that gets the weight for each item.</param>
        public static double WeightedAverage<T>(this IEnumerable<T> items, Func<T, double> valueSelector, Func<T, double> weightSelector)
        {
            double weightTotal = 0;
            double weightedValueTotal = 0;

            items.Each(x =>
            {
                var weight = weightSelector(x);
                var value = valueSelector(x);
                weightTotal += weight;
                weightedValueTotal += weight * value;
            });

            if (weightTotal == 0)
            {
                if (weightedValueTotal == 0)
                {
                    return 0d;
                }
                else
                {
                    return double.NaN;
                }
            }

            var weightedAverage = weightedValueTotal / weightTotal;
            return weightedAverage;
        }

        /// <summary>
        /// Calculates the weighted average of the given property of the item.
        /// If the weight or value for an item is null, it will be excluded from the average.
        /// </summary>
        /// <param name="items">The items to average.</param>
        /// <param name="valueSelector">A function that gets the value to be averaged for each item.</param>
        /// <param name="weightSelector">A function that gets the weight for each item.</param>
        public static double? WeightedAverageNullable<T>(this IEnumerable<T> items, Func<T, double?> valueSelector, Func<T, double?> weightSelector)
        {
            double? weightTotal = null;
            double? weightedValueTotal = null;

            items.Each(x =>
            {
                var weight = weightSelector(x);
                var value = valueSelector(x);
                if (weight.HasValue && value.HasValue)
                {
                    weightTotal = weightTotal.GetValueOrDefault() + weight.Value;
                    weightedValueTotal = weightedValueTotal.GetValueOrDefault() + (weight.Value * value.Value);
                }
            });

            if (weightTotal == null || weightedValueTotal == null) return null;

            if (weightTotal == 0)
            {
                if (weightedValueTotal == 0)
                {
                    return 0d;
                }
                else
                {
                    return double.NaN;
                }
            }

            var weightedAverage = weightedValueTotal / weightTotal;
            return weightedAverage;
        }


        public static TItem FindNearestUnder<TItem, TProperty>(this IList<TItem> items, Func<TItem, TProperty> propertyGetter, TProperty criteria)
            where TProperty : IComparable<TProperty>
        {
            int? index = items.BinaryFindNearestUnder(propertyGetter, criteria);

            if (!index.HasValue)
                return default(TItem);

            return items.ElementAt(index.Value);

        }

        /// <summary>
        /// Given a list of items sorted by a property, gets the index of the item whose property is equal to the criteria value.
        /// If no items exactly match, return the index of the item with the largest property value that is less than the criteria value.
        /// If the first item still has a sort value greater than the criteria, returns null.
        /// </summary>
        /// <param name="items">The list of items.</param>
        /// <param name="propertyGetter">A function which gets the sorting property from each item.</param>
        /// <param name="criteria">The sorting property value to look for.</param>
        public static int? BinaryFindNearestUnder<TItem, TProperty>(this IList<TItem> items, Func<TItem, TProperty> propertyGetter, TProperty criteria)
            where TProperty : IComparable<TProperty>
        {
            if (items.Count == 0) return null;

            int iMin = 0;
            int iMax = items.Count - 1;

            // Check lowest value
            var lowestValue = propertyGetter(items[iMin]);
            var lowestCompResult = lowestValue.CompareTo(criteria);
            if (lowestCompResult == 1) return null; // Smallest item is still smaller than criteria, so no match 
            if (iMin == iMax) return iMin; // This is our only item, so return it

            // Check highest value
            var highestValue = propertyGetter(items[iMax]);
            var highestCompResult = highestValue.CompareTo(criteria);
            if (highestCompResult <= 0) return iMax; // Largest item is smaller than the criterion so it's the best option
            if (iMax == iMin + 1) return iMin; // We only have two items, and this one is too big, so return the smaller one

            iMin++;
            bool hasIncrementedMinBounds = true;
            iMax--;
            bool hasIncrementedMaxBounds = true;

            // We now know that our target value is somewhere in our range

            while (true)
            {
                if (hasIncrementedMinBounds)
                {
                    var minValue = propertyGetter(items[iMin]);
                    var minCompResult = minValue.CompareTo(criteria);
                    if (minCompResult == 0) return iMin;
                    if (minCompResult == 1) return iMin - 1;

                    hasIncrementedMinBounds = false;
                }

                if (hasIncrementedMaxBounds)
                {
                    var maxValue = propertyGetter(items[iMax]);
                    var maxCompResult = maxValue.CompareTo(criteria);
                    if (maxCompResult <= 0) return iMax;

                    hasIncrementedMaxBounds = false;
                }

                if (iMin >= iMax) break; // Should never hit if the list is sorted

                int iMid = iMin + (iMax - iMin) / 2;
                var value = propertyGetter(items[iMid]);
                var compResult = value.CompareTo(criteria);
                if (compResult == -1)
                {
                    iMin = iMid + 1;
                    hasIncrementedMinBounds = true;

                }
                else if (compResult == 1)
                {
                    iMax = iMid - 1;
                    hasIncrementedMaxBounds = true;
                }
                else
                {
                    return iMid;
                }
            }

            return null;
        }

        public static TItem FindNearestOver<TItem, TProperty>(this IList<TItem> items, Func<TItem, TProperty> propertyGetter, TProperty criteria)
            where TProperty : IComparable<TProperty>
        {
            int? index = items.BinaryFindNearestOver(propertyGetter, criteria);

            if (!index.HasValue)
                return default(TItem);

            return items.ElementAt(index.Value);

        }

        /// <summary>
        /// Given a list of items sorted by a property, gets the index of the item whose property is equal to the criteria value.
        /// If no items exactly match, return the index of the item with the smallest property value that is greater than the criteria value.
        /// If the last item still has a sort value less than the criteria, return null.
        /// </summary>
        /// <param name="items">The list of items.</param>
        /// <param name="propertyGetter">A function which gets the sorting property from each item.</param>
        /// <param name="criteria">The sorting property value to look for.</param>
        public static int? BinaryFindNearestOver<TItem, TProperty>(this IList<TItem> items, Func<TItem, TProperty> propertyGetter, TProperty criteria)
            where TProperty : IComparable<TProperty>
        {
            if (items.Count == 0) return null;

            int iMin = 0;
            int iMax = items.Count - 1;

            // Check highest value
            var highestValue = propertyGetter(items[iMax]);
            var highestCompResult = highestValue.CompareTo(criteria);
            if (highestCompResult == -1) return null; // Largest item is still smaller than criteria
            if (iMax == iMin) return iMax; // This is our only item, so return it

            // Check lowest value
            var lowestValue = propertyGetter(items[iMin]);
            var lowestCompResult = lowestValue.CompareTo(criteria);
            if (lowestCompResult >= 0) return iMin; // Our smallest item is still larger than the criteria, so 
            if (iMin == iMax - 1) return iMax; // We only have two items, so return the larger one    

            iMin++;
            bool hasIncrementedMinBounds = true;
            iMax--;
            bool hasIncrementedMaxBounds = true;

            // We now know that our target value is somewhere in our range

            while (true)
            {
                if (hasIncrementedMinBounds)
                {
                    var minValue = propertyGetter(items[iMin]);
                    var minCompResult = minValue.CompareTo(criteria);
                    if (minCompResult >= 0) return iMin;

                    hasIncrementedMinBounds = false;
                }

                if (hasIncrementedMaxBounds)
                {
                    var maxValue = propertyGetter(items[iMax]);
                    var maxCompResult = maxValue.CompareTo(criteria);
                    if (maxCompResult == 0) return iMax;
                    if (maxCompResult == -1) return iMax + 1;

                    hasIncrementedMaxBounds = false;
                }

                if (iMin >= iMax) break; // Should never hit if the list is sorted

                int iMid = iMin + (iMax - iMin) / 2;
                var value = propertyGetter(items[iMid]);
                var compResult = value.CompareTo(criteria);
                if (compResult == -1)
                {
                    iMin = iMid + 1;
                    hasIncrementedMinBounds = true;

                }
                else if (compResult == 1)
                {
                    iMax = iMid - 1;
                    hasIncrementedMaxBounds = true;
                }
                else
                {
                    return iMid;
                }
            }

            return null;
        }

        /// <summary>
        /// Merges items next to each other in a sequence to create a new sequence if the sequential items meet the predicate condition.
        /// </summary>
        /// <param name="mergeFunction">A function that takes two items and returns a merged item.</param>
        /// <param name="predicate">A function that should return true if two consequtive items should be merged, false otherwise.</param>
        public static IEnumerable<T> MergeConsecutive<T>(this IEnumerable<T> items, Func<T, T, bool> predicate, Func<T, T, T> mergeFunction)
            where T : class
        {
            T currentItem = null;
            foreach (var nextItem in items)
            {
                if (currentItem == null)
                {
                    currentItem = nextItem;
                }
                else
                {
                    if (predicate(currentItem, nextItem))
                    {
                        currentItem = mergeFunction(currentItem, nextItem);
                    }
                    else
                    {
                        yield return currentItem;
                        currentItem = nextItem;
                    }
                }
            }

            if (currentItem != null)
            {
                yield return currentItem;
            }
        }

        /// <summary>
        /// Causes execution of the enumerable.
        /// </summary> 
        public static void Evaluate<T>(this IEnumerable<T> items)
        {
            using (var enumerator = items.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    // Do Nothing
                }
            }
        }


        /// <summary>
        /// Given a list of a source items, and a function to select a sequence of items for those sources, returns a sequence of all combinations of each item from each combination (Cartesian product).
        /// The combinations are returned as sequences of items, one from each source, in the order that the sources are given in.
        /// </summary>
        /// <param name="sources">The sequence of objects which act as sources for the items. Each combination will consist of one item from each of these sources.</param>
        /// <param name="itemSelector">A function for selecting a sequence of items from a source object.</param>
        public static IEnumerable<TItem[]> SelectAllCombinations<TSource, TItem>(this IEnumerable<TSource> sources, Func<TSource, IEnumerable<TItem>> itemSelector)
        {
            var sourceEnums = new List<IEnumerator<TItem>>();
            foreach (var collection in sources)
            {
                var enumerable = itemSelector(collection);
                var enu = enumerable.GetEnumerator();

                // If there isn't at least one combination, we can stop here
                if (!enu.MoveNext())
                {
                    yield break;
                }

                sourceEnums.Add(enu);
            }

            var numSources = sourceEnums.Count;

            if (numSources == 0) yield break;

            var sourcesItems = new List<TItem>[numSources];
            for (int i = 0; i < numSources; i++)
            {
                sourcesItems[i] = new List<TItem> { sourceEnums[i].Current };
            }

            var firstCombination = new TItem[numSources];
            for (int i = 0; i < numSources; i++)
            {
                firstCombination[i] = sourcesItems[i][0];
            }
            yield return firstCombination;

            var finishedSources = new bool[numSources];

            int sourceIndexToIncrement = 0;
            int numFinishedSources = 0;

            var currentItemIndexes = new int[numSources];

            while (true)
            {
                if (!finishedSources[sourceIndexToIncrement])
                {
                    var sourceEnum = sourceEnums[sourceIndexToIncrement];
                    if (sourceEnum.MoveNext())
                    {
                        sourcesItems[sourceIndexToIncrement].Add(sourceEnum.Current);

                        #region Inner Solve
                        //currentItemIndexes = new int[numSources]; // TEMP

                        int currentSourceIndex = 0;
                        if (sourceIndexToIncrement == 0)
                        {
                            currentItemIndexes[0] = sourcesItems[0].Count - 1;
                        }
                        else
                        {
                            currentItemIndexes[0] = 0;
                        }

                        while (true)
                        {
                            var currentSource = sourcesItems[currentSourceIndex];
                            var currentItemIndex = currentItemIndexes[currentSourceIndex];

                            if (currentSourceIndex == numSources - 1)
                            {
                                for (int i = currentItemIndex; i < currentSource.Count; i++)
                                {
                                    var combination = new TItem[numSources];

                                    combination[numSources - 1] = currentSource[i];

                                    for (int j = 0; j < currentItemIndexes.Length - 1; j++)
                                    {
                                        combination[j] = sourcesItems[j][currentItemIndexes[j]];
                                    }

                                    yield return combination;
                                }

                                if (currentSourceIndex == 0)
                                {
                                    break;
                                }

                                currentSourceIndex--;
                                currentItemIndexes[currentSourceIndex]++;
                            }
                            else
                            {
                                if (currentItemIndex >= currentSource.Count)
                                {
                                    if (currentSourceIndex == 0)
                                    {
                                        break;
                                    }

                                    currentSourceIndex--;
                                    currentItemIndexes[currentSourceIndex]++;
                                }
                                else
                                {
                                    currentSourceIndex++;
                                    if (currentSourceIndex == sourceIndexToIncrement)
                                    {
                                        currentItemIndexes[currentSourceIndex] = sourcesItems[currentSourceIndex].Count - 1;
                                    }
                                    else
                                    {
                                        currentItemIndexes[currentSourceIndex] = 0;
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        sourceEnum.Dispose();
                        numFinishedSources++;
                        if (numFinishedSources == numSources)
                        {
                            yield break;
                        }
                        finishedSources[sourceIndexToIncrement] = true;
                    }
                }

                if (sourceIndexToIncrement == numSources - 1)
                {
                    sourceIndexToIncrement = 0;
                }
                else
                {
                    sourceIndexToIncrement++;
                }
            }
        }

        public class CombinationNode<T> : IEnumerable<T>
        {
            public T Item { get; set; }
            public CombinationNode<T> PreviousNode { get; set; }

            public IEnumerator<T> GetEnumerator()
            {
                var currentNode = this;
                while (currentNode != null)
                {
                    yield return currentNode.Item;
                    currentNode = currentNode.PreviousNode;
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>
        /// Given a list of item sources, returns one item from each source in alternating order (skipping over sources that have no more items) until all items have been returned.
        /// </summary>
        /// <param name="sources">The sequence of item source objects.</param>
        /// <param name="itemSelector">A functions for selecting items from each source.</param>
        public static IEnumerable<TItem> SelectAlternating<TSource, TItem>(this IEnumerable<TSource> sources, Func<TSource, IEnumerable<TItem>> itemSelector)
        {
            var itemEnumerators = new List<IEnumerator<TItem>>();

            // Get item enumerators from each of the source.
            // We also return the first item from each source in this pass so that this function is fully streaming rather than buffering
            foreach (var source in sources)
            {
                var items = itemSelector(source);
                var itemsEn = items.GetEnumerator();
                if (itemsEn.MoveNext())
                {
                    yield return itemsEn.Current;
                    itemEnumerators.Add(itemsEn);
                }
            }

            if (itemEnumerators.Count == 0) yield break;

            // Rotate through each of our sources, getting items until we have a pass where all of them have no more
            int i = 0;
            int numSkipped = 0;
            while (numSkipped < itemEnumerators.Count)
            {
                var itemEnum = itemEnumerators[i];
                if (itemEnum.MoveNext())
                {
                    numSkipped = 0;
                    yield return itemEnum.Current;
                }
                else
                {
                    numSkipped++;
                }

                i++;
                i %= itemEnumerators.Count;
            }
        }

        public static IEnumerable<TResult> FullOuterJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer, IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
            where TInner : class
            where TOuter : class
        {
            var innerLookup = inner.ToLookup(innerKeySelector);
            var outerLookup = outer.ToLookup(outerKeySelector);

            var innerJoinItems = inner
                .Where(innerItem => !outerLookup.Contains(innerKeySelector(innerItem)))
                .Select(innerItem => resultSelector(null, innerItem));

            return outer
                .SelectMany(outerItem =>
                {
                    var innerItems = innerLookup[outerKeySelector(outerItem)];

                    return innerItems.Any() ? innerItems : new TInner[] { null };
                }, resultSelector)
                .Concat(innerJoinItems);
        }

        public static void ConvertToUtc(this DataRowCollection rows, TimeZoneInfo sourceTimezone)
        {
            if (rows.Count <= 0
                || sourceTimezone == null)
                return;

            List<string> dateTimeColumnNames = new List<string>();
            DataRow sampleRow = rows[0];
            DataTable table = sampleRow.Table;
            foreach (DataColumn column in table.Columns)
            {
                if (column.DataType.IsEquivalentTo(typeof(DateTime)))
                {
                    dateTimeColumnNames.Add(column.ColumnName);
                }
            }

            foreach (DataRow row in rows)
            {
                foreach (string columnName in dateTimeColumnNames)
                {
                    string dateTimeStr = row[columnName].ToString();
                    DateTime parsed;
                    if (DateTime.TryParse(dateTimeStr, out parsed))
                    {
                        DateTime convertedParsed = TimeZoneInfo.ConvertTimeToUtc(parsed, sourceTimezone);
                        row[columnName] = convertedParsed;
                    }
                }
            }


        }

        public static async Task<IList<TEntity>> ToListAsync<TEntity>(this IEnumerable<TEntity> items)
        {
            return await Task.Run(() => items.ToList());
        }
    }
}
