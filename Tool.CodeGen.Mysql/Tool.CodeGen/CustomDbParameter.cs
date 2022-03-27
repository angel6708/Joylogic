using System;
using System.Collections.Generic;
using System.Linq;
using System.Text; 

using System.Data;
namespace DAL
{
    /// <summary>
    /// 自定义数据命令的参数的封装
    /// </summary>
    public class CustomDbParameter
    {
        private String _name;
        /// <summary>
        /// 参数名
        /// </summary>
        public String Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private Object _value;
        /// <summary>
        /// 参数值
        /// </summary>
        public Object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Int32 Length { get; set; }
        private CustomDbType _dataType;
        /// <summary>
        /// 参数类型
        /// </summary>
        public CustomDbType DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        private ParameterDirection _direction;
        /// <summary>
        /// 获取或设置参数的方向
        /// </summary>
        public ParameterDirection Direction
        {
            get { return _direction; }
            set { _direction = value; }
        }
        /// <summary>
        /// 用参数名称、参数值和参数类型创建DbParameter类的实例
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="dbType">参数类型</param>
        /// <param name="direction">参数方向</param>
        public CustomDbParameter(String name, Object value, CustomDbType dbType, ParameterDirection direction)
        {
            _name = name;
            _value = value;
            _dataType = dbType;
            _direction = direction;
        }
        /// <summary>
        /// 用参数名称、参数值和参数类型创建DbParameter类的实例
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        /// <param name="dbType">参数类型</param>
        public CustomDbParameter(String name, Object value, CustomDbType dbType)
            : this(name, value, dbType, ParameterDirection.Input)
        {
        }

        public override bool Equals(object obj)
        {
            CustomDbParameter p = obj as CustomDbParameter;

            if (ReferenceEquals(p, null))
            {
                return false;
            }
            if (ReferenceEquals(this, p)) { return true; }

            return this._name == p._name;
        }

        public override int GetHashCode()
        {
            return this._name.GetHashCode();
        }

        public override string ToString()
        {
            return String.Format("{0} | {1}", this._name, this._value.ToString());
        }
    }
}
