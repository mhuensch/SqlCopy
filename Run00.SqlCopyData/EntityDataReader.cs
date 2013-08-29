//===============================================================================
// Copyright © 2008 Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
//
//This code lives here: http://code.msdn.microsoft.com/LinqEntityDataReader
//Please visit for comments, issues and updates
//
//Version 1.0.0.1 Added GetSchemaTable support for Loading DataTables from EntityDataReader
//Version 1.0.0.2 Added support for Entity Framework types, including Foreign Key columns
//Version 1.0.0.3 In DataReader.GetValue, now using dynamic methods for common scalar types instead of reflection with PropertyInfo.GetValue()
//Version 1.0.0.4 Added support for simple IEnumerable<T> where T is a scalar to support, eg, passing List<int> to a TVP
//Version 1.0.0.5 Simplified the Attribute code, added dynamic method support for all scalar types
//Version 1.0.0.6 Replaced the switch block for ValueAccessor with a Lambda Expression
//Version 1.0.0.6 Fixed a bug with nullable foreign keys on EF entities
//Version 1.0.0.6 Extensive refactoring, introduced EntityDataReaderOptions, changed constructors
//Version 1.0.0.6 Introduced option to flatten related entities.  Now you can have the EntityDataReader flatten an object graph.
//                This is especially useful for enabling you to project all the scalars of a related enty by just projecting the entity.
//                eg. a projection like new { salesOrder.ID, salesOrder.Customer, salesOrder.Product }                 
//                will create a DbDataReader with Id, Customer.*, Product.*
//Version 1.0.0.7 For anonymous types the order of columns is now controlled by the anonymous type's constructor arguments, for which 
//                Reflection tracks the ordinal position.  This ordering is applied to any type where the constructor args match the properties
//                1-1 on name and type.  Reflection over properties does not preserve the declaration order of the properties, and for Table Valued Parameters
//                SqlClient maps the DbDataReader columns to the TVP columns by ordinal position, not by name.  This relies on the behavior of 
//                the C# compiler that Anonymous types have a constructor with constructor arguments that match the object initializer on both name
//                and position.



