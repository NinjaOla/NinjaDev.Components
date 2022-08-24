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
    internal class NDEditPropertyArray<TModel, TList, TListValue> : NDEditPropertyBase<TModel>, INDEditProperty, ICanSetComponentInfo
        where TList : IEnumerable<TListValue>
    {
        protected bool _renderSubList;
        protected IChangeModel<TModel> Parent { get; }
        public TList InputList { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="propertyInfo"></param>
        /// <param name="parent"></param>
        /// <param name="renderSubList">Render the sublist if any exists</param>
        public NDEditPropertyArray(TModel model, TList inputList, PropertyInfo propertyInfo, IChangeModel<TModel> parent, bool renderSubList = false) : base(model, propertyInfo)
        {
            _renderSubList = renderSubList;
            Parent = parent;
            InputList = inputList;
        }

        public void SetComponentInfo()
        {



            //ComponentParameters.Add("Value", CreateSingleFragment(CreateSelectOptions()));

            //ComponentParameters.Add("ValueExpression", System.Linq.Expressions.Expression.Lambda<Func<TModel>>(Expression));
            //ComponentParameters.Add("ValueChanged", CreateEventCallback<TModel>());

            //if ()

            //    ComponentParameters.Add("ChildContent", CreateSingleFragment(CreateSelectOptions()));

            //ComponentType = typeof(InputSelect<TEnum>);
        }


        /// <summary>
        /// Is it possible to create infinite lists in html ? 
        /// </summary>
        /// <returns></returns>
        /*private ListFragment CreateList()
        {
            var fragments = new List<RenderFragment>();
            foreach (var value in _model)
            {
                if (value != null)
                {
                    fragments.Add(CreateSelectOption(value, value.ToString()));
                }
            }

            return fragments;
        }*/

        private RenderFragment CreateSelectOptions(object value, string displayText) => builder =>
        {
            builder.OpenComponent(0, typeof(InputText));
            builder.OpenElement(0, "option");
            builder.AddAttribute(1, "value", value);
            builder.AddContent(2, displayText);
            builder.CloseElement();
        };
        private EventCallback<TValue> CreateEventCallback<TValue>()
        {

            return EventCallback.Factory.Create<TValue>(this, (e) => {
                Value = e;
            });

        }

        public override void OnChange()
        {

        }

        public override RenderFragment Render() => builder  => {
            var buttonCb = EventCallback.Factory.Create(this, (ee) =>
            {
                var x = "";
            });
            foreach (var item in InputList)
            {
                builder.OpenElement(0, "p");
                builder.AddContent(1, item.ToString());
                builder.CloseElement();
            }
            builder.OpenElement(55, "button");
            builder.AddAttribute(56, "onclick", buttonCb);
            builder.AddAttribute(57, "type", "button");
            builder.AddAttribute(58, "class", "btn btn-primary");
            builder.AddContent(59, "Add new item");

            builder.CloseElement();

        };

    }

    class ListFragment : ListFragment<ListFragment>
    {

    }

    class ListFragment<T>
    {
        IEnumerable<T> Values { get; set; }
    }

    class ListElement
    {
        object Value { get; set; }
        string DisplayText { get; set; }
    }
}
