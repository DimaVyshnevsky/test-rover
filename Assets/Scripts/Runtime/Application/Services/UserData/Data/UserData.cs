using System;
using System.Collections.Generic;

namespace Application.Services.UserData
{
    [Serializable]
    public class UserData
    {
        public SettingsData SettingsData = new SettingsData();
    }
}