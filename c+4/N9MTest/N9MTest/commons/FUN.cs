using RM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N9MTest.commons
{
    public class FUN
    {
        private RMXML _xmlconfig = null;
        /// <summary>
        /// 一次性载入，之后如果xml文件内容变更将不会读取到
        /// </summary>
        public RMXML xmlConfig
        {
            get
            {
                if (_xmlconfig == null)
					_xmlconfig = new RMXML($"{Core.WorkPath}Config.xml", true);
                return (_xmlconfig);
            }
        }
    }
}
