using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NinjaDev.Components.Blazor
{

    public interface IChangeModel<T>{
        void OnChangeModel(T Model);
    }
    public partial class NDDynamicComponent<T> : IChangeModel<T>
    {

        [Parameter]
        public T Model { get; set; }

        [Parameter]
        public EventCallback<T> ModelChanged { get; set; }

        [Parameter]
        public EventCallback<T> ChangedCallback { get; set; }

        public void OnChangeModel(T model)
        {
            ModelChanged.InvokeAsync(model);
            ChangedCallback.InvokeAsync(model);
        }

        private Type GenericTypeCreator(Type type1, Type genericType1, Type genericType2)
        {
            Type classType = type1;
            Type[] typeParams = new Type[] { genericType1, genericType2 };
            Type constructedType = classType.MakeGenericType(typeParams);
            return constructedType;
        }




        //https://stackoverflow.com/questions/24471338/activator-createinstance-calling-constructor-with-class-as-parameter/24471737#24471737 
        //TODO: move to helper
        public static object CreateInstance(Type pContext, object[] Params)
        {
            List<Type> argTypes = new List<Type>();
            //used .GetType() method to get the appropriate type
            //Param can be null so handle accordingly
            if (Params != null)
                foreach (object Param in Params)
                {
                    if (Param != null)
                        argTypes.Add(Param.GetType());
                    else
                        argTypes.Add(null);
                }

            ConstructorInfo[] Types = pContext.GetConstructors();
            foreach (ConstructorInfo node in Types)
            {
                ParameterInfo[] Args = node.GetParameters();
                // Params can be null for default constructors so use argTypes
                if (argTypes.Count == Args.Length)
                {
                    bool areTypesCompatible = true;
                    for (int i = 0; i < Params.Length; i++)
                    {
                        if (argTypes[i] == null)
                        {
                            if (Args[i].ParameterType.IsValueType)
                            {
                                //fill the defaults for value type if not supplied
                                Params[i] = CreateInstance(Args[i].ParameterType, null);
                                argTypes[i] = Params[i].GetType();
                            }
                            else
                            {
                                argTypes[i] = Args[i].ParameterType;
                            }
                        }
                        if (!Args[i].ParameterType.IsAssignableFrom(argTypes[i]))
                        {
                            areTypesCompatible = false;
                            break;
                        }
                    }
                    if (areTypesCompatible)
                        return node.Invoke(Params);
                }
            }

            //delegate type to Activator.CreateInstance if unable to find a suitable constructor
            return Activator.CreateInstance(pContext);
        }

        ICanSetComponentInfo CreateEditPropertyEnum(PropertyInfo propertyInfo)
        {

            var constructed = GenericTypeCreator(typeof(NDEditPropertyEnum<,>), typeof(T), propertyInfo.PropertyType);

            return CreateInstance(constructed, new object[] { Model, propertyInfo, this }) as ICanSetComponentInfo;
        }

        public IEnumerable<INDEditProperty> CreateComponents()
        {

            var list = typeof(T).GetProperties();
            foreach (var propertyInfo in typeof(T).GetProperties())
            {
                
                //if (propertyInfo.PropertyType.IsArray)
                //{
                //    editModel.IsRenderSubList = true;
                //}
                //else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().GetInterfaces().Any(i => i.Name == nameof(IList)))
                //{
                   
                //    var x = "";
                //}


                if (propertyInfo.PropertyType.IsEnum)
                {
                    ICanSetComponentInfo variable = CreateEditPropertyEnum(propertyInfo);
                    variable.SetComponentInfo();
                    yield return variable as INDEditProperty;

                }
                NDEditProperty<T> editModel = new NDEditProperty<T>(Model, propertyInfo, this);
                switch (propertyInfo.PropertyType.Name)
                {
                    case nameof(String):
                        editModel.SetComponentInfo<string>();
                        editModel.ComponentType = typeof(InputText);
                        break;
                    case nameof(Boolean):
                        editModel.SetComponentInfo<bool>(false);
                        editModel.ComponentParameters.Add("checked", BindConverter.FormatValue((bool)editModel.Value));
                        editModel.ComponentType = typeof(InputCheckbox);
                        break;
                    case nameof(Int64):
                    case nameof(UInt64):
                        editModel.SetComponentInfo<long>();
                        editModel.ComponentType = typeof(InputNumber<long>);
                        break;
                    case nameof(Int32):
                    case nameof(UInt32):
                        editModel.SetComponentInfo<int>();
                        editModel.ComponentType = typeof(InputNumber<int>);
                        break;
                    case nameof(Int16):
                    case nameof(UInt16):
                        editModel.SetComponentInfo<short>();
                        editModel.ComponentType = typeof(InputNumber<short>);
                        break;
                    case nameof(Double):
                        editModel.SetComponentInfo<double>();
                        editModel.ComponentType = typeof(InputNumber<double>);
                        break;
                    default:
                        break;
                }
                if(editModel.ComponentType != null)
                {
                    yield return editModel;
                }
            }
        }

       

    }
    public interface INDEditProperty
    {
         Dictionary<string, object> ComponentParameters { get; } 
         Type ComponentType { get; }
    }

    public interface ICanSetComponentInfo
    {
        void SetComponentInfo();
    }

    public class NDEditProperty<T> : INDEditProperty
    {
        private PropertyInfo _propertyInfo;
        private T _model;
        private IChangeModel<T> _parent;

        public NDEditProperty(T model, PropertyInfo propertyInfo, IChangeModel<T> parent)
        {
            _propertyInfo = propertyInfo;
            _model = model;
            _parent = parent;

        }
        public Type Type { get { return _propertyInfo.PropertyType; }}

        public Action OnChange { get; private set; }

        private string _labelText;
        public string LabelText
        {
            get { 
                if (string.IsNullOrEmpty(_labelText)){
                    _labelText = _propertyInfo.Name;
                }
                return _labelText;
            }
            set {
                _labelText = value;
            }
        }

        private object _value;
        public object Value {
            get
            {
                if (_value is null)
                {
                    _value = _propertyInfo.GetValue(_model);
                }
                return _value;
            }
            set
            {
                _value = value;
                _propertyInfo.SetValue(_model, value);
                _parent?.OnChangeModel(_model);
            }
        }

        public System.Linq.Expressions.MemberExpression Expression { get

            { return System.Linq.Expressions.MemberExpression.Property(System.Linq.Expressions.Expression.Constant(_model, typeof(T)), _propertyInfo.Name); } }

        public Dictionary<string, object> ComponentParameters { get; } = new Dictionary<string, object>();
        public Type ComponentType { get; set; }
        public bool IsRenderSubList { get; set; }

        private EventCallback<ChangeEventArgs> CreateEventCallback<TValue>()
        {
            return EventCallback.Factory.Create(this, (e) => {

                if(e is ChangeEventArgs)
                {
                    Value = Convert.ChangeType(e.Value, typeof(TValue));
                }
                else
                {
                    Value = Convert.ChangeType(e, typeof(TValue));
                }
            });

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
    }

    public class NDEditPropertyEnum<TModel, TEnum> : INDEditProperty, ICanSetComponentInfo
    {
        public Dictionary<string, object> ComponentParameters { get; } = new Dictionary<string, object>();
        public Type ComponentType { get; private set; }

        private PropertyInfo _propertyInfo;
        private TModel _model;
        private IChangeModel<TModel> _parent;

        public NDEditPropertyEnum(TModel model, PropertyInfo propertyInfo, IChangeModel<TModel> parent)
        {
            _propertyInfo = propertyInfo;
            _model = model;
            _parent = parent;

        }

        public Type Type { get { return _propertyInfo.PropertyType; } }

        public Action OnChange { get; private set; }

        private string _labelText;
        public string LabelText
        {
            get
            {
                if (string.IsNullOrEmpty(_labelText))
                {
                    _labelText = _propertyInfo.Name;
                }
                return _labelText;
            }
            set
            {
                _labelText = value;
            }
        }

        private object _value;
        public object Value
        {
            get
            {
                if (_value is null)
                {
                    _value = _propertyInfo.GetValue(_model);
                }
                return _value;
            }
            set
            {
                _value = value;
                _propertyInfo.SetValue(_model, value);
                _parent?.OnChangeModel(_model);
            }
        }

        public System.Linq.Expressions.MemberExpression Expression
        {
            get

            { return System.Linq.Expressions.MemberExpression.Property(System.Linq.Expressions.Expression.Constant(_model, typeof(TModel)), _propertyInfo.Name); }
        }

        public void SetComponentInfo()
        {
            ComponentParameters.Add("Value", Value);

            ComponentParameters.Add("ValueExpression", System.Linq.Expressions.Expression.Lambda<Func<TEnum>>(Expression));
            ComponentParameters.Add("ValueChanged", CreateEventCallback<TEnum>());

            var fragments = new List<RenderFragment>();
            foreach (var value in Enum.GetValues(_propertyInfo.PropertyType))
            {
                if (value != null)
                {
                    fragments.Add(CreateSelectOption(value, value.ToString()));
                }
            }
            var singleFragment = CreateSingleFragment(fragments);
            ComponentParameters.Add("ChildContent", singleFragment);

            ComponentType = typeof(InputSelect<TEnum>);
        }

        private EventCallback<TEnum> CreateEventCallback<TValue>()
        {

            return EventCallback.Factory.Create<TEnum>(this, (e) => {

                Value = e;
            });

        }
        private RenderFragment CreateSelectOption(object value, string displayText) => builder =>
        {
            builder.OpenElement(0, "option");
            builder.AddAttribute(1, "value", value);
            builder.AddContent(2, displayText);
            builder.CloseElement();
        };

        private RenderFragment CreateSingleFragment(List<RenderFragment> fragments) => builder =>
        {
            foreach (var fragment in fragments)
            {
                fragment(builder);
            }
        };
        private Type GenericTypeCreator(Type type1, Type genericType)
        {
            Type classType = type1;
            Type[] typeParams = new Type[] { genericType };
            Type constructedType = classType.MakeGenericType(typeParams);
            return constructedType;
        }
    }
}
