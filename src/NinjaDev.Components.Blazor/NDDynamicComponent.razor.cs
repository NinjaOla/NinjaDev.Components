using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using NinjaDev.Components.Blazor.Helpers;
using NinjaDev.Components.Blazor.Models;
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

      
        public IEnumerable<INDEditProperty> CreateComponents()
        {

            var list = typeof(T).GetProperties();
            foreach (var propertyInfo in typeof(T).GetProperties())
            {

                if (propertyInfo.PropertyType.IsArray)
                {
                    ICanSetComponentInfo variable = CreateEditPropertyArray(propertyInfo);
                    variable.SetComponentInfo();
                    yield return variable as INDEditProperty;

                }
                //else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().GetInterfaces().Any(i => i.Name == nameof(IList)))
                //{
                //    NDEditPropertyArray<IEnumerable<T>, T> nDEditPropertyArray = new NDEditPropertyArray<IEnumerable<T>, T>((IEnumerable<T>)Model, propertyInfo, this, false);
                //    //nDEditPropertyArray.SetComponentInfo();

                //    yield return nDEditPropertyArray;

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

        ICanSetComponentInfo CreateEditPropertyEnum(PropertyInfo propertyInfo)
        {
            return typeof(NDEditPropertyEnum<,>)
                .CreateType(new Type[] { typeof(T), propertyInfo.PropertyType })
                .CreateInstance(new object[] { Model, propertyInfo, this }) as ICanSetComponentInfo;
        }

        ICanSetComponentInfo CreateEditPropertyArray(PropertyInfo propertyInfo, bool isRenderSublist = false)
        {
            return typeof(NDEditPropertyArray<,,>)
                .CreateType(new Type[] { typeof(T), propertyInfo.PropertyType, propertyInfo.PropertyType.FindElementType()})
                .CreateInstance(new object[] { Model, propertyInfo.GetValue(Model),  propertyInfo, this, isRenderSublist }) as ICanSetComponentInfo;
        }
    }
   
}
