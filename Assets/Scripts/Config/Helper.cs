using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Helper
{

    public static T GetRandom<T>(T[] array) where T : class
    {
        int index = Random.Range(0, array.Length);
        return array[index];
    }

    public static T GetRandom<T>(IEnumerable<T> list) where T : new()
    {
        try
        {
            int index = Random.Range(0, list.Count());
            return list.ElementAt(index);
        }
        catch (System.Exception ex) 
        {
            return new T();
        }
    }

    //public static T GetRandomEnum<T>(IEnumerable<T> list) where T : System.Enum
    //{
    //    int index = Random.Range(0, list.Count());
    //    return list.ElementAt(index);
    //}
}
