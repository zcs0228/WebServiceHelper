using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Xml.Serialization;

namespace WebServiceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ///由于每次获得代理是非常损失性能的操作，所以可以使用单例模式进行优化

            //http://10.24.13.154/cwbase/service/mdm/MDMZdfwMappingSrv.asmx
            //http://10.24.13.154/cwbase/service/mdm/MDMAdapterDataExportSrv.asmx
            string url = "http://10.24.13.158/cwbase/service/mdm/MDMDBLinkSrv.asmx";
            MethodInfo[] m = WebServiceAgent.Methods(url);

            MethodInfo t = WebServiceAgent.Method(url,"GetDBLinkList");

            object[] param = { "OE73", "02", "ORA", "MDMZGZD", "" };
            object result = WebServiceAgent.Invoke(url,"GetYwxtTableColList", param);
        }
    }
}
