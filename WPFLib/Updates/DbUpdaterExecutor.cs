using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.EntityClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Transactions;
using System.Data.Objects;
using System.ComponentModel.Composition;
using WPFLib.Configuration;
using System.Diagnostics;
using System.ComponentModel.Composition.Primitives;

namespace WPFLib.Updates
{
    public interface IOrderNumber
    {
        int OrderNumber { get; }
    }

    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Method)]
    public class DbUpdaterMethodAttribute : ExportAttribute
    {
        public DbUpdaterMethodAttribute(int orderNumber) : base("DbUpdaterMethod")
        {
            OrderNumber = orderNumber;
        }

        public int OrderNumber { get; private set; }
    }

    interface IDbUpdater
    {
        void UpdateStart();
        int OrderNumber { get; }
    }

    public class DbUpdaterExecutor
    {
        private class DbUpdaterWrapper : IDbUpdater
        {
            private Action Action;

            public DbUpdaterWrapper(Action action, int order)
            {
                Action = action;
                OrderNumber = order;
            }

            public int OrderNumber { get; private set; }

            public void UpdateStart()
            {
                Action();
            }
        }

        private IEnumerable<IDbUpdater> GetUpdaters()
        {
            List<IDbUpdater> dbUpdaters = new List<IDbUpdater>();

            var updaterMethods = ServiceLocator.Current.GetExports<Action, IOrderNumber>("DbUpdaterMethod");

            foreach (var v in updaterMethods)
            {
                dbUpdaters.Add(new DbUpdaterWrapper(v.Value, v.Metadata.OrderNumber));
            }

            // Поддержка классов IDbUpdater скорее не нужна, это ни чем не отличается от метода с атрибутом
            // даже хуже - надо делать проперти с номером апдейта
            // IDbUpdater может быть полезен для инициализации апдейтера, и для его деинициализации
            // для того что бы он мог закрыть соединение вообще
            // А лучше вообще соединение расшарить между апдейтерами - оно у нас и так есть, надо только передать

            var badGroups = dbUpdaters.GroupBy(upd => upd.OrderNumber).Where(group => group.Count() > 1 && group.Key > 0);
            if (badGroups.Any())
            {
                string orders = string.Join(",", badGroups.Select(g => g.Key));
                string message = string.Format("Not unique db updaters order numbers : {0}", orders);
                throw new Exception(message);
            }
            return dbUpdaters;
        }


        public bool StartUpdates(ObjectContext context)
        {
            var connection = ((EntityConnection)context.Connection).StoreConnection;
            return StartUpdates(connection);
        }

        public bool StartUpdates(DbConnection connection)
        {
            try
            {
                int currentVersion = GetCurrentVersion(connection);
            	var updaters = GetUpdaters();
				var orderedUpdaters = updaters.Where(upd => upd.OrderNumber > currentVersion).OrderBy(upd => upd.OrderNumber).ToList();
				var everyTimeUpdaters = updaters.Where(item => item.OrderNumber <= 0).OrderByDescending(item => item.OrderNumber).ToList();

				var options = new TransactionOptions();
				options.IsolationLevel = System.Transactions.IsolationLevel.Serializable;
				options.Timeout = TimeSpan.FromMinutes(30);
                using (var tr = new TransactionScope(TransactionScopeOption.Required, options))
                {
                    foreach (var dbUpdater in orderedUpdaters)
                    {
                        dbUpdater.UpdateStart();
						if (currentVersion == 0)
							InsertVersionRow(connection, dbUpdater.OrderNumber);
						else
							UpdateCurrentVersion(connection, dbUpdater.OrderNumber);
						currentVersion = dbUpdater.OrderNumber;
                    }
					foreach (var dbUpdater in everyTimeUpdaters)
					{
						dbUpdater.UpdateStart();
					}
                    tr.Complete();
                }
				return true;
            }
            catch (Exception e)
            {
                this.ErrorMessage(e);
				return false;
            }
        }


    	private int GetCurrentVersion(DbConnection connection)
    	{
    		int currentVersion;
    		try
    		{
    			connection.Open();
    			CheckForTable(connection);
    			currentVersion = GetVersion(connection);
    		}
    		finally
    		{
    			if (connection.State == ConnectionState.Open)
    				connection.Close();
    		}
    		return currentVersion;
    	}

    	private void UpdateCurrentVersion(DbConnection connection, int orderNumber)
        {
            string query = "UPDATE VersionTable SET Version=@versionNum WHERE Version = (SELECT TOP 1 Version FROM VersionTable)";
             try
            {
                connection.Open();
            var command = CreateSingleParamed(connection, query, orderNumber);
            command.ExecuteNonQuery();
            }
             finally
             {
                 if (connection.State == ConnectionState.Open)
                     connection.Close();
             }
        }

        private void InsertVersionRow(DbConnection connection, int orderNumber)
        {
            string query = "INSERT INTO VersionTable (Version) VALUES (@versionNum)";
            try
            {
                connection.Open();
                var command = CreateSingleParamed(connection, query, orderNumber);
                command.ExecuteNonQuery();
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private DbCommand CreateSingleParamed(DbConnection connection, string query, int orderNumber)
        {
            var command = connection.CreateCommand();
            command.CommandText = query;
            var parameter = command.CreateParameter();
            parameter.ParameterName = "versionNum";
            parameter.Value = orderNumber;
            command.Parameters.Add(parameter);
            return command;
        }

        private int GetVersion(DbConnection connection)
        {
            string query = "SELECT TOP 1 Version FROM VersionTable";
            var command = connection.CreateCommand();
            command.CommandText = query;
            object obj = command.ExecuteScalar();
            return obj != null ? (int)obj : 0;
        }

        public void CheckForTable(DbConnection connection)
        {
            string query = @"
IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'VersionTable'))
BEGIN
  CREATE TABLE [dbo].[VersionTable](
	[Version] [int] NULL
) ON [PRIMARY]
END";
            var command = connection.CreateCommand();
            command.CommandText = query;
            command.ExecuteNonQuery();
        }
    }
}
