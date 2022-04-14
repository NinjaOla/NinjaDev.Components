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

        public RenderFragment CreateComponents() => builder =>
        {
            var proList = typeof(TItem).GetProperties();

            foreach (var propertyInfo in proList)
            {
                var labelName = propertyInfo.Name;
                var propertyTpeName = propertyInfo.PropertyType.Name;
                var propertyValue = propertyInfo.GetValue(Model);
                //div around each property
                builder.OpenElement(0, "div");
                builder.AddAttribute(0, "class", "form-group");
                builder.OpenElement(2, "label");
                builder.AddContent(3, labelName + ": ");
                builder.CloseElement();
                var constant = System.Linq.Expressions.Expression.Constant(Model, typeof(TItem));
                var exp = System.Linq.Expressions.MemberExpression.Property(constant, propertyInfo.Name);
                if (propertyInfo.PropertyType.IsEnum)
                {
                    var instantValue = BindConverter.FormatValue(propertyValue);
                    var enumValues = Enum.GetValues(propertyInfo.PropertyType);
                    var underlying = Enum.GetUnderlyingType(propertyInfo.PropertyType);

                    for (int i = 0; i < enumValues.Length; i++)
                    {
                        var enumValue = enumValues.GetValue(i);
                        builder.OpenElement(0, "input");
                        builder.AddAttribute(3, "type", "radio");
                        builder.AddAttribute(4, "name", labelName);
                        builder.AddAttribute(5, "value", BindConverter.FormatValue(enumValue));
                        builder.AddAttribute(6, "checked", instantValue?.Equals(BindConverter.FormatValue(enumValue)));
                        builder.AddAttribute(7, "onchange", EventCallback.Factory.Create<ChangeEventArgs>(this, __value =>
                        {
                            Setter(propertyInfo, enumValue);
                        }));
                        builder.CloseElement();
                    }

                    for (int i = 0; i < enumValues.Length; i++)
                    {

                    }


                }
                else if (propertyInfo.PropertyType.IsArray)
                {
                    //might cast to list and handle that.

                }
                switch (propertyTpeName)
                {
                    case nameof(String):
                        builder.OpenComponent(0, typeof(InputText));
                        // Create the handler for ValueChanged.
                        builder.AddAttribute(3, "ValueChanged", EventCallback.Factory.Create<String>(this, EventCallback.Factory.CreateInferred(this, __value => propertyInfo.SetValue(Model, __value), (string)propertyInfo.GetValue(Model))));
                        builder.AddAttribute(4, "ValueExpression", System.Linq.Expressions.Expression.Lambda<Func<string>>(exp));
                        builder.AddAttribute(5, "value", BindConverter.FormatValue(propertyValue));
                        builder.AddAttribute(6, "oninput", EventCallback.Factory.CreateBinder<string>(this, __value => { Setter(propertyInfo, __value); }, (string)propertyValue));
                        builder.CloseElement();
                        break;
                    case nameof(Boolean):
                        builder.OpenElement(4, "input");
                        builder.AddAttribute(5, "type", "checkbox");
                        builder.AddAttribute(6, "checked", BindConverter.FormatValue((bool)propertyValue));
                        builder.AddAttribute(7, "onchange", EventCallback.Factory.CreateBinder<bool>(this, __value => { Setter(propertyInfo, __value); }, (bool)propertyValue));
                        builder.CloseElement();
                        break;
                    case nameof(Int64):
                    case nameof(UInt64):
                        builder.OpenElement(4, "input");
                        builder.AddAttribute(5, "value", BindConverter.FormatValue(propertyValue));
                        builder.AddAttribute(5, "oninput", EventCallback.Factory.CreateBinder<long>(this, __value => { Setter(propertyInfo, __value); }, (long)propertyValue));
                        builder.CloseElement();
                        break;
                    case nameof(Int32):
                    case nameof(UInt32):

                        builder.OpenElement(4, "input");
                        builder.AddAttribute(5, "value", BindConverter.FormatValue(propertyValue));
                        builder.AddAttribute(5, "oninput", EventCallback.Factory.CreateBinder<int>(this, __value => { Setter(propertyInfo, __value); }, (int)propertyValue));
                        builder.CloseElement();
                        break;
                    case nameof(Int16):
                    case nameof(UInt16):

                        builder.OpenElement(4, "input");
                        builder.AddAttribute(5, "value", BindConverter.FormatValue(propertyValue));
                        builder.AddAttribute(5, "oninput", EventCallback.Factory.CreateBinder<short>(this, __value => { Setter(propertyInfo, __value); }, (short)propertyValue));
                        builder.CloseElement();
                        break;
                    case nameof(Double):
                        builder.OpenElement(4, "input");
                        builder.AddAttribute(5, "value", BindConverter.FormatValue(propertyValue));
                        builder.AddAttribute(5, "oninput", EventCallback.Factory.CreateBinder<double>(this, __value => { Setter(propertyInfo, __value); }, (double)propertyValue));
                        builder.CloseElement();
                        break;
                    default:
                        break;
                }

                builder.CloseElement();
            }
        };

        void FormSubmitted(EditContext editContext)
        {
            bool formIsValid = editContext.Validate();
        }
    }
}
