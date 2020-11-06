using Glasswall.PolicyManagement.Common.Store;

namespace Glasswall.PolicyManagement.Business.Store
{
    public class HistorySearch : IPathFilter
    {
        public PathAction DecideAction(string path)
        {
            if (path == "historical")
                return PathAction.Recurse;

            if (path.StartsWith("historical"))
                return PathAction.Collect;

            return PathAction.Continue;
        }
    }
}