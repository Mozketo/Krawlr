using System;
using System.Collections.Generic;

namespace Krawlr.Core.Extensions
{
    public interface IOption
    {
        bool IsSome { get; }
        bool IsNone { get; }
        object Value { get; }
    }

    /// <summary>
    /// (IS) Option class: similar to the Nullable class with the exception that it does not have value type constraint on its TValue generic argument.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    [System.Diagnostics.DebuggerDisplay("{DebuggerDisplay}")]
    public class Option<TValue> : IOption
    {
        /// <summary>
        /// Creates Option instance which contains the given value
        /// </summary>
        /// <param name="value"></param>
        public Option(TValue value)
        {
            _Value = value;
            _IsSome = true;
        }
        private Option()
        {
            _IsSome = false;
        }
        private bool _IsSome;
        /// <summary>
        /// returns true if the option contains a value
        /// </summary>
        public bool IsSome
        {
            get { return _IsSome; }
        }
        private TValue _Value;
        /// <summary>
        /// returns the value of the option if its IsSome is true, otherwise throws exception.
        /// </summary>
        public TValue Value
        {
            get
            {
                if (this.IsNone)
                    throw new Exception("Attempt is made to retrieve value from the None option.");
                return _Value;
            }
        }
        /// <summary>
        /// returns true if the option does not have a value
        /// </summary>
        public bool IsNone
        {
            get { return !_IsSome; }
        }
        /// <summary>
        /// returns the value of the option if its IsSome is true, otherwise returns default value of the value type.
        /// </summary>
        /// <returns>option's value or default value is the option doesnt have a value</returns>
        public TValue GetValueOrDefault()
        {
            return this.GetValue(() => default(TValue));
        }
        /// <summary>
        /// returns option's value, or calls the defaultValueGetter to get one if the option doesnt have a value
        /// </summary>
        /// <param name="defautlValueGetter">function to call to get a value if the option doesnt have a value</param>
        /// <returns>option's value if the option is Some, or value returned by the defaultValueGetter otherwise</returns>
        public TValue GetValue(Func<TValue> defautlValueGetter)
        {
            if (this.IsSome)
                return this.Value;
            else
                return defautlValueGetter();
        }
        /// <summary>
        /// maps this option to another option through the selector function if the option has a value, None otherwise.
        /// </summary>
        /// <typeparam name="TValue2">type of the projection</typeparam>
        /// <param name="selector">map function</param>
        /// <returns>mapped through the selector value, or None if this option doesnt have a value</returns>
        public Option<TValue2> Select<TValue2>(Func<TValue, Option<TValue2>> selector)
        {
            if (this.IsSome)
                return selector(this.Value);
            else
                return Option<TValue2>.None;
        }
        /// <summary>
        /// calls the action function on option's value if the option has a value, othrwise does nothing
        /// </summary>
        /// <param name="action">function to call on the option's value</param>
        public void Iter(Action<TValue> action)
        {
            if (this.IsSome)
                action(this.Value);
        }
        internal static Option<TValue> _None = new Option<TValue>();
        /// <summary>
        /// returns option which does not have a value
        /// </summary>
        public static Option<TValue> None
        {
            get
            {
                return _None;
            }
        }
        public IEnumerable<TValue> AsEnumerable()
        {
            if (this.IsSome)
                yield return this.Value;
        }
        public override bool Equals(object obj)
        {
            var other = obj as Option<TValue>;
            return other != null && (this.IsNone && other.IsNone || this.IsSome && other.IsSome && object.Equals(this.Value, other.Value));
        }
        public override int GetHashCode()
        {
            if (this.IsNone)
                return -1;

            var o = (object)this.Value;
            if (o == null)
                return 0;

            return o.GetHashCode();
        }
        #region IOption Members

        bool IOption.IsSome
        {
            get { return this.IsSome; }
        }

        bool IOption.IsNone
        {
            get { return this.IsNone; }
        }

        object IOption.Value
        {
            get { return this.Value; }
        }

        #endregion
        public override string ToString()
        {
            if (this.IsNone)
                return string.Format("None<{0}>", typeof(TValue).Name);

            var valueAsString = (object)this.Value == null ? "null" : typeof(TValue) == typeof(string) ? "\"" + this.Value.ToString() + "\"" : this.Value.ToString();
            return string.Format("Some<{0}>({1})", typeof(TValue).Name, valueAsString);
        }
        public string DebuggerDisplay
        {
            get
            {
                return this.ToString();
            }
        }
    }

    /// <summary>
    /// (IS) contains Option related helper functions
    /// </summary>
    public static class Option
    {
        /// <summary>
        /// (IS) creates option which contains the given value
        /// </summary>
        /// <typeparam name="TValue">value type</typeparam>
        /// <param name="value">value for the option</param>
        /// <returns>option which contains the given value</returns>
        public static Option<TValue> Some<TValue>(TValue value)
        {
            return new Option<TValue>(value);
        }
        /// <summary>
        /// (IS) creates an option from the referecne type value; if the value is null, creates None, otherwise Some(referenceValue)
        /// </summary>
        /// <typeparam name="T">value type</typeparam>
        /// <param name="referenceValue">value to create an option from</param>
        /// <returns>if the value is null, creates None, otherwise Some(referenceValue)</returns>
        public static Option<T> OfReference<T>(T referenceValue)
        {
            return referenceValue == null ? Option<T>.None : Some(referenceValue);
        }
        public static bool IsSome<T>(Option<T> option)
        {
            return option.IsSome;
        }
        public static T ValueOf<T>(Option<T> option)
        {
            return option.Value;
        }
    }
}
