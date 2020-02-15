﻿using System;
using System.Linq;

namespace QuestMaster.LuaParser
{
    public struct GetNumOfItems : ITODO
    {
        public ItemFilter filter { get; set; }
        public int ItemId { get; set; }
        public bool UnkA { get; set; }
        public object UnkB { get; set; }
        public int[] Args { get; set; }
    }
}
