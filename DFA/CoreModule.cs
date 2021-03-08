﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DFA
{
    public abstract class CoreModule : BaseViewModel
    {
        public abstract string ModuleName { get; }


        public abstract void CloseModule();
    }
}
