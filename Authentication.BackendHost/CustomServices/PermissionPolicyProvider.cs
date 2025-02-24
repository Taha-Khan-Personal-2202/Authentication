using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Authentication.BackendHost.CustomServices
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        private static readonly ConcurrentDictionary<string, AuthorizationPolicy> _policies = new();

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            Console.WriteLine($"🔍 Checking policy: {policyName}");

            if (_policies.TryGetValue(policyName, out var policy))
            {
                Console.WriteLine($"✅ Found cached policy: {policyName}");
                return Task.FromResult(policy);
            }

            Console.WriteLine($"⚡ Creating new policy: {policyName}");
            policy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            _policies[policyName] = policy;
            return Task.FromResult(policy);
        }


        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => FallbackPolicyProvider.GetDefaultPolicyAsync();
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => FallbackPolicyProvider.GetFallbackPolicyAsync();
    }
}
