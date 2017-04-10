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
        //public object MyTest;

        /// <summary>
        ///     Modifies the Value of calling member or the named property given. The method raises the INotifyPropertyChanged event with the used proprty name.
        /// </summary>
        /// <param name="newValue">The new value to be set.</param>
        /// <param name="propertyName">The name of the property to be set. If not declared specificly the callers member name wil lbe used.</param>
        /// <returns>true if the value was really changed.</returns>
        //protected bool ChangeValue(object newValue, [CallerMemberName] String propertyName = "") {
        //    bool retVal = false;
        //    // dotnetstd 1.4:
        //    var pi = this.GetType().GetTypeInfo().GetDeclaredProperty(propertyName);
        //    object oldValue = pi?.GetValue(this);
        //    if (((oldValue == null) && (newValue != null)) ||
        //          ((oldValue != null) && !oldValue.Equals(newValue)))
        //    {
        //        var fis = this.GetType().GetTypeInfo().DeclaredFields;
        //        FieldInfo fi  = fis.Where(fisrec => fisrec.Name == "_" + propertyName).FirstOrDefault();
                
        //        if (fi != null)
        //        {
        //            fi.SetValue(this, newValue);
        //            retVal = true;
        //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //        }

        //        // code for netstd >= 1.5 (.NET >= 4.6.2) but not for netstd 2.0!!! -> you have to stick with 4.6.1 for that !!!! see : https://blogs.msdn.microsoft.com/dotnet/2016/09/26/introducing-net-standard/
        //        // PropertyInfo propertyInfo = this.GetType().GetProperty(propertyName);
        //        //object oldValue = propertyInfo?.GetValue(this);

        //        //if(((oldValue == null) && (newValue != null)) ||
        //        //      ((oldValue != null) && !oldValue.Equals(newValue))) {
        //        //    FieldInfo fi = this.GetType().GetField("_" + propertyName, BindingFlags.NonPublic | BindingFlags.Instance);

        //        //   if(fi != null) {
        //        //        fi.SetValue(this, newValue);
        //        //        retVal = true;
        //        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //        //    }


        //    }
        //    return retVal;
        //}

        //public void InvokePropertyChanged(string propertyName) {
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        /// <summary>
        ///     Allows to invoke(trigger) the PropertyChanged event for the specified member property.
        /// </summary>
        /// <typeparam name="T">the type of the changed Member property</typeparam>
        /// <param name="propFieldExpression">Expression in form of '() => nameOfProp' to specify the changed member property.</param>
        public void InovokePropertyChanged<T>(Expression<Func<T>> propFieldExpression) {
            MemberExpression MemberProperty = propFieldExpression?.Body as MemberExpression;
            string propName = MemberProperty?.Member?.Name;
            if (!String.IsNullOrEmpty(propName))
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            }
        }

        /// <summary>
        ///     Use in Property Setters only. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newValue">pass in the new value od the property</param>
        /// <param name="warppedFieldExpression">Expression to point to backing Field. if ommitted a field with name "_<propNamen>" is expected !</propNamen>  </param>
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
       
        //private string GenerateAccessCode(Expression expression)
        //{
        //    string retStr = String.Empty;
        //    MemberExpression me = expression as MemberExpression;
        //    if (me != null)
        //    {
        //       // PropertyInfo pi = me.Member as PropertyInfo;

        //        // Todo....
        //        retStr += me.Member.Name;
        //        if (me.Expression != null)
        //        {
        //            string parent = GenerateAccessCode(me.Expression);
        //            if (!String.IsNullOrEmpty(parent))
        //            {
        //                retStr = parent + "." + retStr;
        //            }
        //        }
        //    }
        //    return retStr;
        //}

        //private void SetProperty(string compoundProperty, object target, object value)
        //{
        //    string[] bits = compoundProperty.Split('.');
        //    for (int i = 0; i < bits.Length - 1; i++)
        //    {
        //        PropertyInfo propertyToGet = target.GetType().GetTypeInfo().GetDeclaredProperty(bits[i]);
        //        if (propertyToGet == null)
        //        {
        //            FieldInfo fieldToGet = target.GetType().GetTypeInfo().GetDeclaredField(bits[i]);
        //            target = fieldToGet.GetValue(target);
        //        }
        //        else
        //        {
        //            target = propertyToGet.GetValue(target, null);
        //        }
        //        if (target == null)
        //        {
        //            return;
        //        }
        //    }
        //    PropertyInfo propertyToSet = target.GetType().GetTypeInfo().GetDeclaredProperty(bits.Last());
        //    if (propertyToSet != null)
        //    {
        //        propertyToSet.SetValue(target, value, null);
        //    } else
        //    {
        //        FieldInfo fieldToSet = target.GetType().GetTypeInfo().GetDeclaredField(bits.Last());
        //        fieldToSet?.SetValue(target, value);
        //    }
        //}
    }
}
