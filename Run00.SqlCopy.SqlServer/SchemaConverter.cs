using Run00.SqlCopy;
using Run00.SqlCopySchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Run00.SqlCopySqlServer
{
	public class SchemaConverter : ISchemaConverter
	{
		public SchemaConverter(IEntityInterfaceLocator interfaceLocator)
		{
			_interfaceLocator = interfaceLocator;
		}

		IEnumerable<Type> ISchemaConverter.ToEntityTypes(Database database)
		{
			var result = new List<Type>();
			var assemblyBuilder = AppDomain
					.CurrentDomain
					.DefineDynamicAssembly(new AssemblyName(Guid.NewGuid().ToString()), AssemblyBuilderAccess.RunAndSave);

			var moduleBuilder = assemblyBuilder.DefineDynamicModule(Guid.NewGuid().ToString());

			foreach (var table in database.Tables.Where(t => t.IsSystemObject == false))
			{
				var typeName = table.Schema + "." + table.Name;
				var interfacesForType = _interfaceLocator.GetInterfacesForEntity(typeName).ToArray();
				var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public, null, interfacesForType);
				typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

				foreach (var column in table.Columns)
					CreateAutoImplementedProperty(typeBuilder, column.Name, column.Type);

				var type = typeBuilder.CreateType();
				result.Add(type);
			}           

			return result;
		}

		private static void CreateAutoImplementedProperty(TypeBuilder builder, string propertyName, Type propertyType)
		{
			const string PrivateFieldPrefix = "m_";
			const string GetterPrefix = "get_";
			const string SetterPrefix = "set_";

            
			// Generate the field.
			FieldBuilder fieldBuilder = builder.DefineField(
					string.Concat(PrivateFieldPrefix, propertyName), propertyType, FieldAttributes.Private);

			// Generate the property
			PropertyBuilder propertyBuilder = builder.DefineProperty(
					propertyName, PropertyAttributes.HasDefault, propertyType, null);

			// Property getter and setter attributes.
			MethodAttributes propertyMethodAttributes =
					MethodAttributes.Public | MethodAttributes.FamANDAssem | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask | MethodAttributes.SpecialName;

			// Define the getter method.
			MethodBuilder getterMethod = builder.DefineMethod(
					string.Concat(GetterPrefix, propertyName),
					propertyMethodAttributes, propertyType, Type.EmptyTypes);

			// Emit the IL code.
			// ldarg.0
			// ldfld,_field
			// ret
			ILGenerator getterILCode = getterMethod.GetILGenerator();
			getterILCode.Emit(OpCodes.Ldarg_0);
			getterILCode.Emit(OpCodes.Ldfld, fieldBuilder);
			getterILCode.Emit(OpCodes.Ret);

			// Define the setter method.
			MethodBuilder setterMethod = builder.DefineMethod(
					string.Concat(SetterPrefix, propertyName),
					propertyMethodAttributes, null, new Type[] { propertyType });

			// Emit the IL code.
			// ldarg.0
			// ldarg.1
			// stfld,_field
			// ret
			ILGenerator setterILCode = setterMethod.GetILGenerator();
			setterILCode.Emit(OpCodes.Ldarg_0);
			setterILCode.Emit(OpCodes.Ldarg_1);
			setterILCode.Emit(OpCodes.Stfld, fieldBuilder);
			setterILCode.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getterMethod);
			propertyBuilder.SetSetMethod(setterMethod);
		}

		private readonly IEntityInterfaceLocator _interfaceLocator;
	}
}
