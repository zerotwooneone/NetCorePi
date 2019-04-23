using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace MvcApp.JsonConfig
{
    public class SettingValidationStartupFilter : IStartupFilter
    {
        readonly IEnumerable<IValidatableConfig> _validatableObjects;
        public SettingValidationStartupFilter(IEnumerable<IValidatableConfig> validatableObjects)
        {
            _validatableObjects = validatableObjects;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            foreach (var validatableObject in _validatableObjects)
            {
                validatableObject.Validate();
            }

            //don't alter the configuration
            return next;
        }
    }
}