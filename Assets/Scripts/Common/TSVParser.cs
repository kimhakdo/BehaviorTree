using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Ironcow
{
    public class TSVParser
    {
        public static List<Dictionary<string, string>> TsvToDic(string data)
        {
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            var rows = data.Split('\n');
            var keys = rows[0].Trim().Split('\t');
            for (int i = 1; i < rows.Length; i++)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                var columns = rows[i].Trim().Split('\t');
                for (int j = 0; j < columns.Length; j++)
                {
                    dic.Add(keys[j], columns[j]);
                }
                list.Add(dic);
            }
            return list;
        }


        public static ScriptableObject DicToSOData(Type type, ScriptableObject dt, Dictionary<string, string> data)
        {
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var keys = new List<string>(data.Keys);
            foreach (var field in fields)
            {
                try
                {
                    var idx = keys.FindIndex(obj => obj == field.Name);
                    if (idx >= 0)
                    {
                        if (field.FieldType == typeof(int))
                        {
                            field.SetValue(dt, int.Parse(data[keys[idx]]));
                        }
                        else if (field.FieldType == typeof(float))
                        {
                            field.SetValue(dt, float.Parse(data[keys[idx]]));
                        }
                        else if (field.FieldType == typeof(bool))
                        {
                            field.SetValue(dt, bool.Parse(data[keys[idx]]));
                        }
                        else if (field.FieldType == typeof(double))
                        {
                            field.SetValue(dt, double.Parse(data[keys[idx]]));
                        }
                        else if (field.FieldType == typeof(string))
                        {
                            field.SetValue(dt, data[keys[idx]]);
                        }
                        else if (field.FieldType == typeof(List<int>))
                        {
                            var datas = data[keys[idx]].Replace(" ", "").Split(',');
                            List<int> list = new List<int>();
                            foreach (var str in datas)
                            {
                                if (int.TryParse(str, out var result))
                                {
                                    list.Add(result);
                                }
                            }
                            field.SetValue(dt, list);
                        }
                        else if (field.FieldType == typeof(int[]))
                        {
                            var datas = data[keys[idx]].Replace(" ", "").Split(',');
                            List<int> list = new List<int>();
                            foreach (var str in datas)
                            {
                                if (int.TryParse(str, out var result))
                                {
                                    list.Add(result);
                                }
                            }
                            field.SetValue(dt, list.ToArray());
                        }
                        else if (field.FieldType == typeof(List<string>))
                        {
                            field.SetValue(dt, data[keys[idx]].Replace(" ", "").Split(',').ToList());
                        }
                        else if (field.FieldType == typeof(string[]))
                        {
                            field.SetValue(dt, data[keys[idx]].Replace(" ", "").Split(','));
                        }
                        else if (field.FieldType == typeof(Vector3))
                        {
                            field.SetValue(dt, data[keys[idx]].ToVector3());
                        }
                        else if (field.FieldType.BaseType == typeof(Enum))
                        {
                            var enumData = Enum.Parse(field.FieldType, data[keys[idx]]);
                            field.SetValue(dt, enumData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log("Convert Failed");
                }
            }
            return dt;
        }
    }

}