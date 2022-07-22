using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Extensions
{
    public enum SortDirection
    {
        ascending,
        descending
    }
    public enum SortType
    {
        bubble,
        heap,
        shell,
        merge
    }
    public struct SortOptions
    {
        public SortDirection direction;
        public string column;
    }
    public static class TypeExtensions
    {
        public static bool comparisons_loaded = false;
        public delegate int compare(object one, object two);
        /// <summary>
        /// All comparisons are ascending
        /// </summary>
        public static Dictionary<Type, compare> object_comparisons = new Dictionary<Type, compare>();

        public static void SortByColumn(this DataTable table, SortOptions[] options, SortType type)
        {
            List<DataRow> item_arrays = (from t in table.AsEnumerable()
                                         select t).ToList();

            if (item_arrays == null || item_arrays.Count() == 0)
                return;


            for (int i = options.Length - 1; i >= 0; i--)
            {
                SortOptions this_option = options[i];
                int column = table.Columns.IndexOf(this_option.column);

                if (String.IsNullOrWhiteSpace(this_option.column))
                    continue;

                switch (type)
                {
                    case SortType.bubble:
                        item_arrays.BubbleSort((object_one, object_two) => Comparison(object_one, object_two, column, this_option.direction));
                        break;
                    case SortType.shell:
                        item_arrays.ShellSort((object_one, object_two) => Comparison(object_one, object_two, column, this_option.direction));
                        break;
                    case SortType.heap:
                        item_arrays.HeapSort((object_one, object_two) => Comparison(object_one, object_two, column, this_option.direction));
                        break;
                    case SortType.merge:
                        item_arrays.MergeSort((object_one, object_two) => Comparison(object_one, object_two, column, this_option.direction));
                        break;
                    default:
                        break;
                }
            }
            List<object[]> objectArrays = new List<object[]>();
            foreach (DataRow r in item_arrays)
            {
                objectArrays.Add(r.ItemArray);
            }
            table.Rows.Clear();
            foreach (object[] obj_arr in objectArrays)
            {
                DataRow nr = table.NewRow();
                nr.ItemArray = obj_arr;
                table.Rows.Add(nr);
            }
        }

        //public static int ComparisonAscending<T>(this List<T> list, object[] one, object[] two, int index)
        //{
        //    return ObjectCompare(one[index], two[index]);
        //}
        //public static int ComparisonDescending<T>(this List<T> list, object[] one, object[] two, int index)
        //{
        //    return -ObjectCompare(one[index], two[index]);
        //}

        public static int Comparison(DataRow dr_one, DataRow dr_two, int index, SortDirection direction)
        {
            return Comparison(dr_one.ItemArray, dr_two.ItemArray, index, direction);
        }

        public static int Comparison(object[] one, object[] two, int index, SortDirection direction)
        {
            int i = 1;

            if (direction == SortDirection.descending)
                i = -1;

            return i * ObjectCompare(one[index], two[index]);
        }

        /// <summary>
        /// returns 1: one > two, 0: one ? two, -1: one < two
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns></returns>
        public static int ObjectCompare(object one, object two)
        {
            LoadObjectCompares();

            Type one_type = one.GetType();
            Type two_type = two.GetType();

            ///     if they're not of the same type, they cannot be compared
            if (one_type != two_type)
            {
                return 0;
            }

            ///     Check to see if the type is in the dictionary
            ///     

            if (object_comparisons.ContainsKey(one_type))
            {
                int x = object_comparisons[one_type](one, two);
                return x;
            }
            else
            {
                return 0;
            }
        }
        static void AddObjectCompare(Type t, compare c)
        {
            if (!object_comparisons.ContainsKey(t))
            {
                object_comparisons.Add(t, c);
            }
        }
        public static void LoadObjectCompares()
        {
            if (!comparisons_loaded)
            {
                object_comparisons.Add(typeof(double), delegate (object x, object y)
                {
                    double xt = (double)x; double yt = (double)y;
                    if (xt > yt)
                        return 1;
                    else if (xt == yt)
                        return 0;
                    else return -1;
                });
                object_comparisons.Add(typeof(int), delegate (object x, object y)
                {
                    int xt = (int)x; int yt = (int)y;
                    if (xt > yt) return 1;
                    else if (xt == yt) return 0;
                    else return -1;
                });
                object_comparisons.Add(typeof(string), delegate (object x, object y)
                {
                    string xt = (string)x; string yt = (string)y;
                    return xt.CompareTo(yt);
                });
                object_comparisons.Add(typeof(DateTime), delegate (object x, object y)
                {
                    DateTime xt = (DateTime)x; DateTime yt = (DateTime)y;
                    if (xt > yt)
                        return 1;
                    else if (xt == yt)
                        return 0;
                    else return -1;
                });

                comparisons_loaded = true;
            }
        }

        public static bool IsGenericList(this object o)
        {
            bool isGenericList = false;

            var oType = o.GetType();

            if (oType.IsGenericType && (oType.GetGenericTypeDefinition() == typeof(List<>)))
                isGenericList = true;

            return isGenericList;
        }

        public static void ConvertDateColumnsToUTC(this DataTable table)
        {
            List<DataColumn> dateColumns = new List<DataColumn>();
            foreach (DataColumn column in table.Columns)
            {
                if (column.DataType == typeof(DateTime))
                {
                    dateColumns.Add(column);
                }
            }

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn dateColumn in dateColumns)
                {
                    if (!row.IsNull(dateColumn))
                    {
                        row[dateColumn] = ((DateTime)row[dateColumn]).ToUniversalTime();
                    }
                }
            }
        }

        public static void ConvertDateColumnsToUTC(this DataTable table, TimeZoneInfo timeInfo)
        {
            List<DataColumn> dateColumns = new List<DataColumn>();
            foreach (DataColumn column in table.Columns)
            {
                if (column.DataType == typeof(DateTime))
                {
                    dateColumns.Add(column);
                }
            }

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn dateColumn in dateColumns)
                {
                    if (!row.IsNull(dateColumn))
                    {
                        row[dateColumn] = ((DateTime)row[dateColumn]).ToUTC(timeInfo);
                    }
                }
            }
        }
        public static void ConvertDateColumnsToUTC(this DataTable table, double utcOffsetHours)
        {
            List<DataColumn> dateColumns = new List<DataColumn>();
            foreach (DataColumn column in table.Columns)
            {
                if (column.DataType == typeof(DateTime))
                {
                    dateColumns.Add(column);
                }
            }

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn dateColumn in dateColumns)
                {
                    if (!row.IsNull(dateColumn))
                    {
                        row[dateColumn] = ((DateTime)row[dateColumn]).AddHours(-utcOffsetHours);
                    }
                }
            }
        }
        public static void ConvertDateColumnsToLocalTime(this DataTable table)
        {
            List<DataColumn> dateColumns = new List<DataColumn>();
            foreach (DataColumn column in table.Columns)
            {
                if (column.DataType == typeof(DateTime))
                {
                    dateColumns.Add(column);
                }
            }

            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn dateColumn in dateColumns)
                {
                    if (!row.IsNull(dateColumn))
                    {
                        row[dateColumn] = ((DateTime)row[dateColumn]).ToLocalTime();
                    }
                }
            }
        }

        public static int IndexOfExcludeDeletedRows(this DataRowCollection argDataRowCollection, DataRow argDataRow)
        {
            int i = 0;
            foreach (DataRow row in argDataRowCollection)
            {
                if (row.RowState != DataRowState.Deleted && row.RowState != DataRowState.Detached)
                {
                    if (Object.ReferenceEquals(row, argDataRow))
                    {
                        return i;
                    }
                    i++;
                }
            }
            return -1;
        }

        public static string ToNonNullString(this object o)
        {
            if (o.IsNull())
                return string.Empty;
            return o.ToString();
        }

        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }

        public static T GetEnumValueFromDescription<T>(this string description)
        {
            var type = typeof(T);
            if (!type.IsEnum)
                throw new ArgumentException();
            FieldInfo[] fields = type.GetFields();
            var field = fields
                            .SelectMany(f => f.GetCustomAttributes(
                                typeof(DescriptionAttribute), false), (
                                    f, a) => new { Field = f, Att = a })
                            .Where(a => ((DescriptionAttribute)a.Att)
                                .Description == description).SingleOrDefault();
            return field == null ? default(T) : (T)field.Field.GetRawConstantValue();
        }
    }
}
