using System;
using System.Windows.Controls;

namespace Plugger.Contracts
{
    public interface IPlugger
    {
        /// <summary>  
        /// Name of plugger  
        /// </summary>  
        string PluggerName { get; set; }

        /// <summary>  
        /// It will return UserControl which will display on Main application  
        /// </summary>  
        /// <returns></returns>  
        UserControl GetPlugger();
    }
}
