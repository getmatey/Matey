﻿using Matey.WebServer.Abstractions.Rules;
using System.Collections.Immutable;

namespace Matey.Rules
{
    public class RequestRuleParser : IRequestRuleParser
    {
        private const string HostToken = "host";

        public IRequestRule Parse(string text)
        {
            if(text.StartsWith($"{HostToken}:", StringComparison.InvariantCultureIgnoreCase))
            {
                return new HostRequestRule(text.Substring($"{HostToken}:".Length).Trim());
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
