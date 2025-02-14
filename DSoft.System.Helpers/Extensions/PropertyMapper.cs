using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    /// <summary>
    /// Mapping extensions
    /// </summary>
    public static class PropertyMapper
    {
        /// <summary>
        /// Maps the instance to the a new instance of the target type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static T MapTo<T>(this object source) where T : new()
        {
            var target = new T();

            Map(source, target);

            return target;
        }

        /// <summary>
        /// Maps the instance to the a new instance of the target type with property exclusion list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="excludePropertiesNames"></param>
        /// <returns></returns>
        public static T MapTo<T>(this object source, params string[] excludePropertiesNames) where T : new()
        {
            var target = new T();

            Map(source, target, excludePropertiesNames);

            return target;
        }

        /// <summary>
        /// Maps the instance to the a new instance of the target type, with an inititor action to set properties explicitly
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="initiator">The initiator.</param>
        /// <returns></returns>
        public static T MapTo<T>(this object source, Action<T> initiator) where T : new()
        {
            var target = new T();

            Map(source, target);

            if (initiator != null)
            {
                initiator(target);
            }

            return target;
        }

        /// <summary>
        /// Maps the instance to the a new instance of the target type, with an inititor action to set properties explicitly and property exclusion list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="initiator">The initiator.</param>
        /// <param name="excludePropertiesNames">Names of properties to exclude</param>
        /// <returns></returns>
        public static T MapTo<T>(this object source, Action<T> initiator, params string[] excludePropertiesNames) where T : new()
        {
            var target = new T();

            Map(source, target, excludePropertiesNames);

            if (initiator != null)
            {
                initiator(target);
            }

            return target;
        }

        /// <summary>
        /// Maps the soure instance to the target instance
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        /// <param name="excludePropertiesNames">Names of properties to exclude</param>
        public static void MapTo<T>(this object source, T target, params string[] excludePropertiesNames) where T : new()
        {
            Map(source, target, excludePropertiesNames);
        }

        /// <summary>
        /// Maps the source instance list to a list of instance of the target type
        /// </summary>
        /// <typeparam name="T2">The target type</typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static List<T2> MapToList<T2>(this IEnumerable source) where T2 : new()
        {
            var results = new List<T2>();

            foreach (var aObj in source)
            {
                var newItem = new T2();

                Map(aObj, newItem);

                results.Add(newItem);
            }

            return results;
        }

        /// <summary>
        /// Maps the source instance list to a list of instance of the target type, with an inititor action to set properties explicitly
        /// </summary>
        /// <typeparam name="T">Source Type</typeparam>
        /// <typeparam name="T2">The target type</typeparam>
        /// <param name="source">The source instance</param>
        /// <param name="initiator">The initiator action</param>
        /// <returns></returns>
        public static List<T2> MapToList<T, T2>(this IEnumerable<T> source, Action<T, T2> initiator) where T2 : new()
        {
            var results = new List<T2>();

            foreach (var aObj in source)
            {
                var newItem = new T2();

                Map(aObj, newItem);

                if (initiator != null)
                {
                    initiator(aObj, newItem);
                }

                results.Add(newItem);
            }

            return results;
        }

        private static void Map(object source, object target, params string[] excludePropertiesNames)
        {
            //find all the properties in the source

            var sprops = source.GetType().GetRuntimeProperties();

            var tType = target.GetType();

            //var tprops = target.GetType().GetTypeInfo().DeclaredProperties;
            var exProps = new List<string>();

            if (excludePropertiesNames != null && excludePropertiesNames.Length > 0)
                exProps = excludePropertiesNames.ToList();


            foreach (var aProp in sprops)
            {
                if (exProps.Contains(aProp.Name, StringComparer.OrdinalIgnoreCase)) //see if property has been excluded
                    continue;

                var propGet = aProp.GetMethod;

                if (propGet != null && propGet.IsPublic)
                {
                    var tProp = tType.GetRuntimeProperty(aProp.Name);

                    // See if the target property is null
                    if (tProp != null)
                    {
                        //if not then see if they are the same property type
                        if (tProp.PropertyType.Equals(aProp.PropertyType))
                        {
                            //found the prop
                            var propSet = tProp.SetMethod;

                            //only set on public properties on the target side
                            if (propSet != null && propSet.IsPublic)
                            {
                                var sValue = aProp.GetValue(source);

                                tProp.SetValue(target, sValue);
                            }
                        }
                        else
                        {
                            try
                            {

                                //try setting if the types are different
                                var propSet = tProp.SetMethod;

                                //only set on public properties on the target side
                                if (propSet != null && propSet.IsPublic)
                                {
                                    var sValue = aProp.GetValue(source);

                                    tProp.SetValue(target, sValue);
                                }
                            }
                            catch (Exception)
                            {
                                //
                                Debug.WriteLine($"Cannot set property {tProp.Name} on class {target.GetType().Name} as types are different: {tProp.PropertyType.FullName} instead of {aProp.PropertyType.FullName}");

                            }

                        }
                    }

                }

            }

        }




    }
}

