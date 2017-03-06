using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADManagement.Model
{
    using System.Reflection;

    public class Phone
    {
        public string distinguishedName { get; set; } //CN=iPad§ApplDMPLG480F18P,CN=ExchangeActiveSyncDevices,

        public string msExchDeviceFriendlyName { get; set; } //Svart iPad

        public string msExchDeviceType { get; set; } //iPad

        public DateTime msExchFirstSyncTime { get; set; } //26.11.2013 09.13.45

        public int msExchDeviceAccessState { get; set; } //1

        public int msExchDeviceAccessStateReason { get; set; } //2

        public string msExchDeviceModel { get; set; } //iPad3C6

        public string msExchDeviceOS { get; set; } //iOS 8.3 12F69

        public DateTime whenCreated { get; set; } //26.11.2013 09.13.45

        public DateTime whenChanged { get; set; } //07.01.2016 12.59.14

        public object this[string propertyName]
        {
            get
            {
                // probably faster without reflection:
                // like:  return Properties.Settings.Default.PropertyValues[propertyName] 
                // instead of the following
                Type myType = typeof(Phone);
                PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                return myPropInfo.GetValue(this, null);
            }
            set
            {
                
                Type myType = typeof(Phone);
                if (myType.GetProperties().ToList().Exists(n => n.Name == propertyName))
                {
                    PropertyInfo myPropInfo = myType.GetProperty(propertyName);
                    ;
                    myPropInfo.SetValue(this, value, null);
                }
                
               

            }
        }
    }
}
