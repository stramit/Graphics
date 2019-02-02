using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Graphing.Util;

namespace UnityEditor.ShaderGraph
{
    struct ControlDictionary
    {
        private Dictionary<SlotValueType, Type[]> m_Dictionary;
        public Dictionary<SlotValueType, Type[]> dictionary
        {
            get
            {
                if(m_Dictionary == null)
                    m_Dictionary = Generate();
                return m_Dictionary;
            }
        }

        private List<Type> m_TempList;
        private List<Type> tempList
        {
            get
            {
                if(m_TempList == null)
                    m_TempList = new List<Type>();
                return m_TempList;
            }
        }

        private Dictionary<SlotValueType, Type[]> Generate()
        {
            List<IShaderControl> controls = new List<IShaderControl>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypesOrNothing())
                {
                    // TODO - Support generic texture types
                    if (type.IsClass && !type.IsAbstract && typeof(IShaderControl).IsAssignableFrom(type) 
                        && type != typeof(TextureControl<>) && type != typeof(DefaultControl))
                        controls.Add(Activator.CreateInstance(type) as IShaderControl);
                }
            }

            var output = new Dictionary<SlotValueType, Type[]>();
            for(int t = 0; t < Enum.GetNames(typeof(SlotValueType)).Length; t++)
            {
                tempList.Clear();
                for(int c = 0; c < controls.Count; c++)
                {
                    if(controls[c].validPortTypes.ToList().Contains((SlotValueType)t))
                        tempList.Add(controls[c].GetType());
                }
                output.Add((SlotValueType)t, tempList.ToArray());
            }
            return output;
        }

        public string[] GetControlEntries(SlotValueType valueType)
        {
            Type[] types;
            dictionary.TryGetValue(valueType, out types);
            return types.Select(s => s.Name).ToArray();
        }

        public int GetIndexOfControl(SlotValueType valueType, IShaderControl control)
        {
            Type[] types;
            dictionary.TryGetValue(valueType, out types);
            return Array.IndexOf(types, control.GetType());
        }

        public IShaderControl GetControlFromIndex(SlotValueType valueType, int index)
        {
            Type[] types;
            dictionary.TryGetValue(valueType, out types);
            return (IShaderControl)Activator.CreateInstance(types[index]);
        }
    }
}