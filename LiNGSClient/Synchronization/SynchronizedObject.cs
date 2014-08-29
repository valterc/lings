using LiNGS.Client.GameLogic;
using LiNGS.Common.GameLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LiNGS.Client.Synchronization
{
    internal class SynchronizedObject
    {
        internal String Name { get; set; }
        internal NetworkedObject Object { get; set; }
        internal Dictionary<string, string> FieldsValue { get; private set; }
        internal bool ToDelete { get; set; }

        public SynchronizedObject()
        {
            FieldsValue = new Dictionary<string, string>();
        }

        public SynchronizedObject(string name, NetworkedObject networkedObject)
        {
            this.Name = name;
            this.Object = networkedObject;
            this.FieldsValue = new Dictionary<string, string>();

            for (int i = 0; i < networkedObject.Fields.Length; i++)
            {
                string value = networkedObject.Fields[i].GetValue(networkedObject.OriginalObject) != null ? networkedObject.Fields[i].GetValue(networkedObject.OriginalObject).ToString() : null;
                this.FieldsValue.Add(i.ToString(), value);
            }

        }

    }
}
