using System.Windows.Data;
using Microsoft.Scripting.Hosting;

namespace PyBinding
{
    public interface IPyBinding
    {
        bool UnsetValueIsInvalid { get; set; }
        CompiledCode CompiledCode { get; }
    }

    public class PyBinding : MultiBinding, IPyBinding
    {
        #region Fields

        private string _script;

        #endregion

        #region Constructor

        static PyBinding()
        {
            ScriptConverter = new ScriptConverter();
        }

        public PyBinding()
        {
            this.Mode = BindingMode.OneWay;
            this.StringFormat = "{0}"; // This is required for some reason.  Maybe type converters aren't picking it up?
        }

        public PyBinding(string script)
            : this()
        {
            this.Script = script;
        }

        #endregion

        #region Properties

        private static IMultiValueConverter ScriptConverter { get; set; }

        public bool UnsetValueIsInvalid { get; set; }

        public CompiledCode CompiledCode { get; private set; }

        public string Script
        {
            get { return _script; }
            set
            {
                _script = string.Intern(value);
                BuildBindingFromScript();
            }
        }

        #endregion

        #region Methods

        private void BuildBindingFromScript()
        {
            if (string.IsNullOrEmpty(_script))
                return;

            var bindingPaths = BindingPathParser.GetUniqueMarkupBrackets(_script);

            foreach (var bindingPath in bindingPaths)
            {
                var intermediateBinding = BindingPathParser.BuildBinding(bindingPath);
                this.Bindings.Add(intermediateBinding);
            }

            string finalScript = BindingPathParser.SetupPythonScriptWithReplacement(_script, PythonEvaluator.VariablePrefix);
            this.CompiledCode = PythonEvaluator.Compile(finalScript);
            this.ConverterParameter = this;
            this.Converter = ScriptConverter;
        }

        #endregion
    }
}
