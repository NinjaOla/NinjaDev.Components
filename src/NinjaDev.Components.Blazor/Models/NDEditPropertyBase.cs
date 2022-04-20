using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NinjaDev.Components.Blazor.Models
{
    internal abstract class NDEditPropertyBase<TModel> : INDEditProperty
    {
        protected PropertyInfo _propertyInfo;
        protected TModel _model;
        protected IChangeModel<TModel> _parent;

        #region INDEditProperty
        public virtual Dictionary<string, object> ComponentParameters { get; } = new Dictionary<string, object>();
        public virtual Type ComponentType { get; internal set; }
        #endregion


        #region Interals
        public virtual Type Type { get { return _propertyInfo.PropertyType; } }

        private string _labelText;
        public virtual string LabelText
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
        public virtual object Value
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
                OnChange();
            }
        }

        public System.Linq.Expressions.MemberExpression Expression
        {
            get
            {
                return System.Linq.Expressions.MemberExpression.Property(System.Linq.Expressions.Expression.Constant(_model, typeof(TModel)), _propertyInfo.Name);
            }
        }
        #endregion

        public NDEditPropertyBase(TModel model, PropertyInfo propertyInfo, IChangeModel<TModel> parent)
        {
            _propertyInfo = propertyInfo;
            _model = model;
            _parent = parent;

        }

        public NDEditPropertyBase(TModel model, PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
            _model = model;
        }

        public virtual void OnChange()
        {
            _parent?.OnChangeModel(_model);
        }

        protected virtual RenderFragment CreateSingleFragment(List<RenderFragment> fragments) => builder =>
        {
            foreach (var fragment in fragments)
            {
                fragment(builder);
            }
        };
    }
}
