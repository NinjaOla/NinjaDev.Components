using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NinjaDev.Components.Blazor.Helpers
{
    internal static class TypeHelper
    {
        internal static Type CreateType(this Type type, params Type[] types)
        {
            return type.MakeGenericType(types); ;
        }


        /// <summary>
        /// https://stackoverflow.com/questions/24471338/activator-createinstance-calling-constructor-with-class-as-parameter/24471737#24471737 
        /// </summary>
        /// <param name="pContext"></param>
        /// <param name="Params"></param>
        /// <returns></returns>
        internal static object CreateInstance(this Type pContext, object[] Params)
        {
            List<Type> argTypes = new List<Type>();
            //used .GetType() method to get the appropriate type
            //Param can be null so handle accordingly
            if (Params != null)
                foreach (object Param in Params)
                {
                    if (Param != null)
                        argTypes.Add(Param.GetType());
                    else
                        argTypes.Add(null);
                }

            ConstructorInfo[] Types = pContext.GetConstructors();
            foreach (ConstructorInfo node in Types)
            {
                ParameterInfo[] Args = node.GetParameters();
                // Params can be null for default constructors so use argTypes
                if (argTypes.Count == Args.Length)
                {
                    bool areTypesCompatible = true;
                    for (int i = 0; i < Params.Length; i++)
                    {
                        if (argTypes[i] == null)
                        {
                            if (Args[i].ParameterType.IsValueType)
                            {
                                //fill the defaults for value type if not supplied
                                Params[i] = CreateInstance(Args[i].ParameterType, null);
                                argTypes[i] = Params[i].GetType();
                            }
                            else
                            {
                                argTypes[i] = Args[i].ParameterType;
                            }
                        }
                        if (!Args[i].ParameterType.IsAssignableFrom(argTypes[i]))
                        {
                            areTypesCompatible = false;
                            break;
                        }
                    }
                    if (areTypesCompatible)
                        return node.Invoke(Params);
                }
            }

            //delegate type to Activator.CreateInstance if unable to find a suitable constructor
            return Activator.CreateInstance(pContext);
        }
    }
}
