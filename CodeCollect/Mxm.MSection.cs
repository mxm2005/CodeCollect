using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Configuration;
using System.Collections;

namespace Mxm.MSection
{
    /// <summary>
    /// 使用配置样例
    /// mxm 2017-4-7
    /// <configSections>
    ///  <section name = "schedule" type="TestDemo.schedule,TestDemo"/>
    /// </configSections>
    /// <schedule file = "MyConfig.xml"></ schedule>
    /// <schedule>
    ///   <jobs>
    ///     <job name="neSyncJob">
    ///       <jtype class="CAPTaskApp.stdDb.neSyncJob" assembly="CAPTaskApp" />
    ///       <cron start-time="2016-08-15T00:00:00+08:00" cron-expression="0 0 17 * * ?" />
    ///     </job>
    ///     <job name = "neSyncJobtest">
    ///       <jtype class="CAPTaskApp.stdDb.neSyncJob" assembly="CAPTaskApp" />
    ///       <cron start-time="2016-08-15T00:00:00+08:00" cron-expression="0 0 17 * * ?" />
    ///     </job>
    ///   </jobs>
    /// </schedule>
    /// </summary>
    public class ScheduleHandler : IConfigurationSectionHandler
    {
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            XmlDocument configDoc = null;
            if (section.HasChildNodes)
            {
                configDoc = section.OwnerDocument;
            }
            else
            {
                XmlAttribute attr = section.Attributes["file"];
                if (attr == null)
                {
                    throw new Exception("mySection节点为空时，必须指定file属性");
                }
                string configFile = attr.Value;
                configFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configFile);
                configDoc = new XmlDocument();
                configDoc.Load(configFile);
            }
            return new ScheduleSectoin(configDoc);
        }
    }
    public class ScheduleSectoin
    {


        public ScheduleSectoin(XmlDocument xdoc)
        {
            InitDoc(xdoc);
        }


        #region 变量属性

        public List<Job> Jobs { get; set; }

        public Job this[string name]
        {
            get
            {
                return Jobs.FirstOrDefault<Job>(w => w.Name == name);
            }
        }


        #endregion

        void InitDoc(XmlDocument xdoc)
        {
            this.Jobs = new List<Job>();
             XmlNodeList jobNodes = xdoc.SelectNodes("schedule/jobs/job");

            foreach (XmlNode jNode in jobNodes)
            {
                Job nb = new Job();
                string jName = ((XmlElement)jNode).GetAttribute("name");
                XmlNode jType = jNode.SelectSingleNode("./jtype");
                XmlNode jCron = jNode.SelectSingleNode("./cron");
                nb.Name = jName;
                if (jType != null)
                {
                    Jtype jj = new Jtype();
                    XmlElement xe = (XmlElement)jType;
                    jj.Class = xe.GetAttribute("class");
                    jj.Assembly = xe.GetAttribute("assembly");
                    nb.JobType = jj;
                }
                if (jCron != null)
                {
                    Cron cc = new Cron();
                    XmlElement xe = (XmlElement)jCron;
                    cc.cron_expression = xe.GetAttribute("cron-expression");
                    cc.start_time = DateTime.Parse(xe.GetAttribute("start-time"));
                    nb.JobCron = cc;
                }
                this.Jobs.Add(nb);
            }
        }

    }

    public class Job : ConfigurationElement
    {
        [ConfigurationProperty("name")]
        public string Name
        {
            get
            {
                return this["name"] as string;
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("jtype")]
        public Jtype JobType
        {
            get
            {
                return (Jtype)this["jtype"];
            }
            set { this["jtype"] = value; }
        }

        [ConfigurationProperty("cron")]
        public Cron JobCron
        {
            get
            {
                return (Cron)this["cron"];
            }
            set { this["cron"] = value; }
        }
        #region other proporty
        public DateTime NextRunTime { get; set; }
        #endregion
    }

    public class Jtype : ConfigurationElement
    {
        [ConfigurationProperty("assembly")]
        public string Assembly
        {
            get
            {
                return this["assembly"] as string;
            }
            set
            {
                this["assembly"] = value;
            }
        }

        [ConfigurationProperty("class")]
        public string Class
        {
            get
            {
                return this["class"] as string;
            }
            set
            {
                this["class"] = value;
            }
        }
    }

    public class Cron : ConfigurationElement
    {
        [ConfigurationProperty("start-time")]
        public DateTime start_time
        {
            get { return (DateTime)this["start-time"]; }
            set { this["start-time"] = value; }
        }

        [ConfigurationProperty("cron-expression")]
        public string cron_expression
        {
            get
            {
                return this["cron-expression"] as string;
            }
            set
            {
                this["cron-expression"] = value;
            }
        }
    }
}
