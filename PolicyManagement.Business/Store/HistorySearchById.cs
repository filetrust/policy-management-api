using System;
using System.Linq;
using Glasswall.PolicyManagement.Common.Store;

namespace Glasswlal.PolicyManagement.Business.Store
{
    public class HistorySearchById : IPathFilter
    {
        private readonly Guid _policyId;
        
        public HistorySearchById(Guid policyId)
        {
            _policyId = policyId;
        }

        public PathAction DecideAction(string path)
        {
            var splitPath = path.Split('/');

            if (splitPath.FirstOrDefault() == "historical")
            {
                return splitPath.Length == 1 ? PathAction.Recurse 
                    : Guid.Parse(splitPath[1]) == _policyId ? PathAction.Collect
                    : PathAction.Continue;
            }

            return PathAction.Continue;
        }
    }
}