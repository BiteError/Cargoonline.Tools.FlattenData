using System;
using System.Collections.Generic;
using System.Linq;

namespace Cargoonline.Tools.FlattenData
{
    public class TypeConstraintAttribute : Attribute
    {
        public Type ValueType { get; private set; }

        public IList<Type> ForEntitiesTypes { get; private set; }

        /// <summary>
        /// TypeConstraint constructor
        /// </summary>
        /// <param name="valueType">Type of stored value</param>
        /// <param name="forTypes">List of related types for setting. Empty, if setting related to all types</param>
        public TypeConstraintAttribute(Type valueType, params Type[] forTypes)
        {
            ValueType = valueType;
            ForEntitiesTypes = forTypes.ToList();
        }
    }

}