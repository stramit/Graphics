namespace UnityEditor.ShaderGraph
{
    interface IContext
    {
        string name { get; }
    }

    class TestContext : IContext
    {
        public string name => "Test";
    }
}
