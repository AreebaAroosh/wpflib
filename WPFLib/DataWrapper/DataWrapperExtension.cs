using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;
using PyBinding;
using System.Windows.Controls;
using WPFLib.MarkupExtensionHelpers;
using WPFLib.DataWrapper;

namespace WPFLib
{
    public class DataWrapperExtension : MarkupExtensionBase
    {
        public string Dependent { get; set; }
        public string DependsOn { get; set; }

        static PythonEvaluator Evaluator = new PythonEvaluator();
        public string Rule { get; set; }

        Func<object, ValidationResult> pyRule;
        Func<object, ValidationResult> PyRule
        {
            get
            {
                if (!String.IsNullOrEmpty(Rule) && pyRule == null)
                {
                    pyRule = CreatePyRule();
                }
                return pyRule;
            }
        }

        protected Func<object, ValidationResult> CreatePyRule()
        {
            if (!String.IsNullOrEmpty(Rule))
            {
                var compiledCode = PythonEvaluator.Compile(Rule);
                Func<object, ValidationResult> rule = (context) =>
                {
                    var d = new Dictionary<string, object>();
                    d["this"] = context;
                    var res = (string)Evaluator.ExecuteWithResult(compiledCode, d);
                    return new ValidationResult(String.IsNullOrEmpty(res), res);
                };
                return rule;
            }
            return null;
        }

        //string FirstLevelPathProperty
        //{
        //    get
        //    {
        //        var path = this.binding.Path.Path;
        //        var dot = path.IndexOf('.');
        //        if (dot > 0)
        //        {
        //            return path.Substring(0, dot);
        //        }
        //        return path;
        //    }
        //}

        Binding binding = new Binding();

        [DefaultValue(UpdateSourceTrigger.Default)]
        public UpdateSourceTrigger UpdateSourceTrigger
        {
            get { return binding.UpdateSourceTrigger; }
            set { binding.UpdateSourceTrigger = value; }
        }

        [DefaultValue(null)]
        public virtual PropertyPath Path
        {
            get { return binding.Path; }
            set { binding.Path = value; }
        }

        public DataWrapperExtension(string path)
        {
            this.Path = new PropertyPath(path);
        }

        protected override object ProvideValue(IServiceProvider serviceProvider, FrameworkElement targetElement, DependencyProperty targetProperty)
        {
            if (WPFHelper.IsInDesignMode)
            {
                return this.binding.ProvideValue(serviceProvider);
            }

            var manager = new DataWrapperAttachedManager(Path.Path, this.binding, targetElement, targetProperty, PyRule, Dependent, DependsOn);

            var metaData = targetProperty.GetMetadata(targetElement);
            return metaData.DefaultValue;
        }
    }
}