namespace Run00.SqlCopyData
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Data.Metadata.Edm;
	using System.Data.Objects;
	using System.Data.Objects.DataClasses;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;


	/// <summary>
	/// The EntityDataReader wraps a collection of CLR objects in a DbDataReader.  
	/// Only "scalar" properties are projected, with the exception that Entity Framework
	/// EntityObjects that have references to other EntityObjects will have key values for
	/// the related entity projected.
	/// 
	/// This is useful for doing high-speed data loads with SqlBulkCopy, and copying collections
	/// of entities ot a DataTable for use with SQL Server Table-Valued parameters, or for interop
	/// with older ADO.NET applciations.
	/// 
	/// For explicit control over the fields projected by the DataReader, just wrap your collection
	/// of entities in a anonymous type projection before wrapping it in an EntityDataReader.
	/// 
	/// Instead of 
	/// IEnumerable<Order> orders;
	/// ...
	/// IDataReader dr = orders.AsDataReader();
	/// 
	/// do
	/// IEnumerable<Order> orders;
	/// ...
	/// var q = from o in orders
	///         select new 
	///         {
	///            ID=o.ID,
	///            ShipDate=o.ShipDate,
	///            ProductName=o.Product.Name,
	///            ...
	///         }
	/// IDataReader dr = q.AsDataReader();
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class EntityDataReader<T> : DbDataReader, IDataReader
	{
		public EntityDataReader(IQueryable<T> col)
		{
			this.enumerator = col.GetEnumerator();

			//done without a lock, so we risk running twice
			if (scalarAttributes == null)
			{
				scalarAttributes = DiscoverScalarAttributes(typeof(T));
			}
			if (scalarAttributesPlusRelatedObjectScalarAttributes == null)
			{
				var atts = DiscoverRelatedObjectScalarAttributes(typeof(T));
				scalarAttributesPlusRelatedObjectScalarAttributes = atts.Concat(scalarAttributes).ToList();
			}

			attributes = scalarAttributesPlusRelatedObjectScalarAttributes;


		}

		public override DataTable GetSchemaTable()
		{
			DataSet s = new DataSet();
			s.Locale = System.Globalization.CultureInfo.CurrentCulture;
			s.ReadXmlSchema(new System.IO.StringReader(shemaTableSchema));
			DataTable t = s.Tables[0];
			for (int i = 0; i < this.FieldCount; i++)
			{
				DataRow row = t.NewRow();
				row["ColumnName"] = this.GetName(i);
				row["ColumnOrdinal"] = i;

				Type type = this.GetFieldType(i);
				if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(System.Nullable<int>).GetGenericTypeDefinition())
				{
					type = type.GetGenericArguments()[0];
				}
				row["DataType"] = this.GetFieldType(i);
				row["DataTypeName"] = this.GetDataTypeName(i);
				row["ColumnSize"] = -1;
				t.Rows.Add(row);
			}
			return t;

		}

		public override void Close()
		{
			closed = true;
		}

		public override int Depth
		{
			get { return 1; }
		}

		public override bool IsClosed
		{
			get { return closed; }
		}

		public override bool NextResult()
		{
			return false;
		}

		public override bool Read()
		{
			bool rv = enumerator.MoveNext();
			if (rv)
			{
				current = enumerator.Current;
				entitiesRead += 1;
			}
			return rv;
		}

		public override int RecordsAffected
		{
			get { return -1; }
		}

		protected override void Dispose(bool disposing)
		{
			Close();
			base.Dispose(disposing);
		}

		public override int FieldCount
		{
			get
			{
				return attributes.Count;
			}
		}

		public override bool GetBoolean(int i)
		{
			return GetValue<bool>(i);
		}

		public override byte GetByte(int i)
		{
			return GetValue<byte>(i);
		}

		public override long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
		{

			var buf = GetValue<byte[]>(i);
			int bytes = Math.Min(length, buf.Length - (int)fieldOffset);
			Buffer.BlockCopy(buf, (int)fieldOffset, buffer, bufferoffset, bytes);
			return bytes;

		}

		public override char GetChar(int i)
		{
			return GetValue<char>(i);
		}

		public override long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
		{
			//throw new NotImplementedException();
			string s = GetValue<string>(i);
			int chars = Math.Min(length, s.Length - (int)fieldoffset);
			s.CopyTo((int)fieldoffset, buffer, bufferoffset, chars);

			return chars;
		}

		public override string GetDataTypeName(int i)
		{
			return attributes[i].Type.Name;
		}

		public override DateTime GetDateTime(int i)
		{
			return GetValue<DateTime>(i);
		}

		public override decimal GetDecimal(int i)
		{
			return GetValue<decimal>(i);
		}

		public override double GetDouble(int i)
		{
			return GetValue<double>(i);
		}

		public override Type GetFieldType(int i)
		{
			Type t = attributes[i].Type;
			return t;
		}

		public override float GetFloat(int i)
		{
			return GetValue<float>(i);
		}

		public override Guid GetGuid(int i)
		{
			return GetValue<Guid>(i);
		}

		public override short GetInt16(int i)
		{
			return GetValue<short>(i);
		}

		public override int GetInt32(int i)
		{
			return GetValue<int>(i);
		}

		public override long GetInt64(int i)
		{
			return GetValue<long>(i);
		}

		public override string GetName(int i)
		{
			Attribute a = attributes[i];
			return a.Name;
		}

		public override int GetOrdinal(string name)
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				var a = attributes[i];

				if (!a.IsRelatedAttribute && a.Name == name)
				{
					return i;
				}


			}
			return -1;
		}

		public override string GetString(int i)
		{
			return GetValue<string>(i);
		}

		public override int GetValues(object[] values)
		{
			for (int i = 0; i < attributes.Count; i++)
			{
				values[i] = GetValue(i);
			}
			return attributes.Count;
		}

		public override object GetValue(int i)
		{
			object o = GetValue<object>(i);
			return o;
		}

		public override bool IsDBNull(int i)
		{
			object o = GetValue<object>(i);
			return (o == null);
		}

		public override object this[string name]
		{
			get { return GetValue(GetOrdinal(name)); }
		}

		public override object this[int i]
		{
			get { return GetValue(i); }
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			return this.enumerator;
		}

		public override bool HasRows
		{
			get { throw new NotSupportedException(); }
		}

		private TField GetValue<TField>(int i)
		{
			TField val = (TField)attributes[i].GetValue(current);
			return val;
		}


		private static bool IsScalarType(Type t)
		{
			return scalarTypes.Contains(t);
		}

		private static readonly HashSet<Type> scalarTypes = LoadScalarTypes();

		private static HashSet<Type> LoadScalarTypes()
		{
			HashSet<Type> set = new HashSet<Type>() 
															{ 
																//reference types
																typeof(String),
																typeof(Byte[]),
																//value types
																typeof(Byte),
																typeof(Int16),
																typeof(Int32),
																typeof(Int64),
																typeof(Single),
																typeof(Double),
																typeof(Decimal),
																typeof(DateTime),
																typeof(Guid),
																typeof(Boolean),
																typeof(TimeSpan),
																//nullable value types
																typeof(Byte?),
																typeof(Int16?),
																typeof(Int32?),
																typeof(Int64?),
																typeof(Single?),
																typeof(Double?),
																typeof(Decimal?),
																typeof(DateTime?),
																typeof(Guid?),
																typeof(Boolean?),
																typeof(TimeSpan?)
															};


			return set;

		}

		private static List<Attribute> DiscoverScalarAttributes(Type thisType)
		{

			//Not a collection of entities, just an IEnumerable<String> or other scalar type.
			//So add just a single Attribute that returns the object itself
			if (IsScalarType(thisType))
			{
				return new List<Attribute> { new Attribute("Value", "Value", thisType, t => t, false) };
			}


			//find all the scalar properties
			var allProperties = (from p in thisType.GetProperties()
													 where IsScalarType(p.PropertyType)
													 select p).ToList();

			//Look for a constructor with arguments that match the properties on name and type
			//(name modulo case, which varies between constructor args and properties in coding convention)
			//If such an "ordering constructor" exists, return the properties ordered by the corresponding
			//constructor args ordinal position.  
			//An important instance of an ordering constructor, is that C# anonymous types all have one.  So
			//this enables a simple convention to specify the order of columns projected by the EntityDataReader
			//by simply building the EntityDataReader from an anonymous type projection.
			//If such a constructor is found, replace allProperties with a collection of properties sorted by constructor order.
			foreach (var completeConstructor in from ci in thisType.GetConstructors()
																					where ci.GetParameters().Count() == allProperties.Count()
																					select ci)
			{
				var q = (from cp in completeConstructor.GetParameters()
								 join p in allProperties
									 on new { n = cp.Name.ToLower(), t = cp.ParameterType } equals new { n = p.Name.ToLower(), t = p.PropertyType }
								 select new { cp, p }).ToList();

				if (q.Count() == allProperties.Count()) //all constructor parameters matched by name and type to properties
				{
					//sort all properties by constructor ordinal position
					allProperties = (from o in q
													 orderby o.cp.Position
													 select o.p).ToList();
					break; //stop looking for an ordering consturctor
				}


			}

			return allProperties.Select(p => new Attribute(p)).ToList();

		}

		private static List<Attribute> DiscoverRelatedObjectKeyAttributes(Type thisType, ObjectContext objectContext)
		{

			var attributeList = new SortedList<string, Attribute>();


			//recreate foreign key column values
			//by adding Attributes for any key values of referenced entities 
			//that aren't already exposed as scalar properties
			var mw = objectContext.MetadataWorkspace;
			var entityTypesByName = mw.GetItems<EntityType>(DataSpace.OSpace).ToLookup(e => e.FullName);

			//find the EntityType metadata for T 
			EntityType thisEntity = entityTypesByName[thisType.FullName].First();
			var thisEntityKeys = thisEntity.KeyMembers.ToDictionary(k => k.Name);

			//TODOx use the NavigationProperties instead of the ENtityRelations -- too complicated
			//TODO fix the attribute naming.  Probably requires marking each attribtue as direct or related.


			var erProps = thisType.GetProperties()
														.Where(p => typeof(EntityReference)
														.IsAssignableFrom(p.PropertyType)).ToList();


			//For each EntityRelation property add add the keys of the related Entity
			foreach (var pi in erProps)
			{
				//Find the name of the CLR Type at the other end of the reference because we need to get its key attributes.
				//the property type is EntityReference<T>, we need T.
				string relatedEntityCLRTypeName = pi.PropertyType.GetGenericArguments().First().FullName;

				//Find the EntityType at the other end of the relationship because we need to get its key attributes.
				EntityType relatedEntityEFType = entityTypesByName[relatedEntityCLRTypeName].FirstOrDefault();
				if (relatedEntityEFType == null)
				{
					throw new InvalidOperationException("Cannot find EntityType for EntityReference Property " + pi.Name);
				}

				//Add attributes for each key value of the related entity.  These are the properties that
				//would probably appear in the storage object.  The names will be the same as they are on the 
				//related entity, except prefixed with the related entity name, 
				//and with a check to make sure that we're not introducing a duplicate.
				// so if you have 
				//  if OrderItem.OrderID -> Order.ID   then the column will be Order_ID
				//  if OrderItem.OrderID -> Order.OrderID   then the column will be Order_OrderID
				foreach (var key in relatedEntityEFType.KeyMembers)
				{
					string targetKeyAttributeName = key.Name;

					//TODO it would be better to get the NavigationProperty and find the ToEndMember name
					//but the NavigationProperty doesn't have good way to get the EntityReference 
					//or the related entity key.
					string referenceName;
					if (pi.Name.EndsWith("Reference", StringComparison.Ordinal))
					{
						referenceName = pi.Name.Substring(0, pi.Name.Length - "Reference".Length);
					}
					else  //there's no rule that the EntityReference named like this so, if not just use the target type
					{
						referenceName = pi.PropertyType.Name;

						//if there are multiple relations to the same target type, just uniqify them with an index
						int ix = erProps.Where(p => p.PropertyType == pi.PropertyType).ToList().IndexOf(pi);
						if (ix > 0)
						{
							referenceName = referenceName + ix.ToString(System.Globalization.CultureInfo.InvariantCulture);
						}

					}
					string fullName = referenceName + "_" + key.Name;


					//bind out local variables for the valueAccessor closure.
					Type kType = Type.GetType(key.TypeUsage.EdmType.FullName);
					PropertyInfo entityReferenceProperty = pi;

					Func<T, object> valueAccessor = o =>
					{
						EntityReference er = (EntityReference)entityReferenceProperty.GetValue(o, null);

						//for nullable foregn keys, just return null
						if (er.EntityKey == null)
						{
							return null;
						}
						object val = er.EntityKey.EntityKeyValues.First(k => k.Key == targetKeyAttributeName).Value;
						return val;
					};
					string name = key.Name;

					attributeList.Add(name, new Attribute(fullName, name, kType, valueAccessor, true));
				}


			}

			return attributeList.Values.ToList();

		}

		private static List<Attribute> DiscoverRelatedObjectScalarAttributes(Type thisType)
		{

			var atts = new List<Attribute>();

			//get the related objects which aren't scalars, not EntityReference objects and not collections
			var relatedObjectProperties =
												(from p in thisType.GetProperties()
												 where !IsScalarType(p.PropertyType)
														&& !typeof(System.Collections.IEnumerable).IsAssignableFrom(p.PropertyType)
														&& !typeof(EntityReference).IsAssignableFrom(p.PropertyType)
														&& !typeof(EntityKey).IsAssignableFrom(p.PropertyType)
												 select p).ToList();

			foreach (var rop in relatedObjectProperties)
			{
				var type = rop.PropertyType;
				//get the scalar properties for the related type
				var scalars = type.GetProperties().Where(p => IsScalarType(p.PropertyType)).ToList();

				foreach (var sp in scalars)
				{
					string attName = rop.Name + "_" + sp.Name;
					//create a value accessor which takes an instance of T, and returns the related object scalar
					var valueAccessor = Attribute.MakeRelatedPropertyAccessor<T, object>(rop, sp);
					string name = attName;
					Attribute att = new Attribute(rop.Name, attName, sp.PropertyType, valueAccessor, true);
					atts.Add(att);
				}

			}
			return atts;

		}

		private static Type nullable_T = typeof(System.Nullable<int>).GetGenericTypeDefinition();

		private static bool IsNullable(Type t)
		{
			return (t.IsGenericType
					&& t.GetGenericTypeDefinition() == nullable_T);
		}

		private static Type StripNullableType(Type t)
		{
			return t.GetGenericArguments()[0];
		}


		private int entitiesRead = 0;
		private readonly IEnumerator<T> enumerator;
		private T current;
		private bool closed = false;
		private static List<Attribute> scalarAttributes;
		private static List<Attribute> scalarAttributesPlusRelatedObjectScalarAttributes;
		private readonly List<Attribute> attributes;




		private class Attribute
		{
			//PropertyInfo propertyInfo;
			public readonly Type Type;
			public readonly string FullName;
			public readonly string Name;
			public readonly bool IsRelatedAttribute;

			readonly Func<T, object> ValueAccessor;

			/// <summary>
			/// Uses Lamda expressions to create a Func<T,object> that invokes the given property getter.
			/// The property value will be extracted and cast to type TProperty
			/// </summary>
			/// <typeparam name="TObject">The type of the object declaring the property.</typeparam>
			/// <typeparam name="TProperty">The type to cast the property value to</typeparam>
			/// <param name="pi">PropertyInfo pointing to the property to wrap</param>
			/// <returns></returns>
			public static Func<TObject, TProperty> MakePropertyAccessor<TObject, TProperty>(PropertyInfo pi)
			{
				ParameterExpression objParam = Expression.Parameter(typeof(TObject), "obj");
				MemberExpression typedAccessor = Expression.PropertyOrField(objParam, pi.Name);
				UnaryExpression castToObject = Expression.Convert(typedAccessor, typeof(object));
				LambdaExpression lambdaExpr = Expression.Lambda<Func<TObject, TProperty>>(castToObject, objParam);

				return (Func<TObject, TProperty>)lambdaExpr.Compile();
			}


			public static Func<TObject, TProperty> MakeRelatedPropertyAccessor<TObject, TProperty>(PropertyInfo pi, PropertyInfo pi2)
			{

				Func<TObject, object> getRelatedObject;
				{
					// expression like:
					//    return (object)t.SomeProp;
					ParameterExpression typedParam = Expression.Parameter(typeof(T), "t");
					MemberExpression typedAccessor = Expression.PropertyOrField(typedParam, pi.Name);
					UnaryExpression castToObject = Expression.Convert(typedAccessor, typeof(object));
					LambdaExpression lambdaExpr = Expression.Lambda<Func<TObject, object>>(castToObject, typedParam);
					getRelatedObject = (Func<TObject, object>)lambdaExpr.Compile();
				}


				Func<object, TProperty> getRelatedObjectProperty;
				{

					// expression like:
					//    return (object)((PropType)o).RelatedProperty;
					ParameterExpression objParam = Expression.Parameter(typeof(object), "o");
					UnaryExpression typedParam = Expression.Convert(objParam, pi.PropertyType);
					MemberExpression typedAccessor = Expression.PropertyOrField(typedParam, pi2.Name);
					UnaryExpression castToObject = Expression.Convert(typedAccessor, typeof(TProperty));
					LambdaExpression lambdaExpr = Expression.Lambda<Func<object, TProperty>>(castToObject, objParam);
					getRelatedObjectProperty = (Func<object, TProperty>)lambdaExpr.Compile();
				}

				Func<TObject, TProperty> f = (TObject t) =>
				{
					object o = getRelatedObject(t);
					if (o == null) return default(TProperty);
					return getRelatedObjectProperty(o);
				};

				return f;
			}

			public Attribute(PropertyInfo pi)
			{
				this.FullName = pi.DeclaringType.Name + "_" + pi.Name;
				this.Name = pi.Name;
				Type = pi.PropertyType;
				IsRelatedAttribute = false;

				ValueAccessor = MakePropertyAccessor<T, object>(pi);
			}

			public Attribute(string fullName, string name, Type type, Func<T, object> getValue, bool isRelatedAttribute)
			{
				this.FullName = fullName;
				this.Name = name;
				this.Type = type;
				this.ValueAccessor = getValue;
				this.IsRelatedAttribute = isRelatedAttribute;
			}

			public object GetValue(T target)
			{
				return ValueAccessor(target);
			}
		}


		const string shemaTableSchema = @"<?xml version=""1.0"" standalone=""yes""?>
