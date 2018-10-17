using System.Collections.Generic;

namespace Wgetter
{

    public static class Extensions
    {
        public static List<string> GoesToEleven(this List<string> lst)
        {
            if (lst.Count == 1)
            {
                for (var i = 0; i < 10; i++)
                {
                    lst.Add(lst[0]);
                }
            }

            return lst;
        }
    }
}