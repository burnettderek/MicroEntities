using AutoMapper.Internal;
using MicroEntities.Attributes;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace MicroEntities.Data.SqlServer
{
	internal class SqlServerSchemaEnforcer<TEntity> where TEntity : new()
	{
		public SqlServerSchemaEnforcer(ILogger<SqlServerSystemLayer<TEntity>> log, string connectionString, string tableName)
		{
			_log = log;
			_connectionString = connectionString;
			_tableName = tableName;
			Properties = typeof(TEntity).GetProperties().ToList();
			InputProperties = Properties.Where(prop => !(prop.GetCustomAttributes(typeof(ReadOnly), true).Length > 0)).ToList();
			InitializeTypeMap();
		}

		public async Task EnforceSchema()
		{
			_log.LogDebug($"Checking if table '{_tableName}' exists.");
			var check = $"IF OBJECT_ID(N'dbo.{_tableName}', N'U') IS NULL SELECT 0 ELSE SELECT 1";
			using (var connection = new SqlConnection(_connectionString))
			{
				bool exists = false;
				connection.Open();
				using (var command = new SqlCommand(check, connection))
				{
					var result = await command.ExecuteScalarAsync();
					if(result != null)
						exists = (int)result == 1;
				}
				if(!exists)
				{
					_log.LogDebug($"Table '{_tableName}' does not exist and will be created.");
					var sql = $"CREATE TABLE dbo.{_tableName} ( {GetPropertyString()} )";
					using (var command = new SqlCommand(sql, connection))
					{
						await command.ExecuteNonQueryAsync();
					}
				}
				else
				{
					_log.LogDebug($"Table '{_tableName}' already exists and will not be created.");
				}
				connection.Close();
			}
		}

		private string GetPropertyString()
		{
			var builder = new StringBuilder();
			foreach (var property in Properties)
			{
				var maxStorageSize = property.GetCustomAttributes(typeof(MaxStorageSize), true).FirstOrDefault() as MaxStorageSize;
				string type = GetType(property, maxStorageSize);
				builder.Append($"\n[{property.Name}] {type}");
				if (property != Properties.Last()) builder.Append(",");
			}
			return builder.ToString();
		}

		private string GetType(PropertyInfo property, MaxStorageSize maxStorageSize)
		{
			var type = property.PropertyType;
			var nullQualifier = "NOT NULL";
			if (type.IsNullableType())
			{
				nullQualifier = "NULL";
				type = Nullable.GetUnderlyingType(property.PropertyType);
			}
			else if (type.IsEnum)
				type = Enum.GetUnderlyingType(property.PropertyType);
			
			return $"{ToSql(type.Name, maxStorageSize)} {nullQualifier} {GetConstraints(property)}";
		}

		private string GetConstraints(PropertyInfo property)
		{
			var constraints = new List<string>();
			var id = property.GetCustomAttributes(typeof(Identity), true).FirstOrDefault() as Identity;
			if(id != null)
				constraints.Add($"IDENTITY({id.Seed}, {id.Increment})");
			var primaryKey = property.GetCustomAttributes(typeof(PrimaryKey), true).FirstOrDefault() as PrimaryKey;
			if (primaryKey != null)
			{
				constraints.Add($"PRIMARY KEY");
			}
			var defaultValue = property.GetCustomAttributes(typeof(Default), true).FirstOrDefault() as Default;
			if(defaultValue != null)
			{
				constraints.Add(ToSql(defaultValue));
			}
			var isUnique = property.GetCustomAttributes(typeof(Unique), true).FirstOrDefault() as Unique;
			if (isUnique != null)
			{
				constraints.Add("UNIQUE");
			}
			var foreignKey = property.GetCustomAttributes(typeof(ForeignKey), true).FirstOrDefault() as ForeignKey;
			if (foreignKey != null)
			{
				constraints.Add($"FOREIGN KEY REFERENCES {foreignKey.TableName}([{foreignKey.FieldName}])");
			}
			var builder = new StringBuilder();
			foreach(var constraint in constraints)
			{
				builder.Append(constraint);
				if (constraint != constraints.Last()) builder.Append(" ");
			}
			return builder.ToString();
		}

		private string ToSql(Default def)
		{
			if(def.Value is Default.Func)
			{
				switch(def.Value)
				{
					case Default.Func.TimeStamp:
						return "DEFAULT GETDATE()";
					case Default.Func.TimeStampUtc:
						return "DEFAULT GETUTCDATE()";
					case Default.Func.NewGuid:
						return "DEFAULT NEWID()";
					default:
						throw new Exception($"Default.Func '{def.Value}' is not supported.");
				}
			}
			else if(def.Value is string)
			{
				return $"DEFAULT '{def.Value}'";
			}
			if (def.Value != null)
				return $"DEFAULT {def.Value}";
			else
				return "DEFAULT NULL";
		}

		private void InitializeTypeMap()
		{
			_typeMap[typeof(string).Name] = SqlDbType.NVarChar.ToString();
			_typeMap[typeof(char[]).Name] = SqlDbType.NVarChar.ToString();
			_typeMap[typeof(int).Name] = SqlDbType.Int.ToString();
			_typeMap[typeof(Int32).Name] = SqlDbType.Int.ToString();
			_typeMap[typeof(Int16).Name] = SqlDbType.SmallInt.ToString();
			_typeMap[typeof(Int64).Name] = SqlDbType.BigInt.ToString();
			_typeMap[typeof(Byte[]).Name] = SqlDbType.VarBinary.ToString();
			_typeMap[typeof(Boolean).Name] = SqlDbType.Bit.ToString();
			_typeMap[typeof(DateTime).Name] = SqlDbType.DateTime2.ToString();
			_typeMap[typeof(DateTimeOffset).Name] = SqlDbType.DateTimeOffset.ToString();
			_typeMap[typeof(Decimal).Name] = SqlDbType.Decimal.ToString();
			_typeMap[typeof(Double).Name] = SqlDbType.Float.ToString();
			_typeMap[typeof(Decimal).Name] = SqlDbType.Money.ToString();
			_typeMap[typeof(Byte).Name] = SqlDbType.TinyInt.ToString();
			_typeMap[typeof(TimeSpan).Name] = SqlDbType.Time.ToString();
			_typeMap[typeof(Guid).Name] = SqlDbType.UniqueIdentifier.ToString();
		}

		private string ToSql(string dotNetType, MaxStorageSize maxStorageSize)
		{
			if (maxStorageSize == null)
			{
				if (dotNetType.Equals("String") || dotNetType.Equals("Char[]") || dotNetType.Equals("Byte[]"))
					return _typeMap[dotNetType] + "(max)";
				return _typeMap[dotNetType];
			}
			else
				return $"{_typeMap[dotNetType]}({maxStorageSize.SizeInBytes})";
		}

		protected readonly ILogger<SqlServerSystemLayer<TEntity>> _log;
		protected readonly string _connectionString;
		protected readonly string _tableName;
		protected List<PropertyInfo> Properties = new();
		protected List<PropertyInfo> InputProperties = new();
		protected Dictionary<string, string> _typeMap = new();
	}
}
