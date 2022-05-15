using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LazyReader.Enums
{
    public enum TextDisplayEnum
    {
        [Display(Name = "保持文字一直显示")]
        Normal = 1,
        [Display(Name = "鼠标离开时文字隐藏，移上时显示")]
        MoveUp = 2,
        [Display(Name = "鼠标离开时文字隐藏，双击时显示")]
        Dblclick = 3,
    }
}
