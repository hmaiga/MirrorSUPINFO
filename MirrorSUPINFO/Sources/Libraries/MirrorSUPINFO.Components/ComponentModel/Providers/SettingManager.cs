﻿// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>

using Windows.Storage;
using GalaSoft.MvvmLight;
// ReSharper disable InconsistentNaming

namespace MirrorSUPINFO.Components.ComponentModel.Providers
{
    /// <summary>
    /// Provide access to the application's settings
    /// </summary>
    public sealed class SettingManager : ViewModelBase 
    {
        #region Fields

        private static SettingManager _settingManager = null;

        private readonly ApplicationDataContainer _localSettings = ApplicationData.Current.LocalSettings;

        #endregion
        
        #region Properties
    
        /// <summary>
        /// Gets the value of FirstRun
        /// </summary>
        public System.Boolean FirstRun
        {
            get 
            {
                return (System.Boolean)_localSettings.Values["FirstRun"];
            }
            set
            {
                _localSettings.Values["FirstRun"] = value;
                RaisePropertyChanged();
            }
        }
    
        /// <summary>
        /// Gets the value of VoiceCommandPrefixMode
        /// </summary>
        public System.Int32 VoiceCommandPrefixMode
        {
            get 
            {
                return (System.Int32)_localSettings.Values["VoiceCommandPrefixMode"];
            }
            set
            {
                _localSettings.Values["VoiceCommandPrefixMode"] = value;
                RaisePropertyChanged();
            }
        }
    
        /// <summary>
        /// Gets the value of SpeechSynthesisGender
        /// </summary>
        public System.String SpeechSynthesisGender
        {
            get 
            {
                return (System.String)_localSettings.Values["SpeechSynthesisGender"];
            }
            set
            {
                _localSettings.Values["SpeechSynthesisGender"] = value;
                RaisePropertyChanged();
            }
        }
    
        /// <summary>
        /// Gets the value of UserGender
        /// </summary>
        public System.String UserGender
        {
            get 
            {
                return (System.String)_localSettings.Values["UserGender"];
            }
            set
            {
                _localSettings.Values["UserGender"] = value;
                RaisePropertyChanged();
            }
        }
    
        /// <summary>
        /// Gets the value of NetworkSsid
        /// </summary>
        public System.String NetworkSsid
        {
            get 
            {
                return (System.String)_localSettings.Values["NetworkSsid"];
            }
            set
            {
                _localSettings.Values["NetworkSsid"] = value;
                RaisePropertyChanged();
            }
        }
    
        /// <summary>
        /// Gets the value of NetworkPassword
        /// </summary>
        public System.String NetworkPassword
        {
            get 
            {
                return (System.String)_localSettings.Values["NetworkPassword"];
            }
            set
            {
                _localSettings.Values["NetworkPassword"] = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Constructors
        
        /// <summary>
        /// Initialize a new instance of <see cref="SettingManager"/>
        /// </summary>
        public SettingManager()
        { 
            if (!_localSettings.Values.ContainsKey("FirstRun"))
            {  
                _localSettings.Values.Add("FirstRun", true);
            }
 
            if (!_localSettings.Values.ContainsKey("VoiceCommandPrefixMode"))
            {  
                _localSettings.Values.Add("VoiceCommandPrefixMode", 1);
            }
 
            if (!_localSettings.Values.ContainsKey("SpeechSynthesisGender"))
            { 
                _localSettings.Values.Add("SpeechSynthesisGender", "Male");
            }
 
            if (!_localSettings.Values.ContainsKey("UserGender"))
            { 
                _localSettings.Values.Add("UserGender", "Male");
            }
 
            if (!_localSettings.Values.ContainsKey("NetworkSsid"))
            { 
                _localSettings.Values.Add("NetworkSsid", "");
            }
 
            if (!_localSettings.Values.ContainsKey("NetworkPassword"))
            { 
                _localSettings.Values.Add("NetworkPassword", "");
            }
        }

        #endregion

        #region Methods
            
        /// <summary>
        /// Get an instance of <see cref="SettingManager"/>
        /// </summary>
        internal static SettingManager GetProvider()
        {
            return _settingManager ?? (_settingManager = new SettingManager());
        }

        #endregion
    }
}
