﻿using System;
using System.Linq;

namespace QuestMaster.LuaParser
{ 
    public enum ItemFilter : byte
    {
        NQ = 0,
        HQ = 1,
        NQ_OR_HQ = 2,
        NQ_AND_HQ = 3,

    }
}
