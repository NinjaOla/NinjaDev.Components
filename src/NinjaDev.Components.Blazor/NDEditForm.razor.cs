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


        void FormSubmitted(EditContext editContext)
        {
            bool formIsValid = editContext.Validate();
        }
    }

    
}