<xs:schema id=""NewDataSet"" xmlns="""" xmlns:xs=""http://www.w3.org/2001/XMLSchema"" xmlns:msdata=""urn:schemas-microsoft-com:xml-msdata"">
	<xs:element name=""NewDataSet"" msdata:IsDataSet=""true"" msdata:MainDataTable=""SchemaTable"" msdata:Locale="""">
		<xs:complexType>
			<xs:choice minOccurs=""0"" maxOccurs=""unbounded"">
				<xs:element name=""SchemaTable"" msdata:Locale="""" msdata:MinimumCapacity=""1"">
					<xs:complexType>
						<xs:sequence>
							<xs:element name=""ColumnName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
							<xs:element name=""ColumnOrdinal"" msdata:ReadOnly=""true"" type=""xs:int"" default=""0"" minOccurs=""0"" />
							<xs:element name=""ColumnSize"" msdata:ReadOnly=""true"" type=""xs:int"" minOccurs=""0"" />
							<xs:element name=""NumericPrecision"" msdata:ReadOnly=""true"" type=""xs:short"" minOccurs=""0"" />
							<xs:element name=""NumericScale"" msdata:ReadOnly=""true"" type=""xs:short"" minOccurs=""0"" />
							<xs:element name=""IsUnique"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
							<xs:element name=""IsKey"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
							<xs:element name=""BaseServerName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
							<xs:element name=""BaseCatalogName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
							<xs:element name=""BaseColumnName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
							<xs:element name=""BaseSchemaName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
							<xs:element name=""BaseTableName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
							<xs:element name=""DataType"" msdata:DataType=""System.Type, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
							<xs:element name=""AllowDBNull"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
							<xs:element name=""ProviderType"" msdata:ReadOnly=""true"" type=""xs:int"" minOccurs=""0"" />
							<xs:element name=""IsAliased"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
							<xs:element name=""IsExpression"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
							<xs:element name=""IsIdentity"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
							<xs:element name=""IsAutoIncrement"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
							<xs:element name=""IsRowVersion"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
							<xs:element name=""IsHidden"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
							<xs:element name=""IsLong"" msdata:ReadOnly=""true"" type=""xs:boolean"" default=""false"" minOccurs=""0"" />
							<xs:element name=""IsReadOnly"" msdata:ReadOnly=""true"" type=""xs:boolean"" minOccurs=""0"" />
							<xs:element name=""ProviderSpecificDataType"" msdata:DataType=""System.Type, mscorlib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
							<xs:element name=""DataTypeName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
							<xs:element name=""XmlSchemaCollectionDatabase"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
							<xs:element name=""XmlSchemaCollectionOwningSchema"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
							<xs:element name=""XmlSchemaCollectionName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
							<xs:element name=""UdtAssemblyQualifiedName"" msdata:ReadOnly=""true"" type=""xs:string"" minOccurs=""0"" />
							<xs:element name=""NonVersionedProviderType"" msdata:ReadOnly=""true"" type=""xs:int"" minOccurs=""0"" />
						</xs:sequence>
					</xs:complexType>
				</xs:element>
			</xs:choice>
		</xs:complexType>
	</xs:element>
</xs:schema>";
	}
}