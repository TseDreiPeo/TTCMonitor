using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BeaconMonitor {
    /// <summary>
    ///     Baseclase able to raise INotifyPropertyChanged when proprty is modified by using ChangeValue() Method.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        //public object MyTest;

        /// <summary>
        ///     Modifies the Value of calling member or the named property given. The method raises the INotifyPropertyChanged event with the used proprty name.
        /// </summary>
        /// <param name="newValue">The new value to be set.</param>
        /// <param name="propertyName">The name of the property to be set. If not declared specificly the callers member name wil lbe used.</param>
        /// <returns>true if the value was really changed.</returns>
        protected bool ChangeValue(object newValue, [CallerMemberName] String propertyName = "") {
            bool retVal = false;

            PropertyInfo propertyInfo = this.GetType().GetProperty(propertyName);
            object oldValue = propertyInfo?.GetValue(this);

            if(((oldValue == null) && (newValue != null)) ||
                  ((oldValue != null) && !oldValue.Equals(newValue))) {
                FieldInfo fi = this.GetType().GetField("_" + propertyName, BindingFlags.NonPublic | BindingFlags.Instance);

                if(fi != null) {
                    fi.SetValue(this, newValue);
                    retVal = true;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

                }
            }
            return retVal;
        }

        //public void InvokePropertyChanged(string propertyName) {
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        public void InovokePropertyChanged<T>(Expression<Func<T>> propFieldExpression) {
            MemberExpression MemberProperty = propFieldExpression.Body as MemberExpression;
            string propName = MemberProperty.Member.Name;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }


    }
}
