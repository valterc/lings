﻿using LiNGS.Common;
using LiNGS.Common.GameLogic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LiNGS.Client.GameLogic
{
    internal class NetworkedObject
    {
        private readonly static Object StaticLock = new Object();
        private static int NameId = 0;

        internal INetworkedObject OriginalObject { get; private set; }
        internal string Name { get; private set; }
        internal bool AutoCreateObject { get; private set; }
        internal FieldInfo[] Fields { get; private set; }
        internal bool[] IsFieldSimulated { get; private set; }
        internal MethodInfo setActiveMethod;

        public NetworkedObject(INetworkedObject obj, bool useRealName = false, bool autoCreateObject = true)
        {
            this.OriginalObject = obj;
            this.Fields = obj.GetType().
                GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                .Where(f => f.GetCustomAttributes(typeof(NetworkedField), true).Length > 0)
                .OrderBy(f => f.Name)
                .ToArray();

            this.setActiveMethod = obj.GetType().GetMethod("LiNGSSetActive", new Type[] { typeof(bool) });
            this.IsFieldSimulated = this.Fields.Select(f => (f.GetCustomAttributes(typeof(NetworkedField), true)[0] as NetworkedField).Simulated).ToArray();
            this.AutoCreateObject = autoCreateObject;

            if (useRealName)
            {
                this.Name = LiNGSMarkers.AutoCreatedObject + obj.GetType().Name;
            }
            else
            {
                //Thread safe
                lock (StaticLock)
                {
                    this.Name = LiNGSMarkers.AutoCreatedObject + NameId++.ToString();
                }
            }
        }

        public NetworkedObject(INetworkedObject obj, string name)
        {
            this.OriginalObject = obj;
            this.Fields = obj.GetType().
                GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)
                .Where(f => f.GetCustomAttributes(typeof(NetworkedField), true).Length > 0)
                .OrderBy(f => f.Name)
                .ToArray();

            this.setActiveMethod = obj.GetType().GetMethod("LiNGSSetActive", new Type[] { typeof(bool) });
            this.IsFieldSimulated = this.Fields.Select(f => (f.GetCustomAttributes(typeof(NetworkedField), true)[0] as NetworkedField).Simulated).ToArray();
            this.AutoCreateObject = false;
            this.Name = name;
        }

        public void ReceiveValue(string fieldIndex, string value)
        {
            FieldInfo f = Fields[int.Parse(fieldIndex)];
            f.SetValue(OriginalObject, Convert.ChangeType(value, f.FieldType, CultureInfo.InvariantCulture));
        }

        public void SetActive(bool active)
        {
            if (setActiveMethod != null)
            {
                setActiveMethod.Invoke(OriginalObject, new object[] { active });
            }
        }
       
    }
}
