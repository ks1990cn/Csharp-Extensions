using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Extensions
{
    public static class Sorting
    {
        //The "this" keyword before the method parameter identifies this as a C# extension
        //method, which can be called using instance method syntax on any generic list,
        //without having to modify the generic List<T> code provided by the .NET framework.
        //In order to use this funcitonality we must be "using" this namespace

        #region Shell Sort
        public static void ShellSort<T>(this List<T> list) where T : IComparable
        {
            ShellSort(list, Comparer<T>.Default);
        }

        public static void ShellSort<T>(this List<T> list, IComparer<T> Comparer)
        {
            ShellSort(list, Comparer.Compare);
        }

        public static void ShellSort<T>(this List<T> list, Comparison<T> Comparison)
        {
            int n = list.Count;
            int[] incs = { 1, 3, 7, 21, 48, 112, 336, 861, 1968, 4592, 13776, 33936, 86961, 198768, 463792, 1391376, 3402672, 8382192, 21479367, 49095696, 114556624, 343669872, 52913488, 2085837936 };
            for (int l = incs.Length / incs[0]; l > 0;)
            {
                int m = incs[--l];
                for (int i = m; i < n; ++i)
                {
                    int j = i - m;
                    if (Comparison(list[i], list[j]) < 0)
                    {
                        T tempItem = list[i];
                        do
                        {
                            list[j + m] = list[j];
                            j -= m;
                        } while ((j >= 0) && (Comparison(tempItem, list[j]) < 0));
                        list[j + m] = tempItem;
                    }
                }
            }
        }
        #endregion

        public static void DataTableSort<T>(this DataTable table, Comparison<T> Comparison) where T : DataRow
        {
            //Okay so this is a kludge at best and wrong at worst, but if you sort on the pfw_pipe table but are passing a comparison of type pfw_tankRow in, this will
            //fail SILENTLY, so the best way to do this is try cast the first item in the table's row collection 

            //For speed's sake, lets check if the number of rows is < 2. If it is, there no need to do a sort
            if (table.Rows.Count < 2)
            {
                return;
            }

            //Get the zeroth row.
            T dr = table.Rows[0] as T;
            if (dr == null) { throw new InvalidOperationException("The passed type " + typeof(T) + " is not a type in the DataTable's collection"); }

            // we need to accept changes so we dont have to worry about preserving the row metadata, since we are
            // only swapping around the itemarrays.
            table.AcceptChanges();
            List<T> rows = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                rows.Add(row as T);
            }

            rows.MergeSort(Comparison);

            List<object[]> itemArrays = new List<object[]>();

            foreach (DataRow row in rows)
            {
                itemArrays.Add(row.ItemArray);
            }

            for (int i = 0; i < rows.Count; i++)
            {
                table.Rows[i].ItemArray = itemArrays[i];
            }
        }

        #region Heap Sort
        public static void HeapSort<T>(this List<T> list)
        {
            HeapSort<T>(list, 0, list.Count, Comparer<T>.Default);
        }

        public static void HeapSort<T>(this List<T> list, int offset, int length, IComparer<T> comparer)
        {
            HeapSort<T>(list, offset, length, comparer.Compare);
        }

        public static void HeapSort<T>(this List<T> list, Comparison<T> comparison)
        {
            HeapSort<T>(list, 0, list.Count, comparison);
        }

        public static void HeapSort<T>(this List<T> list, int offset, int length, Comparison<T> comparison)
        {
            // build binary heap from all items
            for (int i = 0; i < length; i++)
            {
                int index = i;
                T item = list[offset + i]; // use next item

                // and move it on top, if greater than parent
                while (index > 0 &&
                    comparison(list[offset + (index - 1) / 2], item) < 0)
                {
                    int top = (index - 1) / 2;
                    list[offset + index] = list[offset + top];
                    index = top;
                }
                list[offset + index] = item;
            }

            for (int i = length - 1; i > 0; i--)
            {
                // delete max and place it as last
                T last = list[offset + i];
                list[offset + i] = list[offset];

                int index = 0;
                // the last one positioned in the heap
                while (index * 2 + 1 < i)
                {
                    int left = index * 2 + 1, right = left + 1;

                    if (right < i && comparison(list[offset + left], list[offset + right]) < 0)
                    {
                        if (comparison(last, list[offset + right]) > 0) break;

                        list[offset + index] = list[offset + right];
                        index = right;
                    }
                    else
                    {
                        if (comparison(last, list[offset + left]) > 0) break;

                        list[offset + index] = list[offset + left];
                        index = left;
                    }
                }
                list[offset + index] = last;
            }
        }
        #endregion

        #region MergeSort

        public static void MergeSort<T>(this List<T> List)
        {
            MergeSort<T>(List, Comparer<T>.Default);
        }

        public static void MergeSort<T>(this List<T> List, IComparer<T> Comparer)
        {
            MergeSort<T>(List, Comparer.Compare);
        }

        public static void MergeSort<T>(this List<T> List, Comparison<T> Comparison)
        {
            if (List.Count <= 1) return;

            List<T> left = List.GetRange(0, List.Count / 2);
            List<T> right = List.GetRange(left.Count, List.Count - left.Count);
            left.MergeSort(Comparison);
            right.MergeSort(Comparison);

            //This has to be done as opposed to List = Merge(l,r,Comp); since we cannot modify list in that way by reassigning its reference;
            List<T> temp = Merge(left, right, Comparison);

            List.Clear();
            List.AddRange(temp);
        }

        private static List<T> Merge<T>(List<T> Left, List<T> Right, Comparison<T> Comparison)
        {
            List<T> result = new List<T>();
            while (Left.Count > 0 && Right.Count > 0)
            {
                if (Comparison(Left[0], Right[0]) <= 0)
                {
                    result.Add(Left[0]);
                    Left.RemoveAt(0);
                }
                else
                {
                    result.Add(Right[0]);
                    Right.RemoveAt(0);
                }
            }
            result.AddRange(Left);
            result.AddRange(Right);
            return result;
        }

        #endregion

        #region Bubble Sort
        public static void BubbleSort<T>(this List<T> list) where T : IComparable
        {
            BubbleSort(list, Comparer<T>.Default);
        }

        public static void BubbleSort<T>(this List<T> list, IComparer<T> Comparer)
        {
            BubbleSort(list, Comparer.Compare);
        }

        public static void BubbleSort<T>(this List<T> list, Comparison<T> Comparison)
        {
            bool madeChanges;
            int itemCount = list.Count;
            do
            {
                madeChanges = false;
                itemCount--;
                for (int i = 0; i < itemCount; i++)
                {
                    if (Comparison(list[i], list[i + 1]) > 0)
                    {
                        T temp = list[i + 1];
                        list[i + 1] = list[i];
                        list[i] = temp;
                        madeChanges = true;
                    }
                }
            } while (madeChanges);
        }
        #endregion
    }
}
