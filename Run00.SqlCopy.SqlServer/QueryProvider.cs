using IQToolkit.Data;
using IQToolkit.Data.Common;
using IQToolkit.Data.Mapping;
using IQToolkit.Data.SqlClient;
using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopySqlServer
{
	public class QueryProviderFactory : IQueryProviderFactory
	{
		IQueryProvider IQueryProviderFactory.Create(DatabaseInfo dbInfo)
		{
			var connection = new SqlConnection(dbInfo.ConnectionString);
			return new DbEntityProvider(connection, new TSqlLanguage(), new ImplicitSqlMapping(), new EntityPolicy());
		}

		private class ImplicitSqlMapping : ImplicitMapping
		{
			public override string GetTableName(MappingEntity entity)
			{
<<<<<<< HEAD
				var name = entity.EntityType.FullName;
				if (name.Equals("dbo.user"))
				{
					name = "[dbo].[user]";
				}
				return name;
=======
				return "[" + entity.EntityType.Namespace + "].[" + entity.EntityType.Name +"]";
>>>>>>> fb90f2f7ba08b41dc8952c80fe35e9c92e544a9a
			}
		}
	}
}
