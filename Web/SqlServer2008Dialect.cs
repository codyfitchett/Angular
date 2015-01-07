using ServiceStack.OrmLite;
using ServiceStack.OrmLite.SqlServer;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RevHR.Web
{
	public static class SqlServer2008Dialect
	{
		public static IOrmLiteDialectProvider Provider { get { return SqlServer2008OrmLiteDialectProvider.Instance; } }

		private class SqlServer2008OrmLiteDialectProvider : SqlServerOrmLiteDialectProvider
		{
			public new static SqlServerOrmLiteDialectProvider Instance = new SqlServer2008OrmLiteDialectProvider();

			protected override string GetUndefinedColumnDefinition(Type fieldType, int? fieldLength)
			{
				if (TypeSerializer.CanCreateFromString(fieldType))
				{
					return string.Format(StringLengthColumnDefinitionFormat, "MAX");
				}

				throw new NotSupportedException(
					string.Format("Property of type: {0} is not supported", fieldType.FullName));
			}
		}
	}
}