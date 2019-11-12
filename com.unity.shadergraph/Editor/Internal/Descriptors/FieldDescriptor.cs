namespace UnityEditor.ShaderGraph.Internal
{
    public sealed class FieldDescriptor
    {
        // Default
        public string tag { get; }
        public string name { get; }
        public string define { get; }

        // StructField
        public string type { get; }
        public int vectorCount { get; }
        public string semantic { get; }
        public string preprocessor { get; }
        public StructFieldOptions subscriptOptions { get; }

        public FieldDescriptor(string tag, string name, string define)
        {
            this.tag = tag;
            this.name = name;
            this.define = define;
            // TODO: Statically define master node slots
//            FieldRegistry.instance.Add(this);
        }

        public FieldDescriptor(string tag, string name, string define, ShaderValueType type,
                string semantic = "", string preprocessor = "", StructFieldOptions subscriptOptions = StructFieldOptions.Static)
        {
            this.tag = tag;
            this.name = name;
            this.define = define;
            this.type = type.ToShaderString();
            this.vectorCount = type.GetVectorCount();
            this.semantic = semantic;
            this.preprocessor = preprocessor;
            this.subscriptOptions = subscriptOptions;
            if ((subscriptOptions & StructFieldOptions.Hidden) == 0)
            {
                FieldRegistry.instance.Add(this);
            }
        }

        public FieldDescriptor(string tag, string name, string define, string type,
                string semantic = "", string preprocessor = "", StructFieldOptions subscriptOptions = StructFieldOptions.Static)
        {
            this.tag = tag;
            this.name = name;
            this.define = define;
            this.type = type;
            this.vectorCount = 0;
            this.semantic = semantic;
            this.preprocessor = preprocessor;
            this.subscriptOptions = subscriptOptions;
            if ((subscriptOptions & StructFieldOptions.Hidden) == 0)
            {
                FieldRegistry.instance.Add(this);
            }
        }
    }
}
