using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFLib.Contracts.Validation
{
    public interface ICustomValidator<T>
    {
        bool IsValid(T obj);
        IEnumerable<string> BrokenRules(T obj);
    }

    //По сути - маркер
    public interface IValidatable<T>
    {
        bool CustomValidate(ICustomValidator<T> validator, out IEnumerable<string> brokenRules);
    }

    public static class Validator
    {
        private static Dictionary<Type, object> _validators = new Dictionary<Type, object>();

        public static void RegisterValidatorFor<T>(T entity, ICustomValidator<T> validator)
            where T : IValidatable<T>
        {
            _validators.Add(entity.GetType(), validator);
        }

        public static void RegisterValidator<T>(ICustomValidator<T> validator)
            where T : IValidatable<T>
        {
            _validators.Add(typeof(T), validator);
        }

        public static ICustomValidator<T> GetValidatorFor<T>(T entity)
            where T : IValidatable<T>
        {
            Type entityType = entity.GetType();
            if (!_validators.ContainsKey(entityType) && entityType.BaseType!=null && _validators.ContainsKey(entityType.BaseType))
                return _validators[entityType.BaseType] as ICustomValidator<T>; //Принцип Лисковой -поможет с EF
            return _validators[entity.GetType()] as ICustomValidator<T>;
        }

        public static bool Validate<T>(this T entity, out IEnumerable<string> brokenRules)
            where T : IValidatable<T>
        {
            ICustomValidator<T> validator = Validator.GetValidatorFor(entity);

            return entity.CustomValidate(validator, out brokenRules);
        }
    }
}
