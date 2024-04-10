using System;
using System.Collections.Generic;

namespace HappyKit
{
    public static class String
    {
        public static EnumType Parse<EnumType>(string str)
        {
            return (EnumType)Enum.Parse(typeof(EnumType), str);
        }
        public static string ToString<T>(T[] array)
        {
            string result = "";
            foreach (T element in array)
                result += element + ", ";
            return result;
        }
        public static string ToString<T>(List<T> list)
        {
            string result = "";
            foreach (T element in list)
                result += element + ", ";
            return result;
        }
    }
}