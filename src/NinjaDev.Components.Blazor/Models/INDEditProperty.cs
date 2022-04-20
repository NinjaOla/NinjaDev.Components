using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaDev.Components.Blazor.Models
{
    public interface INDEditProperty
    {
        Dictionary<string, object> ComponentParameters { get; }
        Type ComponentType { get; }
    }
}
