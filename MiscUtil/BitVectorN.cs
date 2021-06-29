using System;
using System.Collections.Generic;
using System.Text;

namespace MiscUtil
{
    public struct BitVectorN
    {
        /*
         * IDEA:
         * https://github.com/dotnet/corefx/blob/master/src/System.Collections.Specialized/src/System/Collections/Specialized/BitVector32.cs
         * BitVector32, but using Generators
         *    - to allow it to use any backing integer size from 8 to 64 depending on number of flags or Section sizes
         *    - to allow Sections to return an appropriately-sized integer instead of only int32
         */
    }
}
