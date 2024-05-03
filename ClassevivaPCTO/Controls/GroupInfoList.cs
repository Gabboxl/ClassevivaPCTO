using System.Collections.Generic;

namespace ClassevivaPCTO.Controls;

public class GroupInfoList : List<object>
{
    public GroupInfoList(IEnumerable<object> items) : base(items)
    {
    }

    public object Key { get; set; }
}