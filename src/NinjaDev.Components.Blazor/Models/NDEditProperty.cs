using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NinjaDev.Components.Blazor.Models
{
    internal class NDEditProperty<TModel> : NDEditPropertyBase<TModel>
    {
        public override Dictionary<string, object> ComponentParameters { get; } = new Dictionary<string, object>();
        public override Type ComponentType { get; internal set; }
       
        public NDEditProperty(TModel model, PropertyInfo propertyInfo, IChangeModel<TModel> parent) : base(model, propertyInfo, parent)
        {
        
        }


        internal void SetComponentInfo<TValue>(bool isIncludeOnInput = true)
        {
            ComponentParameters.Add("ValueExpression", System.Linq.Expressions.Expression.Lambda<Func<TValue>>(Expression));
            ComponentParameters.Add("Value", Value);
            if (isIncludeOnInput)
            {
                ComponentParameters.Add("oninput", CreateEventCallback<TValue>());
            }
            else
            {
                ComponentParameters.Add("onchange", CreateEventCallback<TValue>());
            }
        }

        private EventCallback<ChangeEventArgs> CreateEventCallback<TValue>()
        {
            return EventCallback.Factory.Create(this, (e) => {

                if (e is ChangeEventArgs)
                {
                    Value = Convert.ChangeType(e.Value, typeof(TValue));
                }
                else
                {
                    Value = Convert.ChangeType(e, typeof(TValue));
                }
            });
        }
    }
}
