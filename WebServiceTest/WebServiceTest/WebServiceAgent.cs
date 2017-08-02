using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Xml;

namespace WebServiceTest
{
    public class WebServiceAgent
    {
        private const string CODE_NAMESPACE = "zcs.temp.service";
        private static Dictionary<string, object> _agent;
        private static Dictionary<string, Type> _agentType;

        static WebServiceAgent()
        {
            _agent = new Dictionary<string, object>();
            _agentType = new Dictionary<string, Type>();
        }
        public static object Agent(string url)
        {
            url = url.ToLower();
            if (_agent.Keys.Contains(url))
            {
                return _agent[url];
            }
            else
            {
                SetWebServiceAgent(url);
                return _agent[url];
            }
        }
        public static Type AgentType(string url)
        {
            url = url.ToLower();
            if (_agent.Keys.Contains(url))
            {
                return _agentType[url];
            }
            else
            {
                SetWebServiceAgent(url);
                return _agentType[url];
            }
        }
        /// <summary<
        /// 设置代理
        /// </summary<
        /// <param name="url"<</param<
        public static void SetWebServiceAgent(string url)
        {
            XmlTextReader reader = new XmlTextReader(url + "?wsdl");
            
            //创建和格式化 WSDL 文档
            ServiceDescription sd = ServiceDescription.Read(reader);

            //创建客户端代理代理类
            ServiceDescriptionImporter sdi = new ServiceDescriptionImporter();
            sdi.AddServiceDescription(sd, null, null);
            
            //使用 CodeDom 编译客户端代理类
            CodeNamespace cn = new CodeNamespace(CODE_NAMESPACE);
            CodeCompileUnit ccu = new CodeCompileUnit();
            ccu.Namespaces.Add(cn);
            sdi.Import(cn, ccu);
            Microsoft.CSharp.CSharpCodeProvider icc = new Microsoft.CSharp.CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();
            cp.GenerateExecutable = false;
            //cp.OutputAssembly = "temp.dll";//输出程序集的名称
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.XML.dll");
            cp.ReferencedAssemblies.Add("System.Web.Services.dll");
            cp.ReferencedAssemblies.Add("System.Data.dll");
            CompilerResults cr = icc.CompileAssemblyFromDom(cp, ccu);
            if (true == cr.Errors.HasErrors)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (System.CodeDom.Compiler.CompilerError ce in cr.Errors)
                {
                    sb.Append(ce.ToString());
                    sb.Append(System.Environment.NewLine);
                }
                throw new Exception(sb.ToString());
            }
            //生成代理实例
            string key = url.ToLower();
            Type agentType = cr.CompiledAssembly.GetTypes()[0]; //编译生成程序集Type
            _agentType.Add(key, agentType);
            object agent = Activator.CreateInstance(agentType); //生成代理实例
            _agent.Add(key, agent);
        }

        ///<summary<
        ///调用指定的方法
        ///</summary<
        ///<param name="methodName"<方法名，大小写敏感</param<
        ///<param name="args"<参数，按照参数顺序赋值</param<
        ///<returns<Web服务的返回值</returns<
        public static object Invoke(string url, string methodName, params object[] args)
        {
            MethodInfo mi = AgentType(url).GetMethod(methodName);
            return Invoke(url, mi, args);
        }
        ///<summary<
        ///调用指定方法
        ///</summary<
        ///<param name="method"<方法信息</param<
        ///<param name="args"<参数，按照参数顺序赋值</param<
        ///<returns<Web服务的返回值</returns<
        public static object Invoke(string url, MethodInfo method, params object[] args)
        {
            object agent = Agent(url);
            return method.Invoke(agent, args);
        }
        public static MethodInfo[] Methods(string url)
        {
            return AgentType(url).GetMethods();
        }
        public static MethodInfo Method(string url, string methodName)
        {
            return AgentType(url).GetMethod(methodName);
        }
    }
}
