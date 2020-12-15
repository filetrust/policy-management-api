namespace Glasswall.PolicyManagement.Common.Store
{
    public interface IPathActions
    {
        PathAction DecideAction(string path);
    }
}