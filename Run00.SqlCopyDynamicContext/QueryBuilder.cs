//using Run00.SqlCopy;
//using System;
//using System.Collections.Generic;
//using System.Data.Objects;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Run00.SqlCopyDynamicContext
//{
//	public class QueryBuilder : IQueryBuilder
//	{
//		Query IQueryBuilder.Build(IQueryable query)
//		{
//			var objectQuery = GetObjectQuery(query);
//			var result = new Query()
//			{
//				Sql = objectQuery.ToTraceString(),
//				Parameters = objectQuery.Parameters.Select(p => new QueryParameter() { Name = p.Name, Value = p.Value, Type = p.ParameterType })
//			};
//			return result;
//		}

//		private ObjectQuery GetObjectQuery(IQueryable query)
//		{
//			var internalQueryField = query.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.Name.Equals("_internalQuery")).FirstOrDefault();

//			var internalQuery = internalQueryField.GetValue(query);

//			var objectQueryField = internalQuery.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(f => f.Name.Equals("_objectQuery")).FirstOrDefault();

//			return objectQueryField.GetValue(internalQuery) as ObjectQuery;
//		}
//	}
//}
