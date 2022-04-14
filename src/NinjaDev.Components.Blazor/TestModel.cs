using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaDev.Components.Blazor
{
    public class TestModel
    {
        [MaxLength(5)]
        public string Text { get; set; }
        public int Int { get; set; }
        public long Longtest { get; set; }
        public bool Boolean { get; set; } = true;
        public TestEnum Enum { get; set; } 
        public List<SubModel> List { get; set; }
    }

    public class SubModel
    {

        public string SubName { get; set; }

    }
    public enum TestEnum { get,set,totally};
}
