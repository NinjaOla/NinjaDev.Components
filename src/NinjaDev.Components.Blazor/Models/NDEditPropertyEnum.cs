using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NinjaDev.Components.Blazor.Models
{
    internal class NDEditPropertyEnum<TModel, TEnum> : NDEditPropertyBase<TModel>, INDEditProperty, ICanSetComponentInfo
    {

        public NDEditPropertyEnum(TModel model, PropertyInfo propertyInfo, IChangeModel<TModel> parent) : base(model, propertyInfo, parent)
        {

        }

        public void SetComponentInfo()
        {
            ComponentParameters.Add("Value", Value);

            ComponentParameters.Add("ValueExpression", System.Linq.Expressions.Expression.Lambda<Func<TEnum>>(Expression));
            ComponentParameters.Add("ValueChanged", CreateEventCallback<TEnum>());

            ComponentParameters.Add("ChildContent", CreateSingleFragment(CreateSelectOptions()));

            ComponentType = typeof(InputSelect<TEnum>);
        }

        private List<RenderFragment> CreateSelectOptions()
        {
            var fragments = new List<RenderFragment>();
            foreach (var value in Enum.GetValues(_propertyInfo.PropertyType))
            {
                if (value != null)
                {
                    fragments.Add(CreateSelectOption(value, value.ToString()));
                }
            }

            return fragments;
        }

        private RenderFragment CreateSelectOption(object value, string displayText) => builder =>
        {
            builder.OpenElement(0, "option");
            builder.AddAttribute(1, "value", value);
            builder.AddContent(2, displayText);
            builder.CloseElement();
        };

        private EventCallback<TEnum> CreateEventCallback<TValue>()
        {

            return EventCallback.Factory.Create<TEnum>(this, (e) => {

                Value = e;
            });

        }
    }
}
