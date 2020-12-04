using System;
using System.Collections.Generic;
using System.Text;
//The Offsetgenerator does not convert the unions properly. these must be adjusted by hand


namespace OosterhofDesign.CncApi_Netstandard
{
    public enum Offst_KinControldata
    {

        dData = 0,

        dDataRankL_1 = 12,//double largest type (8*12 = 96)

        iData = 0,//dData + (Offst_Double.TotalSize * dDataRankL_1),

        iDataRankL_1 = 12,

        cData = 0,//iData + (Offst_Int.TotalSize * iDataRankL_1),

        cDataRankL_1 = 64,

        TotalSize = Offst_Double.TotalSize * dDataRankL_1,//cData + (Offst_Char.TotalSize * cDataRankL_1),
    }
}
