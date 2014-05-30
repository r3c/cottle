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

		public Function (IEnumerable<string> arguments, Command command, Trimmer trimmer)
		{
			Compiler		compiler;
			DynamicMethod	method;

			method = new DynamicMethod (string.Empty, typeof (Value), new [] {typeof (Storage), typeof (IList<Value>), typeof (IScope), typeof (TextWriter)}, this.GetType ());
			compiler = new Compiler (method.GetILGenerator (), trimmer);

			this.storage = compiler.CompileFunction (arguments, command);
			this.renderer = (Renderer)method.CreateDelegate (typeof (Renderer));
/*
			var name = "HelloWorld.exe";
			var assemblyname = new System.Reflection.AssemblyName(name);
			var assemblybuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyname, AssemblyBuilderAccess.RunAndSave);
			var modulebuilder = assemblybuilder.DefineDynamicModule(name, "x.dll");
			var programmclass = modulebuilder.DefineType("Program",System.Reflection.TypeAttributes.Public);
			var method2 = programmclass.DefineMethod("Main",System.Reflection.MethodAttributes.Public | System.Reflection.MethodAttributes.Static,typeof (Value), new [] {typeof (Storage), typeof (IList<Value>), typeof (IScope), typeof (TextWriter)});
			compiler = new Compiler (method2.GetILGenerator (), trimmer);
			compiler.CompileFunction (arguments, command);
			programmclass.CreateType();
			assemblybuilder.Save ("x.dll");
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
