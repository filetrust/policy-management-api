using Glasswall.PolicyManagement.Common.Store;

namespace Glasswall.PolicyManagement.Business.Store
{
    public class HistorySearch : IPathActions
    {
        public PathAction DecideAction(string path)
        {
            if (path.EndsWith("historical"))
                return PathAction.Recurse;

            if (path.Contains("historical"))
                return PathAction.Collect;

            return PathAction.Continue;
        }
    }
}