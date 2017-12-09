using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.SDK.Tooling
{
    public class XYZ: DataConnection
    {

        //XH: X 轴加速度值整数部分，最高位为符号位
        protected int XH;

        //XL: X 轴加速度值小数部分，基本单位1/256G
        protected int XL;


        //YH: Y 轴加速度值整数部分，最高位为符号位
        protected int YH;

        //YL: Y 轴加速度值小数部分，基本单位1/256G
        protected int YL;

        //ZH: Z 轴加速度值整数部分，最高位为符号位
        protected int ZH;

        //ZL: Z 轴加速度值小数部分，基本单位1/256G
        protected int ZL;

        //SUM1:校验和（0X58 0X59 0X5A XH XL YH YL ZH ZL 相加，结果取低8 位）
        protected int sum1;

        //X: X 轴加速度，最高位为符号位，基本单位1/64G
        protected int X;

        //Y: Y 轴加速度，最高位为符号位，基本单位1/64G
        protected int Y;

        //Z: Z 轴加速度，最高位为符号位，基本单位1/64G
        protected int Z;

        public static XYZ ToObject(string data)
        {
            XYZ xyz = new XYZ();

            return xyz;
        }

        public static string ToString(XYZ xyz)
        {
            string data = "$XYZ";
            data += ",";

            data += "58";
            data += " ";

            data += "59";
            data += " ";

            data += "5A";
            data += " ";




            return data;
        }
    }
}
