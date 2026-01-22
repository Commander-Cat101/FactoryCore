namespace FactoryCore.API
{
    public static class ModuleExtentions
    {
        public static void ProcessAll(this IEnumerable<Module> modules)
        {
            foreach (var module in modules)
            {
                module.ProcessModule();
            }
        }
        public static string[] AsGuids(this IEnumerable<Module> modules)
        {
            return modules.Select(a => a.Id.ToString()).ToArray();
        }
    }
}
