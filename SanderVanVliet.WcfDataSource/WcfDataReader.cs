using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.ReportingServices.DataProcessing;

namespace SanderVanVliet.WcfDataSource
{
    public class WcfDataReader : IDataReader
    {
        private readonly PropertyInfo[] _fields;
        private readonly MethodInfo _operation;
        private readonly IDataParameterCollection _parameters;
        private readonly ServiceProxy _proxy;
        private int _resultIndex;
        private object[] _data;
        private bool _operationWasCalled;

        public WcfDataReader(ServiceProxy proxy, MethodInfo operation, IDataParameterCollection parameters)
        {
            _proxy = proxy;
            _operation = operation;
            _parameters = parameters;
            _fields = GetFieldsFromOperation();
        }

        private PropertyInfo[] GetFieldsFromOperation()
        {
            Type typeToGetFieldsFrom = null;

            if (_operation.ReturnType.IsArray)
            {
                typeToGetFieldsFrom = _operation.ReturnType.GetElementType();
            }
            else if (typeof (IEnumerable<>).IsAssignableFrom(_operation.ReturnType))
            {
                // It's a collection Jim, but not as we know it
                var genericTypeDefinition = _operation
                    .ReturnType
                    .GetGenericTypeDefinition();

                if (genericTypeDefinition != null)
                {
                    typeToGetFieldsFrom = genericTypeDefinition
                        .GetGenericArguments()
                        .SingleOrDefault();
                }
            }
            else
            {
                typeToGetFieldsFrom = _operation.ReturnType;
            }

            if (typeToGetFieldsFrom == null)
            {
                typeToGetFieldsFrom = typeof(object);
            }

            return typeToGetFieldsFrom.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        protected virtual object[] GetDataFromOperation()
        {
            var retval = _operation.Invoke(_proxy.Client, GetOperationParameters()) as object[];

            _operationWasCalled = true;

            return retval;
        }

        private object[] GetOperationParameters()
        {
            var parameters = _parameters.OfType<IDataParameter>();

            return _operation
                .GetParameters()
                .Select(p =>
                {
                    var isSet = parameters.SingleOrDefault(dparam => dparam.ParameterName == p.Name);
                    
                    return isSet != null ? isSet.Value : null;
                })
                .ToArray();
        }

        public void Dispose()
        {
        }

        public string GetName(int fieldIndex)
        {
            if (fieldIndex < 0 || fieldIndex >= _fields.Length)
            {
                throw new ArgumentOutOfRangeException("fieldIndex");
            }

            return _fields[fieldIndex].Name;
        }

        public int GetOrdinal(string fieldName)
        {
            for (var i = 0; i < _fields.Length; i++)
            {
                if (_fields[i].Name == fieldName)
                {
                    return i;
                }
            }

            throw new Exception("Field '" + fieldName + "' not found");
        }

        public bool Read()
        {
            if (_data == null && !_operationWasCalled)
            {
                _data = GetDataFromOperation();
            }

            if (_resultIndex >= _data.Length)
            {
                return false;
            }

            _resultIndex++;

            return true;
        }

        public Type GetFieldType(int fieldIndex)
        {
            if (fieldIndex < 0 || fieldIndex >= _fields.Length)
            {
                throw new ArgumentOutOfRangeException("fieldIndex");
            }

            return _fields[fieldIndex].PropertyType;
        }

        public object GetValue(int fieldIndex)
        {
            var rowData = _data[_resultIndex];

            return _fields[fieldIndex].GetGetMethod().Invoke(rowData, null);
        }

        public int FieldCount
        {
            get { return _fields.Length; }
        }
    }
}