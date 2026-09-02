using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;

namespace StackRadius.DBHelper
{
    public class DbParameterList
    {
        public List<DbParameter> List = new List<DbParameter>(10);
        public DbParameter Add(DbParameter parm)
        {
            List.Add(parm);
            return parm;
        }

        public DbParameter Add(string _name, DbType _dbType, int _size = -1, ParameterDirection _parameterDirection = ParameterDirection.Input)
        {
            DbParameter parm = new DbParameter(_name, _dbType, _size, _parameterDirection);
            List.Add(parm);
            return parm;
        }
    }

    public class DbParameter
    {

        #region "Parameterized Constructor"

        public DbParameter(string _name, object _value)
        {
            this.Name = _name;
            this.Value = _value;
            this.DBDirection = ParameterDirection.Input;
        }

        public DbParameter(string _name, object _value, DbType _dbType)
        {
            this.Name = _name;
            this.Value = _value;
            this.DBType = _dbType;
            this.NpgsqlDbType = null;
            this.DBDirection = ParameterDirection.Input;
        }

        public DbParameter(string _name, object _value, NpgsqlDbType _npgsqlDbType)
        {
            this.Name = _name;
            this.Value = _value; 
            this.DBType = null;
            this.NpgsqlDbType = _npgsqlDbType;
            this.DBDirection = ParameterDirection.Input;
        }

        public DbParameter(string _name, object _value, DbType _dbType, ParameterDirection _parameterDirection)
        {
            this.Name = _name;
            this.Value = _value;
            this.DBType = _dbType;
            this.NpgsqlDbType = null;
            this.DBDirection = _parameterDirection;
        }

        public DbParameter(string _name, object _value, DbType _dbType, int _size, ParameterDirection _parameterDirection = ParameterDirection.Input)
        {
            this.Name = _name;
            this.Value = _value;
            this.DBType = _dbType;
            this.NpgsqlDbType = null;
            this.Size = _size;
            this.DBDirection = _parameterDirection;
        }
        public DbParameter(string _name, DbType _dbType, int _size = -1, ParameterDirection _parameterDirection = ParameterDirection.Input)
        {
            this.Name = _name;
            this.DBType = _dbType;
            if (_size >= 0)
                this.Size = _size;
            this.DBDirection = _parameterDirection;
        }

        public DbParameter()
        {
            // TODO: Complete member initialization
        }

        #endregion

        #region "Property"

        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }
        private string m_Name;
        public object Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        private object m_Value;

        public System.Nullable<DbType> DBType
        {
            get { return m_DBType; }
            set { m_DBType = value; }
        }

        private System.Nullable<DbType> m_DBType;

        public System.Nullable<NpgsqlTypes.NpgsqlDbType> NpgsqlDbType
        {
            get { return m_NpgsqlDbType; }
            set { m_NpgsqlDbType = value; }
        }

        private System.Nullable<NpgsqlTypes.NpgsqlDbType> m_NpgsqlDbType;

        public ParameterDirection DBDirection
        {
            get { return m_DBDirection; }
            set { m_DBDirection = value; }
        }

        




        private ParameterDirection m_DBDirection;
        public int Size
        {
            get { return m_Size; }
            set { m_Size = value; }
        }
        public object ParameterName { get; set; }
        private int m_Size;
        #endregion

    }
}
