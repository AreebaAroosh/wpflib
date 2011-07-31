using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Data.Objects;
using System.Data.EntityClient;
using System.Data;

namespace WPFLib.Updates
{
	public class ParameterValue
	{
		public ParameterValue()
		{
		}

		public ParameterValue(object value, DbType type)
		{
			Value = value;
			Type = type;
		}

		public object Value
		{
			get;
			set;
		}
		public DbType Type
		{
			get;
			set;
		}
	}

    public class SqlFileUpdater
    {
        public SqlFileUpdater()
        {
        }

        public SqlFileUpdater(DbConnection connection)
        {
            Connection = connection;
        }

        public SqlFileUpdater(ObjectContext context)
        {
            ObjectContext = context;
        }

        ObjectContext _objectContext = null;

        protected ObjectContext ObjectContext
        {
            get
            {
                return _objectContext;
            }
            set
            {
                _objectContext = value;
                Connection = ((EntityConnection)value.Connection).StoreConnection;
            }
        }

		public void RunSqlReader(string sql, Action<DbDataReader> processor, IDictionary<string, object> parameters = null, bool closeConnection = true)
		{
			var d = OpenConnection();
			try
			{
				var command = Connection.CreateCommand();
				SetParameters(command, parameters);
				command.CommandText = sql;
				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						processor(reader);
					}
				}
			}
			finally
			{
				if (closeConnection)
					d.Dispose();
			}
		}

		public object RunSql(string sql, IDictionary<string, object> parameters = null, bool closeConnection = true)
		{
			var d = OpenConnection();
			try
			{
				var command = Connection.CreateCommand();
				SetParameters(command, parameters);
				command.CommandText = sql;
				return command.ExecuteScalar();
			}
			finally
			{
				if (closeConnection)
					d.Dispose();
			}
		}

    	private void SetParameters(DbCommand command, IDictionary<string, object> parameters)
    	{
    		if (parameters != null)
    		{
    			foreach (var pair in parameters)
    			{
    				var param = command.CreateParameter();
    				param.ParameterName = pair.Key;
    				if (pair.Value is ParameterValue)
    				{
    					param.Value = (pair.Value as ParameterValue).Value;
    					param.DbType = (pair.Value as ParameterValue).Type;
    				}
    				else
    				{
    					param.Value = pair.Value;
    				}
    				command.Parameters.Add(param);
    			}
    		}
    	}

    	protected void Run(string path, bool closeConnection = true, Assembly assembly = null)
        {
            if (assembly == null)
            {
                // С этим надо что-то сделать
                assembly = Assembly.GetCallingAssembly();
            }
            var sql = GetFileContent(path, assembly);
            var d = OpenConnection();
            try
            {
                ExecuteBatches(sql, Connection);
            }
            finally
            {
                if (closeConnection)
                    d.Dispose();
            }
        }

        protected string GetFileContent(string path, Assembly assembly = null)
        {
            var filePath = Path.Combine(BasePath, path);

            if (File.Exists(filePath))
            {
                var content = File.ReadAllText(filePath);
                return content;
            }
            else
            {
                if (assembly == null)
                {
                    // С этим надо что-то сделать
                    assembly = Assembly.GetCallingAssembly();
                }
                var res = assembly.GetManifestResourceNames();
                var resName = path.Replace('\\', '.');
                var found = res.Where(r => r.EndsWith(resName)).ToList();
                if (found.Count == 0)
                {
                    throw new Exception("No file or resource for path \"" + path + "\"");
                }
                else if (found.Count > 1)
                {
                    throw new Exception("More than one resource for path \"" + path + "\"");
                }
                using (var stream = assembly.GetManifestResourceStream(found[0]))
                {
                    using (var rdr = new StreamReader(stream))
                    {
                        var content = rdr.ReadToEnd();
                        return content;
                    }
                }
            }
        }

        private void ExecuteBatches(string full, DbConnection connection)
        {
            string[] commands = full.Split(new string[] { "GO\r\n", "GO ", "GO\t", "go\r\n", "go ", "go\t", "GO", "go"}, StringSplitOptions.RemoveEmptyEntries);
            foreach (string c in commands)
            {
                var command = connection.CreateCommand();
                command.CommandText = c;
                command.ExecuteNonQuery();
            }
        }

        string BasePath
        {
            get
            {
                var p = Process.GetCurrentProcess().MainModule.FileName;
                return Path.GetDirectoryName(p);
            }
        }

        protected virtual void OnBeforeOpenConnection()
        {
        }

        protected IDisposable OpenConnection()
        {
            if (Connection != null && Connection.State == System.Data.ConnectionState.Open)
                return Connection;
            OnBeforeOpenConnection();
            if (Connection == null)
                throw new Exception("No connection");
            Connection.Open();
            return Connection;
        }

        protected DbConnection Connection
        {
            get;
            private set;
        }
    }
}
