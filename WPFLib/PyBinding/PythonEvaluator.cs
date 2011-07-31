using System;
using System.IO;
using System.Windows;
using IronPython.Compiler;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System.Collections.Generic;
using Microsoft.Scripting.Runtime;

namespace PyBinding
{
    public class PythonEvaluator
    {
        #region Constants

        public const string VariablePrefix = "var_";

        private const string STARTER_SCRIPT_FILENAME = "StartupScript.py";

        #endregion

        #region Constructors

        static PythonEvaluator()
        {
            //TODO: bring back configuration via config file
            var options = new Dictionary<string, object>();
            options["LightweightScopes"] = false;
            options["Optimize"] = true;
            
            Engine = IronPython.Hosting.Python.CreateEngine(options);
            // Allow Optimized option to be set. We are caching compiled code anyway, so pinning for the sake of performance is ok. 
            DefaultCompilerOptions = new PythonCompilerOptions(ModuleOptions.Optimized | ModuleOptions.ModuleBuiltins);

            CodeCache = new Dictionary<string, CompiledCode>();
        }

        public PythonEvaluator()
            : this(STARTER_SCRIPT_FILENAME)
        {
        }
        
        public PythonEvaluator(string starterFile)
        {
            this.StarterScriptFileName = GetPath(starterFile);

            LoadAssembliesIntoEngine();

            this.ScriptScope = Engine.CreateScope();

            if (!File.Exists(this.StarterScriptFileName))
                return;
             
            var importSource = Engine.CreateScriptSourceFromFile(starterFile);
            ExecuteSafely(() => importSource.Compile(DefaultCompilerOptions).Execute(this.ScriptScope));
        }

        #endregion

        #region Properties

        public string StarterScriptFileName { get; private set; }

        private ScriptScope ScriptScope { get; set; }

        private static PythonCompilerOptions DefaultCompilerOptions { get; set; }

        private static ScriptEngine Engine { get; set; }

        private static IDictionary<String, CompiledCode> CodeCache { get; set; }

        #endregion

        #region Methods

        public void SetVariable(ScriptScope scope, string name, object value)
        {
            scope.SetVariable(name, value);
        }

        public static CompiledCode Compile(string script)
        {
            CompiledCode compiledCode;
            
            if (!CodeCache.TryGetValue(script, out compiledCode))
            {
                //specify kind so that IronPython does not have to parse the script and determine kind
                var source = Engine.CreateScriptSourceFromString(script, SourceCodeKind.Expression);
                compiledCode = (CompiledCode)ExecuteSafely(() => source.Compile(DefaultCompilerOptions));
                CodeCache.Add(script, compiledCode);
            }

            return compiledCode;
        }

        public object ExecuteWithResult(CompiledCode compiledCode, object[] values)
        {
            SetVariables(this.ScriptScope, values);
            return ExecuteSafely(() => compiledCode.Execute(this.ScriptScope));
        }

        public object ExecuteWithResult(CompiledCode compiledCode, Dictionary<string,object> values)
        {
            foreach (var pair in values)
            {
                this.ScriptScope.SetVariable(pair.Key, pair.Value);
            }
            return ExecuteSafely(() => compiledCode.Execute(this.ScriptScope));
        }

        public void SetVariables(ScriptScope scope, object[] values)
        {
            if (values == null)
                return;

            for (int i = 0; i < values.Length; i++)
                SetVariable(scope, VariablePrefix + i, values[i]);
        }

        private static object ExecuteSafely(Func<object> executionBlock)
        {
            return executionBlock();
        }

        private static string GetPath(string fileName)
        {
            if (Application.Current == null || File.Exists(fileName))
                return fileName;

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }

        private static void LoadAssembliesIntoEngine()
        {
            var domain = AppDomain.CurrentDomain;
            if (domain == null) return;

            var runtime = Engine.Runtime;
            var assemblies = domain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                runtime.LoadAssembly(assembly);
            }
        }

        #endregion
    }
}
