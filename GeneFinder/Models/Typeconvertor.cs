using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GeneFinder.Models
{
    sealed class typeconvertor : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type returntype = null;
            if (assemblyName == "AnnotationApr10, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")
            {
                assemblyName = Assembly.GetExecutingAssembly().FullName;
                returntype =
                    Type.GetType(String.Format("{0}, {1}", typeName, assemblyName));
                return returntype;
            }
            if (typeName == "System.Collections.Generic.List`1[[smorf, AnnotationApr10, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null]]")
            {
                typeName =
                    typeName.Replace("AnnotationApr10, Version = 1.0.0.0, Culture = neutral, PublicKeyToken = null", Assembly.GetExecutingAssembly().FullName);
                returntype =
                    Type.GetType(String.Format("{0}, {1}",
                    typeName, assemblyName));
                return returntype;
            }
            return returntype;
        }
    }
}
