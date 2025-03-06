using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyBase.ApplicationEvent
{
    public static class ApplicationEventKey
    {
        public static string ON_VIEW_CHANGE = "ON_VIEW_CHANGE";

        public static string ON_GAME_START = "ON_GAME_START";
        public static string ON_GAME_PAUSE = "ON_GAME_PAUSE";
        public static string ON_GAME_RESUME = "ON_GAME_RESUME";
        public static string ON_GAME_END = "ON_GAME_END";

        public static string ON_ENTER_BACKGROUND = "ON_ENTER_BACKGROUND";
        public static string ON_ENTER_FOREGROUND = "ON_ENTER_FOREGROUND";

        /// Your own events
        /// ...
    }
}

