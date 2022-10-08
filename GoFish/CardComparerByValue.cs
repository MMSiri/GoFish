using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace GoFish
{
    public class CardComparerByValue : IComparer<Card>
    {
        public int Compare([AllowNull] Card x, [AllowNull] Card y)
        {
            if (x.Suit < y.Suit) return -1;
            else if (x.Suit > y.Suit) return 1;
            else
                if (x.Value < y.Value) return -1;
            else if (x.Value > y.Value) return 1;
            else return 0;
        }
    }
}
