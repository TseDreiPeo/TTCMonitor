using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MMVVM {
    /// <summary>
    ///     Baseclass able to raise INotifyPropertyChanged when proprty is modified by using ChangeValue() Method.
    /// </summary>
    public class ObservableObject : INotifyPropertyChanged {

        /// <summary>
        ///     Implements the INotifyPropertyChanged event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Allows to invoke(trigger) the PropertyChanged event for the specified member property.
        /// </summary>
        /// <typeparam name="T">the type of the changed Member property</typeparam>
        /// <param name="propFieldExpression">Expression in form of '() => nameOfProp' to specify the changed member property.</param>
        protected void InovokePropertyChanged<T>(Expression<Func<T>> propFieldExpression) {
            MemberExpression MemberProperty = propFieldExpression?.Body as MemberExpression;
            string propName = MemberProperty?.Member?.Name;
            if (!String.IsNullOrEmpty(propName))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }

        /// <summary>
        ///     Use in Property Setters only. Usage Variants: .....
        ///     .....
        ///     .....
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newValue">pass in the new value of the property</param>
        /// <param name="warppedFieldExpression">Expression to point to backing Field. if ommitted a field with name "_propName" is expected !  </param>
        /// <param name="propertyHolder">The Object reference holding the backing field. If omitted then "this." is asumend.</param>
        /// <param name="propertyName">leave allone the property name is compiler detected.</param>
        /// <returns></returns>
        protected bool ChangeValue<T>(T newValue, Expression<Func<T>> warppedFieldExpression = null, object propertyHolder = null, [CallerMemberName] String propertyName = "")
        {
            bool retVal = false;
            // dotnetstd 1.4:
            var vmPi = this.GetType().GetTypeInfo().GetDeclaredProperty(propertyName);
            object oldValue = vmPi?.GetValue(this);
            if (((oldValue == null) && (newValue != null)) ||
                  ((oldValue != null) && !oldValue.Equals(newValue)))
            {
                FieldInfo fi = null;
                PropertyInfo pi = null;

                if (warppedFieldExpression == null)
                {
                    var fis = this.GetType().GetTypeInfo().DeclaredFields;
                    fi = fis.Where(fisrec => fisrec.Name == "_" + propertyName).FirstOrDefault();
                } else
                {
                    MemberExpression wrappedProperty = warppedFieldExpression.Body as MemberExpression;
                    fi = wrappedProperty.Member as FieldInfo;
                    pi = wrappedProperty.Member as PropertyInfo;
                }

                if (fi != null)
                {
                    fi.SetValue(propertyHolder ?? this, newValue);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                    retVal = true;
                }
                if (pi != null)
                {
                    if (pi.DeclaringType == propertyHolder?.GetType())
                    {
                        pi.SetValue(propertyHolder, newValue);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                        retVal = true;
                    } else if (pi.DeclaringType == this.GetType())
                    {
                        pi.SetValue(this, newValue);
                        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                        retVal = true;
                    }
                }
            }
            return retVal;
        }
     
    }
}
