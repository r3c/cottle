using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;

namespace Cottle.Documents.Dynamic
{
	class Function : IFunction
	{
		#region Attributes

		private readonly Renderer	renderer;

		private readonly Storage	storage;

		#endregion

		#region Constructors

//		private static int g = 0;

		public Function (IEnumerable<string> arguments, Command command, Trimmer trimmer)
		{
			Compiler		compiler;
			DynamicMethod	method;

			method = new DynamicMethod (string.Empty, typeof (Value), new [] {typeof (Storage), typeof (IList<Value>), typeof (IScope), typeof (TextWriter)}, this.GetType ());
			compiler = new Compiler (method.GetILGenerator (), trimmer);

//			int p = g++;

			this.storage = compiler.Compile (arguments, command);
			this.renderer = (Renderer)method.CreateDelegate (typeof (Renderer));
/*
			var assemblyName = new System.Reflection.AssemblyName ("Dynamic_" + p);
			var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly (assemblyName, AssemblyBuilderAccess.RunAndSave);
			var moduleBuilder = assemblyBuilder.DefineDynamicModule (assemblyName.Name, assemblyName.Name + ".dll");
			var program = moduleBuilder.DefineType ("Program",System.Reflection.TypeAttributes.Public);
			var main = program.DefineMethod ("Main", System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Static, typeof (Value), new [] {typeof (Storage), typeof (IList<Value>), typeof (IScope), typeof (TextWriter)});
			compiler = new Compiler (main.GetILGenerator (), trimmer);
			compiler.Compile (arguments, command);
			program.CreateType();
			assemblyBuilder.Save (assemblyName.Name + ".dll");
*/
		}

		#endregion

		#region Methods

		public Value Execute (IList<Value> arguments, IScope scope, TextWriter output)
		{
			return this.renderer (this.storage, arguments, scope, output);
		}

		public override string ToString ()
		{
			return "<dynamic>";
		}

		#endregion
	}
}
