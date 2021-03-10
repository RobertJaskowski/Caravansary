using Caravansary.Properties;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Caravansary
{
    public static class GlobalSettings
    {

        public static Version _version;
        public static Version Version
        {
            get
            {
                if (_version == null)
                {
                    var v = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    _version = new Version(v);

                }


                return _version;
            }

        }


        //public static BlacklistItems _settingsBlacklistItems;
        public static BlacklistItems SettingsBlackListItems
        {
            get
            {
                return Settings.Default.BlacklistItems;
            }

            set
            {
                Settings.Default.BlacklistItems = value;
                Settings.Default.Save();
            }
        }
    }


    public class Version
    {
        public int major;
        public int minor;
        public int build;

        public Version(int major, int minor, int build)
        {
            this.major = major;
            this.minor = minor;
            this.build = build;

        }

        public Version(string versionString)
        {

            List<string> vsplit = versionString.Split('.').ToList();


            this.major = int.Parse(vsplit[0]);
            this.minor = int.Parse(vsplit[1]);
            this.build = int.Parse(vsplit[2]);

        }
        public override string ToString()
        {
            return major + "." + minor + "." + build;
        }

        public bool IsLower(Version other)
        {
            if (major < other.major)
                return true;
            else if(major > other.major)
                return false;


            if (minor < other.minor)
                return true;
            else if (minor > other.minor)
                return false;

            if (build < other.build)
                return true;
            else if (build > other.build)
                return false;

            return false;
        }
    }

}
