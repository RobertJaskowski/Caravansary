using System;
using System.Windows.Controls;

namespace Caravansary.SDK.Contracts
{
    public interface IPlugger
    {
        /// <summary>  
        /// Name of Caravansary.SDK  
        /// </summary>  
        string  Name { get; set; }

        /// <summary>  
        /// It will return UserControl which will display on Main application  
        /// </summary>  
        /// <returns></returns>  
        UserControl GetPlugger();
    }
}
