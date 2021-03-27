using System;
using System.Collections.Generic;

namespace Common
{
    public class Event
    {
        public string type {get; set;}
        public string id {get; set;}
        public string value {get; set;}

        public Event(){}

        public Event(string eventTitle, string elemId, string eventValue)
        {
            type = eventTitle;
            id = elemId;
            value = eventValue;
        }
    }
}