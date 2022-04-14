using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.CompilerServices;
using System.Collections;
using System.Reflection;

namespace NinjaDev.Components.Blazor
{
    public partial class NDEditForm<TItem>
    {

        [Parameter]
        public TItem Model { get; set; }

        [Parameter]
        public EventCallback<TItem> ModelChanged { get; set; }

        [Parameter]
        public bool DisplaySubmit { get; set; } = true;

        void ChangeModel() => ModelChanged.InvokeAsync(Model);

        void Setter(PropertyInfo property, object value)
        {
            property.SetValue(Model, value);
            ChangeModel();
        }

        bool IsFlagsEnum(Type t)
        {
            var attr = t.GetCustomAttributes(typeof(FlagsAttribute), true).FirstOrDefault();
            var result = attr != null;
            return result;
        }

        public IEnumerable<NDEditProperty> CreateComponents()
        {
            var proList = typeof(TItem).GetProperties();

            foreach (var propertyInfo in proList)
            {

                var constant = System.Linq.Expressions.Expression.Constant(Model, typeof(TItem));
                var exp = System.Linq.Expressions.MemberExpression.Property(constant, propertyInfo.Name);

                NDEditProperty editModel = new();
                editModel.LabelText = propertyInfo.Name;
                editModel.Value = propertyInfo.GetValue(Model);
                editModel.Type = propertyInfo.PropertyType;
                editModel.PropertyInfo = propertyInfo;
                editModel.Callback = Setter;
                editModel.Expresseion = exp;
                editModel.ComponentParameters = new Dictionary<string, object>();

                if (propertyInfo.PropertyType.IsEnum)
                {
                    //var instantValue = BindConverter.FormatValue(propertyValue);
                    //var enumValues = Enum.GetValues(propertyInfo.PropertyType);
                    //var underlying = Enum.GetUnderlyingType(propertyInfo.PropertyType);
                }
                else if (propertyInfo.PropertyType.IsArray)
                {

                }
                switch (editModel.Type.Name)
                {
                    case nameof(String):
                        editModel.ComponentParameters.Add("ValueExpression", System.Linq.Expressions.Expression.Lambda<Func<string>>(exp));
                        editModel.ComponentParameters.Add("oninput", EventCallback.Factory.CreateBinder<string>(this, __value => { Setter(propertyInfo, __value); }, (string)editModel.Value));
                        editModel.ComponentParameters.Add("Value", (string)editModel.Value);
                        editModel.ComponentType = typeof(InputText);
                        break;
                    case nameof(Boolean):
                        editModel.ComponentParameters.Add("ValueExpression", System.Linq.Expressions.Expression.Lambda<Func<bool>>(exp));
                        editModel.ComponentParameters.Add("onchange", EventCallback.Factory.CreateBinder<bool>(this, __value => { Setter(propertyInfo, __value); }, (bool)editModel.Value));
                        editModel.ComponentParameters.Add("Value", (bool)editModel.Value);
                        editModel.ComponentParameters.Add("checked", BindConverter.FormatValue((bool)editModel.Value));
                        editModel.ComponentType = typeof(InputCheckbox);
                        break;
                    case nameof(Int64):
                    case nameof(UInt64):
                        editModel.ComponentParameters.Add("ValueExpression", System.Linq.Expressions.Expression.Lambda<Func<long>>(exp));
                        editModel.ComponentParameters.Add("onchange", EventCallback.Factory.CreateBinder<long>(this, __value => { Setter(propertyInfo, __value); }, (long)editModel.Value));
                        editModel.ComponentParameters.Add("Value", editModel.Value);
                        editModel.ComponentType = typeof(InputNumber<long>);
                        break;
                    case nameof(Int32):
                    case nameof(UInt32):
                        editModel.ComponentParameters.Add("ValueExpression", System.Linq.Expressions.Expression.Lambda<Func<int>>(exp));
                        editModel.ComponentParameters.Add("onchange", EventCallback.Factory.CreateBinder<int>(this, __value => { Setter(propertyInfo, __value); }, (int)editModel.Value));
                        editModel.ComponentParameters.Add("Value", editModel.Value);
                        editModel.ComponentType = typeof(InputNumber<int>);
                        break;
                    case nameof(Int16):
                    case nameof(UInt16):
                        editModel.ComponentParameters.Add("ValueExpression", System.Linq.Expressions.Expression.Lambda<Func<short>>(exp));
                        editModel.ComponentParameters.Add("onchange", EventCallback.Factory.CreateBinder<short>(this, __value => { Setter(propertyInfo, __value); }, (short)editModel.Value));
                        editModel.ComponentParameters.Add("Value", editModel.Value);
                        editModel.ComponentType = typeof(InputNumber<short>);
                        break;
                    case nameof(Double):
                        editModel.ComponentParameters.Add("ValueExpression", System.Linq.Expressions.Expression.Lambda<Func<double>>(exp));
                        editModel.ComponentParameters.Add("onchange", EventCallback.Factory.CreateBinder<double>(this, __value => { Setter(propertyInfo, __value); }, (double)editModel.Value));
                        editModel.ComponentParameters.Add("Value", editModel.Value);
                        editModel.ComponentType = typeof(InputNumber<double>);
                        break;
                    default:
                        break;
                }

                yield return editModel;
            }
        }

        void FormSubmitted(EditContext editContext)
        {
            bool formIsValid = editContext.Validate();
        }
    }

    public class NDEditProperty
    {
        public Type Type { get; set; }
        public string LabelText { get; set; }
        public Action<PropertyInfo, object> Callback { get; set; }
        public object Value { get; set; }
        public PropertyInfo PropertyInfo { get; internal set; }
        public System.Linq.Expressions.MemberExpression Expresseion { get; internal set; }
        public Dictionary<string, object> ComponentParameters { get; set; }
        public Type ComponentType { get; set; }
    }
}
